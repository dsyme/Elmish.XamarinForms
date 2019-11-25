// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.

// Notes on adaptive version:
// In this kind of sample, there is a bunch of game logic implemented on top of the model.
// Much of this affects the view, e.g. "is this a legitimate move" or "has someone won the game".
// In this case, you have to either
//    1. write thse computations using the adaptive model, OR
//    2. the information needs to be computed and stored in the immutable model on each update, and the adaption process will
//       apply changes

namespace TicTacToe

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open FSharp.Data.Adaptive

/// Represents a player and a player's move
type Player = 
    | X 
    | O 
    member p.Swap = match p with X -> O | O -> X
    member p.Name = match p with X -> "X" | Y -> "Y"

/// Represents the game state contents of a single cell
type GameCell = 
    | Empty 
    | Full of Player
    member x.CanPlay = (x = Empty)

/// Represents the result of a game
type GameResult = 
    | StillPlaying 
    | Win of Player
    | Draw

/// Represents a position on the board
type Pos = int * int

/// Represents an update to the game
type Msg =
    | Play of Pos
    | Restart
    | SetVisualBoardSize of double

/// Represents the state of the game board
type Board = Map<Pos, GameCell>

/// Represents the elements of a possibly-winning row
type Row = GameCell list

/// Represents the state of the game
type Model =
    { 
      /// Who is next to play
      NextUp: Player

       /// The state of play on the board
      Board: Board

      /// The state of play on the board
      GameScore: (int * int)
      
      /// The model occasionally includes things related to the view.  In this case,
      /// we track the desired visual size of the board, to ensure a square, in response to
      /// updates telling us the overall allocated size.
      VisualBoardSize: double option
    }

type AdaptiveBoard = cmap<Pos, GameCell>

[<RequireQualifiedAccess>]
type AdaptiveModel = 
    { 
      NextUp: cval<Player>
      Board: AdaptiveBoard
      GameScore: cval<(int * int)>
      VisualBoardSize: cval<double option>
    }

module List =
   let existsA (f: 'T -> aval<bool>) xs =  AList.existsA f (AList.ofList xs)
   let forallA (f: 'T -> aval<bool>) xs =  AList.forallA f (AList.ofList xs)

module AMap =
    let find f x = AMap.tryFind f x |> AVal.map Option.get 

/// The model, update and view content of the app. This is placed in an 
/// independent model to facilitate unit testing.
module App = 
    let positions = 
        [ for x in 0 .. 2 do 
            for y in 0 .. 2 do 
               yield (x, y) ]

    let initialBoard = 
        Map.ofList [ for p in positions -> p, Empty ]

    let init () = 
        { NextUp = X
          Board = initialBoard
          GameScore = (0,0)
          VisualBoardSize = Some 400.0 }

    let lines =
        [
            // rows
            for row in 0 .. 2 do yield [(row,0); (row,1); (row,2)]
            // columns
            for col in 0 .. 2 do yield [(0,col); (1,col); (2,col)]
            // diagonals
            yield [(0,0); (1,1); (2,2)]
            yield [(0,2); (1,1); (2,0)]
        ]

    /// Determine the game result, if any.
    let getGameResult (model: AdaptiveModel) =
        let board = model.Board
        let xwin = lines |> List.existsA (List.forallA (fun pos -> AMap.tryFind pos board |> AVal.map (function Some (Full X) -> true | _ -> false) ))
        let owin = lines |> List.existsA (List.forallA (fun pos -> AMap.tryFind pos board |> AVal.map (function Some (Full O) -> true | _ -> false) ))
        let moreMoves = board  |> AMap.exists (fun _ v -> v = Empty)
        (xwin, owin, moreMoves) |||> AVal.map3 (fun xwin owin moreMoves ->
            if xwin then Win X else
            if owin then Win O else
            if moreMoves then StillPlaying
            else Draw
        )

    /// Get a message to show the current game result
    let getMessage (model: AdaptiveModel) = 
        let result = getGameResult model
        (result, model.NextUp) ||> AVal.map2 (fun result nextUp ->
            match result with
            | StillPlaying -> sprintf "%s's turn" nextUp.Name
            | Win p -> sprintf "%s wins!" p.Name
            | Draw -> "It is a draw!"
        )

    /// The 'update' function to update the model
    let update gameOver msg model =
        let newModel = 
            match msg with
            | Play pos -> 
                { model with Board = model.Board.Add(pos, Full model.NextUp)
                             NextUp = model.NextUp.Swap }
            | Restart -> 
                { model with NextUp = X; Board = initialBoard }
            | SetVisualBoardSize size -> 
                { model with VisualBoardSize = Some size }

        // Make an announcement in the middle of the game. 
        //let result = getGameResult newModel
        //if result <> StillPlaying then 
        //    gameOver (getMessage newModel)

        //let newModel2 = 
        //    let (x,y) = newModel.GameScore
        //    match result with 
        //    | Win p -> { newModel with GameScore = (if p = X then (x+1, y) else (x, y+1)) }
        //    | _ -> newModel
            
        // Return the new model.
        newModel

    /// A helper used in the 'view' function to get the name 
    /// of the Xaml resource for the image for a player
    let imageForCell cell =
        let path =
            match cell with
            | Full X -> 
                match Device.RuntimePlatform with
                | Device.macOS -> "Cross"
                | _ -> "Cross.png"
            | Full O -> 
                match Device.RuntimePlatform with
                | Device.macOS -> "Nought"
                | _ -> "Nought.png"
            | Empty -> ""
        Path path

    /// A helper to get the suffix used in the Xaml for a position on the board.
    let uiText (row,col) = sprintf "%d%d" row col

    /// A condition used in the 'view' function to check if we can play in a cell.
    /// The visual contents of a cell depends on this condition.
    let canPlayCell (res: aval<GameResult>) cell =
        res |> AVal.map (fun res -> (cell = Empty) && (res = StillPlaying))

    /// The dynamic 'view' function giving the updated content for the view
    let view (model: AdaptiveModel) dispatch =

      // Compute the view maps - derived adaptive information 
      let gameResult = getGameResult model
      let canPlay = model.Board |> AMap.mapA (fun _ v -> canPlayCell gameResult v)
      let imagesForCells = model.Board |> AMap.map (fun _ v -> imageForCell v)

      // Give the view
      View.NavigationPage(barBackgroundColor = c Color.LightBlue, 
        barTextColor = c Color.Black,
        pages = cs [
           View.ContentPage(
            View.Grid(rowdefs = c [ Star; Auto; Auto ],
              children = cs [
                View.Grid(rowdefs = c [ Star; Absolute 5.0; Star; Absolute 5.0; Star ],
                    coldefs = c [ Star; Absolute 5.0; Star; Absolute 5.0; Star ],
                    children = alist {
                        yield View.BoxView(c Color.Black).Row(c 1).ColumnSpan(c 5)
                        yield View.BoxView(c Color.Black).Row(c 3).ColumnSpan(c 5)
                        yield View.BoxView(c Color.Black).Column(c 1).RowSpan(c 5)
                        yield View.BoxView(c Color.Black).Column(c 3).RowSpan(c 5)

                        for ((row,col) as pos) in positions do
                            let! cp = AMap.find pos canPlay
                            let item = 
                                if cp then 
                                    View.Button(command = c (fun () -> dispatch (Play pos)), 
                                        backgroundColor = c Color.LightBlue)
                                else
                                    View.Image(source = AMap.find pos imagesForCells,
                                        margin = c (Thickness 10.0), 
                                        horizontalOptions = c LayoutOptions.Center,
                                        verticalOptions = c LayoutOptions.Center)
                            yield item.Row(c (row*2)).Column(c (col*2)) 
                       },

                    rowSpacing = c 0.0,
                    columnSpacing = c 0.0,
                    horizontalOptions = c LayoutOptions.Center,
                    verticalOptions = c LayoutOptions.Center,
                    width = (model.VisualBoardSize |> AVal.map (function None -> 100.0 | Some v -> v)),
                    height = (model.VisualBoardSize |> AVal.map (function None -> 100.0 | Some v -> v))
                  ).Row(c 0)

                View.Label(text = getMessage model, 
                    margin = c (Thickness 10.0), 
                    textColor = c Color.Black, 
                    horizontalOptions = c LayoutOptions.Center,
                    verticalOptions = c LayoutOptions.Center,
                    horizontalTextAlignment = c TextAlignment.Center, 
                    verticalTextAlignment = c TextAlignment.Center,
                    fontSize = c (Named NamedSize.Large)).Row(c 1)

                View.Button(command = c (fun () -> dispatch Restart),
                    text = c "Restart game",
                    backgroundColor = c Color.LightBlue,
                    textColor = c Color.Black,
                    fontSize = c (Named NamedSize.Large)).Row(c 2)
              ]),

             //// This requests a square board based on the width we get allocated on the device 
             sizeAllocated = c (fun (width, height) ->
                   let sz = min width height - 80.0
                   dispatch (SetVisualBoardSize sz))
            )])
      |> AVal.constant

    // Display a modal message giving the game result. This is doing a UI
    // action in the model update, which is ok for modal messages. We factor
    // this dependency out to allow unit testing of the 'update' function. 

    let gameOver msg =
        Application.Current.MainPage.DisplayAlert("Game over", msg, "OK") |> ignore


    let ainit (model: Model) : AdaptiveModel = 
        { NextUp = cval model.NextUp
          Board = cmap (HashMap.ofMap model.Board)
          GameScore = cval model.GameScore  
          VisualBoardSize = cval model.VisualBoardSize  }

    let adelta (model: Model) (amodel: AdaptiveModel) =
        transact (fun () -> 
            if model.NextUp <> amodel.NextUp.Value then 
                amodel.NextUp.Value <- model.NextUp
            amodel.Board.Value <- HashMap.ofMap model.Board
            if model.GameScore <> amodel.GameScore.Value then 
                amodel.GameScore.Value <- model.GameScore
            if model.VisualBoardSize <> amodel.VisualBoardSize.Value then 
                amodel.VisualBoardSize.Value <- model.VisualBoardSize)

    let program = 
        Program.mkSimple init (update gameOver) ainit adelta view
        |> Program.withConsoleTrace

#if TESTEVAL
    let testInit = init ()
    let testView = view testInit (fun _ -> ())
#endif

/// Stitch the model, update and view content into a single app.
type App() as app =
    inherit Application()

    let runner = 
        App.program
        |> XamarinFormsProgram.run app
        
#if DEBUG && !TESTEVAL
    do runner.EnableLiveUpdate ()
#endif
