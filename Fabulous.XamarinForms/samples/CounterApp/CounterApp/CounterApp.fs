// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace CounterApp

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open FSharp.Data.Adaptive
open System.Diagnostics

module App = 
    type Model = 
      { Count : int
        Step : int
        TimerOn: bool }

    [<RequireQualifiedAccess>]
    type AdaptiveModel = 
      { Count : cval<int>
        Step : cval<int>
        TimerOn: cval<bool>  }

    let ainit (model: Model) : AdaptiveModel = 
        { Count = cval model.Count
          Step = cval model.Step
          TimerOn = cval model.TimerOn  }

    let adelta (model: Model) (amodel: AdaptiveModel) =
        transact (fun () -> 
            if model.Count <> amodel.Count.Value then 
                amodel.Count.Value <- model.Count
            if model.Step <> amodel.Step.Value then 
                amodel.Step.Value <- model.Step
            if model.TimerOn <> amodel.TimerOn.Value then 
                amodel.TimerOn.Value <- model.TimerOn)

    type Msg = 
        | Increment 
        | Decrement 
        | Reset
        | SetStep of int
        | TimerToggled of bool
        | TimedTick

    type CmdMsg =
        | TickTimer

    let timerCmd () =
        async { do! Async.Sleep 200
                return TimedTick }
        |> Cmd.ofAsyncMsg

    let mapCmdMsgToCmd cmdMsg =
        match cmdMsg with
        | TickTimer -> timerCmd()

    let initialModel = { Count = 0; Step = 1; TimerOn = false }

    let init () = initialModel, []

    let update msg (model: Model) =
        match msg with
        | Increment -> { model with Count = model.Count + model.Step }, []
        | Decrement -> { model with Count = model.Count - model.Step }, []
        | Reset -> init ()
        | SetStep n -> { model with Step = n }, []
        | TimerToggled on -> { model with TimerOn = on }, (if on then [ TickTimer ] else [])
        | TimedTick -> if model.TimerOn then { model with Count = model.Count + model.Step }, [ TickTimer ] else model, [] 

    let view (model: AdaptiveModel) dispatch =  
        View.ContentPage(content = View.ListView (items = cs [ for i in 1 .. 10 -> View.TextCell(c (sprintf "Item %d" i)) ]))
        |> AVal.constant

        //View.ContentPage(
        //  content = View.Grid(rowdefs= c [for i in 1 .. 6 -> Star], 
        //                coldefs = c [for i in 1 .. 6 -> Star], 
        //                children = cs [ 
        //                    for i in 1 .. 6 do 
        //                        for j in 1 .. 6 -> 
        //                            let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0) 
        //                            View.BoxView(c color).Row(c (i-1)).Column(c (j-1)) ] ))
        //|> AVal.constant

        //View.ContentPage(
        //  content=View.StackLayout(padding = c (Thickness 30.0), verticalOptions = c LayoutOptions.Center,
        //    children = cs [

        //      View.Label(automationId = c "CountLabel", 
        //          text = (amodel.Count |> AVal.map (sprintf "%d")),
        //          horizontalOptions = c LayoutOptions.Center, 
        //          width = c 200.0, 
        //          horizontalTextAlignment = c TextAlignment.Center)

        //      View.Button(automationId = c "IncrementButton",
        //          text = c "Increment",
        //          command= c (fun () -> dispatch Increment))

        //      View.Button(automationId = c "DecrementButton",
        //          text = c "Decrement",
        //          command= c (fun () -> dispatch Decrement)) 

        //      View.Label(text = c "Timer")

        //      View.Switch(automationId = c "TimerSwitch",
        //        isToggled = amodel.TimerOn, 
        //        toggled = c (fun on -> dispatch (TimerToggled on.Value)))

        //      View.Slider(automationId = c "StepSlider", 
        //          minimumMaximum = c (0.0, 10.0), 
        //          value = (amodel.Step |> AVal.map double),
        //          valueChanged = c (fun args -> dispatch (SetStep (int (args.NewValue + 0.5)))))

        //      View.Label(automationId = c "StepSizeLabel",
        //          text= (amodel.Step |> AVal.map (sprintf "Step size: %d")),
        //          horizontalOptions = c LayoutOptions.Center)

        //      View.Button(text = c "Reset",
        //          horizontalOptions = c LayoutOptions.Center,
        //          command = c (fun () -> dispatch Reset),
        //          commandCanExecute = 
        //              ((amodel.Step, amodel.Count, amodel.TimerOn) |||> AVal.map3 (fun step count timerOn -> 
        //                  step <> initialModel.Step || 
        //                  count <> initialModel.Count || 
        //                  timerOn <> initialModel.TimerOn)))
        //    ]))
        //|> AVal.constant
            
        //View.ContentPage(
        //      View.Button(text = (amodel.Count |> AVal.map (sprintf "%d")),
        //          command= c (fun () -> dispatch Increment)
        //      )
        // )
        //|> AVal.constant

        //aval {
        //  let! count = model.Count
        //  return 
        //      if count <= 1 then 
        //            View.ContentPage(
        //              View.Button(text = (model.Count |> AVal.map (sprintf "%d")),
        //                  command= c (fun () -> dispatch Increment)
        //              )
        //           )
        //      else
        //            View.TabbedPage (cs [
        //                for i in 1 .. count do 
        //                    View.ContentPage(title = c (sprintf "Page %d" i), 
        //                        content =
        //                          View.Button(text = (model.Count |> AVal.map (sprintf "Page %d - %d" i)),
        //                              command= c (fun () -> dispatch Increment)
        //                          )
        //                     )
        //            ])
        //}

        //View.ContentPage(
        //  content=View.StackLayout(
        //    children = alist {
        //      let! count = model.Count
        //      for i in 0 .. count do 
        //          View.Button(text = c (sprintf "Button %d, count = %d" i count),
        //              command= c (fun () -> dispatch Increment)
        //          )
        //     })
        // )
        //|> AVal.constant
        
        //View.ContentPage(
        //  content=View.StackLayout(
        //    children = cs [
        //      //View.Label(text = (amodel.Count |> AVal.map (sprintf "%d")))
        //      View.Button(text = (amodel.Count |> AVal.map (sprintf "%d")),
        //          command= c (fun () -> dispatch Increment)
        //      )
        //    ])
        // )
        //|> AVal.constant

    let program = 
        Program.mkProgramWithCmdMsg init update ainit adelta view mapCmdMsgToCmd

type CounterApp () as app = 
    inherit Application ()

    let runner =
        App.program
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app

#if DEBUG
    // Run LiveUpdate using: 
    //    
    do runner.EnableLiveUpdate ()
#endif


#if SAVE_MODEL_WITH_JSON
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Debug.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Debug.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Debug.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Debug.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex ->
            runner.OnError ("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Debug.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()

#endif
