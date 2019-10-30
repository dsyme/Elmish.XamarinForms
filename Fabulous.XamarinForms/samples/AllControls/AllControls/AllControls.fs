// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace AllControls

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open FSharp.Data
open AllControls.Effects

type RootPageKind = 
    | Choice of bool
    | Tabbed1 
    | Tabbed2 
    | Tabbed3 
    | Navigation 
    | Carousel 
    | MasterDetail
    | InfiniteScrollList
    | Animations
    | WebCall
    | ScrollView
    | ShellView
    | CollectionView
    | CarouselView
    | Effects

type Model = 
  { RootPageKind: RootPageKind
    Count : int
    CountForSlider : int
    CountForActivityIndicator : int
    StepForSlider : int 
    MinimumForSlider : int
    MaximumForSlider : int
    StartDate : System.DateTime
    EndDate : System.DateTime
    EditorText : string
    EntryText : string
    Placeholder : string
    Password : string
    NumTaps : int 
    NumTaps2 : int 
    PickedColorIndex: int
    GridSize: int
    NewGridSize: double // used during pinch
    GridPortal: int * int 
    // For MasterDetailPage demo
    IsMasterPresented: bool 
    DetailPage: string
    // For NavigationPage demo
    PageStack: string option list
    // For InfiniteScroll page demo. It's not really an "infinite" scroll, just an unbounded set of data whose growth is prompted by the need formore of it in the UI
    InfiniteScrollMaxRequested: int
    SearchTerm: string
    CarouselCurrentPageIndex: int
    Tabbed1CurrentPageIndex: int
    // For WebCall page demo
    IsRunning: bool
    ReceivedData: bool
    WebCallData: string option
    // For ScrollView page demo
    ScrollPosition: float * float
    AnimatedScroll: AnimationKind
    IsScrollingWithFabulous: bool
    IsScrolling: bool
    }

type Msg = 
    | Increment 
    | Decrement 
    | Reset
    | IncrementForSlider
    | DecrementForSlider
    | ChangeMinimumMaximumForSlider1
    | ChangeMinimumMaximumForSlider2
    | IncrementForActivityIndicator
    | DecrementForActivityIndicator
    | SliderValueChanged of int
    | TextChanged of string * string
    | EditorEditCompleted of string
    | EntryEditCompleted of string
    | PasswordEntryEditCompleted of string
    | PlaceholderEntryEditCompleted of string
    | GridEditCompleted of int * int
    | StartDateSelected of DateTime 
    | EndDateSelected of DateTime 
    | PickerItemChanged of int
    | ListViewSelectedItemChanged of int option
    | ListViewGroupedSelectedItemChanged of (int * int) option
    | FrameTapped 
    | FrameTapped2 
    | UpdateNewGridSize of double * GestureStatus
    | UpdateGridPortal of int * int
    // For NavigationPage demo
    | GoHomePage
    | PopPage 
    | PagePopped 
    | ReplacePage of string
    | PushPage of string
    | SetRootPageKind of RootPageKind
    // For MasterDetail page demo
    | IsMasterPresentedChanged of bool
    | SetDetailPage of string
    // For InfiniteScroll page demo. It's not really an "infinite" scroll, just a growing set of "data"
    | SetInfiniteScrollMaxIndex of int
    | ExecuteSearch of string
    | ShowPopup
    | AnimationPoked
    | AnimationPoked2
    | AnimationPoked3
    | SetCarouselCurrentPage of int
    | SetTabbed1CurrentPage of int
    | ReceivedLowMemoryWarning
    // For WebCall page demo
    | ReceivedDataSuccess of string option
    | ReceivedDataFailure of string option
    | ReceiveData
    // For ScrollView page demo
    | ScrollFabulous of float * float * AnimationKind
    | ScrollXamarinForms of float * float * AnimationKind
    | Scrolled of float * float
    // For ShellView page demo
    //| ShowShell

[<AutoOpen>]
module MyExtension = 
    /// Test the extension API be making a 2nd wrapper for "Label":
    let TestLabelTextAttribKey = AttributeKey<_> "TestLabel_Text"
    let TestLabelFontFamilyAttribKey = AttributeKey<_> "TestLabel_FontFamily"

    type View with 

        static member TestLabel(?text: string, ?fontFamily: string, ?backgroundColor, ?rotation) = 

            // Get the attributes for the base element. The number is the expected number of attributes.
            // You can add additional base element attributes here if you like
            let attribCount = 0
            let attribCount = match text with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fontFamily with Some _ -> attribCount + 1 | None -> attribCount
            let attribs = ViewBuilders.BuildView(attribCount, ?backgroundColor = backgroundColor, ?rotation = rotation) 

            // Add our own attributes. They must have unique names.
            match text with None -> () | Some v -> attribs.Add(TestLabelTextAttribKey, v) 
            match fontFamily with None -> () | Some v -> attribs.Add(TestLabelFontFamilyAttribKey, v) 

            // The creation method
            let create () = new Xamarin.Forms.Label()

            // The incremental update method
            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: Xamarin.Forms.Label) = 
                ViewBuilders.UpdateView(prevOpt, source, target)
                source.UpdatePrimitive(prevOpt, target, TestLabelTextAttribKey, (fun target v -> target.Text <- v))
                source.UpdatePrimitive(prevOpt, target, TestLabelFontFamilyAttribKey, (fun target v -> target.FontFamily <- v))

            ViewElement.Create<Xamarin.Forms.Label>(create, update, attribs)

    // Test some adhoc functional abstractions
    type View with 
        static member ScrollingContentPage(title, children) =
            View.ContentPage(title=title, content=View.ScrollView(View.StackLayout(padding=Thickness 20.0, children=children) ), useSafeArea=true)

        static member NonScrollingContentPage(title, children, ?gestureRecognizers) =
            View.ContentPage(title=title, content=View.StackLayout(padding=Thickness 20.0, children=children, ?gestureRecognizers=gestureRecognizers), useSafeArea=true)


module App = 
    let init () = 
        { RootPageKind = Choice false
          Count = 0
          CountForSlider = 0
          StepForSlider = 3
          MinimumForSlider = 0
          MaximumForSlider = 10
          CountForActivityIndicator = 0
          PickedColorIndex = 0
          EditorText = "hic hac hoc"
          Placeholder = "cogito ergo sum"
          Password = "in omnibus errant"
          EntryText = "quod erat demonstrandum"
          GridSize = 6
          NewGridSize = 6.0
          GridPortal=(0, 0)
          StartDate=System.DateTime.Today
          EndDate=System.DateTime.Today.AddDays(1.0)
          IsMasterPresented=false
          NumTaps=0
          NumTaps2=0
          PageStack=[ Some "Home" ]
          DetailPage="A"
          InfiniteScrollMaxRequested = 10 
          SearchTerm = "nothing!"
          CarouselCurrentPageIndex = 0
          Tabbed1CurrentPageIndex = 0 
          IsRunning = false
          ReceivedData = false
          WebCallData = None
          ScrollPosition = 0.0, 0.0
          AnimatedScroll = Animated
          IsScrollingWithFabulous = false
          IsScrolling = false }, Cmd.none

    let getWebData =
        async {
            do! Async.SwitchToThreadPool()
            let! response = 
                Http.AsyncRequest(url="https://api.myjson.com/bins/1ecasc", httpMethod="GET", silentHttpErrors=true)
            let r = 
                match response.StatusCode with
                | 200 -> Msg.ReceivedDataSuccess (Some (response.Body |> string))
                | _ -> Msg.ReceivedDataFailure (Some "Failed to get data")
            return r
        } |> Cmd.ofAsyncMsg

    let animatedLabelRef = ViewRef<Label>()
    let scrollViewRef = ViewRef<ScrollView>()

    let scrollWithXFAsync (x: float, y: float, animated: AnimationKind) =
        async {
            match scrollViewRef.TryValue with
            | None -> return None
            | Some scrollView ->
                let animationEnabled =
                    match animated with
                    | Animated -> true
                    | NotAnimated -> false
                do! scrollView.ScrollToAsync(x, y, animationEnabled) |> Async.AwaitTask |> Async.Ignore
                return Some (Scrolled (x, y))
        } |> Cmd.ofAsyncMsgOption

    let update msg model =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }, Cmd.none
        | Decrement -> { model with Count = model.Count - 1}, Cmd.none
        | IncrementForSlider -> { model with CountForSlider = model.CountForSlider + model.StepForSlider }, Cmd.none
        | DecrementForSlider -> { model with CountForSlider = model.CountForSlider - model.StepForSlider }, Cmd.none
        | ChangeMinimumMaximumForSlider1 -> { model with MinimumForSlider = 0; MaximumForSlider = 10 }, Cmd.none
        | ChangeMinimumMaximumForSlider2 -> { model with MinimumForSlider = 15; MaximumForSlider = 20 }, Cmd.none
        | IncrementForActivityIndicator -> { model with CountForActivityIndicator = model.CountForActivityIndicator + 1 }, Cmd.none
        | DecrementForActivityIndicator -> { model with CountForActivityIndicator = model.CountForActivityIndicator - 1 }, Cmd.none
        | Reset -> init ()
        | SliderValueChanged n -> { model with StepForSlider = n }, Cmd.none
        | TextChanged _ -> model, Cmd.none
        | EditorEditCompleted t -> { model with EditorText = t }, Cmd.none
        | EntryEditCompleted t -> { model with EntryText = t }, Cmd.none
        | PasswordEntryEditCompleted t -> { model with Password = t }, Cmd.none
        | PlaceholderEntryEditCompleted t -> { model with Placeholder = t }, Cmd.none
        | StartDateSelected d -> { model with StartDate = d; EndDate = d + (model.EndDate - model.StartDate) }, Cmd.none
        | EndDateSelected d -> { model with EndDate = d }, Cmd.none
        | GridEditCompleted _ -> model, Cmd.none
        | ListViewSelectedItemChanged _ -> model, Cmd.none
        | ListViewGroupedSelectedItemChanged _ -> model, Cmd.none
        | PickerItemChanged i -> { model with PickedColorIndex = i }, Cmd.none
        | FrameTapped -> { model with NumTaps= model.NumTaps + 1 }, Cmd.none
        | FrameTapped2 -> { model with NumTaps2= model.NumTaps2 + 1 }, Cmd.none
        | UpdateNewGridSize (n, status) -> 
            match status with 
            | GestureStatus.Running -> { model with NewGridSize = model.NewGridSize * n}, Cmd.none
            | GestureStatus.Completed -> let sz = int (model.NewGridSize + 0.5) in { model with GridSize = sz; NewGridSize = float sz }, Cmd.none
            | GestureStatus.Canceled -> { model with NewGridSize = double model.GridSize }, Cmd.none
            | _ -> model, Cmd.none
        | UpdateGridPortal (x, y) -> { model with GridPortal = (x, y) }, Cmd.none
        // For NavigationPage
        | GoHomePage -> { model with PageStack = [ Some "Home" ] }, Cmd.none
        | PagePopped -> 
            if model.PageStack |> List.exists Option.isNone then 
               { model with PageStack = model.PageStack |> List.filter Option.isSome }, Cmd.none
            else
               { model with PageStack = (match model.PageStack with [] -> model.PageStack | _ :: t -> t) }, Cmd.none
        | PopPage -> 
               { model with PageStack = (match model.PageStack with [] -> model.PageStack | _ :: t -> None :: t) }, Cmd.none
        | PushPage page -> 
            { model with PageStack = Some page :: model.PageStack}, Cmd.none
        | ReplacePage page -> 
            { model with PageStack = (match model.PageStack with [] -> Some page :: model.PageStack | _ :: t -> Some page :: t) }, Cmd.none
        // For MasterDetail
        | IsMasterPresentedChanged b -> { model with IsMasterPresented = b }, Cmd.none
        | SetDetailPage s -> { model with DetailPage = s ; IsMasterPresented=false}, Cmd.none
        | SetInfiniteScrollMaxIndex n -> if n >= max n model.InfiniteScrollMaxRequested then { model with InfiniteScrollMaxRequested = (n + 10)}, Cmd.none else model, Cmd.none
        // For selection page
        | SetRootPageKind kind -> { model with RootPageKind = kind }, Cmd.none
        | ExecuteSearch search -> { model with SearchTerm = search }, Cmd.none
        // For pop-ups
        | ShowPopup ->
            Application.Current.MainPage.DisplayAlert("Clicked", "You clicked the button", "OK") |> ignore
            model, Cmd.none
        | AnimationPoked -> 
            match animatedLabelRef.TryValue with
            | Some _ ->
                animatedLabelRef.Value.Rotation <- 0.0
                animatedLabelRef.Value.RotateTo (360.0, 2000u) |> ignore
            | None -> ()
            model, Cmd.none
        | AnimationPoked2 -> 
            ViewExtensions.CancelAnimations (animatedLabelRef.Value)
            animatedLabelRef.Value.Rotation <- 0.0
            animatedLabelRef.Value.RotateTo (360.0, 2000u) |> ignore
            model, Cmd.none
        | AnimationPoked3 -> 
            ViewExtensions.CancelAnimations (animatedLabelRef.Value)
            animatedLabelRef.Value.Rotation <- 0.0
            animatedLabelRef.Value.RotateTo (360.0, 2000u) |> ignore
            model, Cmd.none
        | SetCarouselCurrentPage index ->
            { model with CarouselCurrentPageIndex = index }, Cmd.none
        | SetTabbed1CurrentPage index ->
            { model with Tabbed1CurrentPageIndex = index }, Cmd.none
        | ReceivedLowMemoryWarning ->
            Application.Current.MainPage.DisplayAlert("Low memory!", "Cleaning up data...", "OK") |> ignore
            { model with
                EditorText = ""
                EntryText = ""
                Placeholder = ""
                Password = ""
                SearchTerm = "" }, Cmd.none
        | ReceiveData ->
            {model with IsRunning=true}, getWebData
        | ReceivedDataFailure value ->
            {model with ReceivedData=false; IsRunning=false; WebCallData = value}, Cmd.none
        | ReceivedDataSuccess value ->
            {model with ReceivedData=true; IsRunning=false; WebCallData = value}, Cmd.none
        | ScrollFabulous (x, y, animated) ->
            { model with IsScrolling = true; IsScrollingWithFabulous = true; ScrollPosition = (x, y); AnimatedScroll = animated }, Cmd.none
        | ScrollXamarinForms (x, y, animated) ->
            { model with IsScrolling = true; IsScrollingWithFabulous = false; ScrollPosition = (x, y); AnimatedScroll = animated }, scrollWithXFAsync (x, y, animated)
        | Scrolled (x, y) ->
            { model with ScrollPosition = (x, y); IsScrolling = false; IsScrollingWithFabulous = false }, Cmd.none 

    let pickerItems = 
        [ ("Aqua", Color.Aqua); ("Black", Color.Black);
           ("Blue", Color.Blue); ("Fucshia", Color.Fuchsia);
           ("Gray", Color.Gray); ("Green", Color.Green);
           ("Lime", Color.Lime); ("Maroon", Color.Maroon);
           ("Navy", Color.Navy); ("Olive", Color.Olive);
           ("Purple", Color.Purple); ("Red", Color.Red);
           ("Silver", Color.Silver); ("Teal", Color.Teal);
           ("White", Color.White); ("Yellow", Color.Yellow ) ]

    let view (model: Model) dispatch =

        let MainPageButton = 
            View.Button(text="Main page", 
                        command=(fun () -> dispatch (SetRootPageKind (Choice false))), 
                        horizontalOptions=LayoutOptions.CenterAndExpand)

        match model.RootPageKind with 
        | Choice showAbout -> 
            View.NavigationPage(pages=
                [ yield 
                    View.ContentPage(useSafeArea=true,
                        padding = Thickness (10.0, 20.0, 10.0, 5.0), 
                        content = View.ScrollView(
                            content = View.StackLayout(
                                children = [ 
                                     View.Button(text = "TabbedPage #1 (various controls)", command=(fun () -> dispatch (SetRootPageKind Tabbed1)))
                                     View.Button(text = "TabbedPage #2 (various controls)", command=(fun () -> dispatch (SetRootPageKind Tabbed2)))
                                     View.Button(text = "TabbedPage #3 (various controls)", command=(fun () -> dispatch (SetRootPageKind Tabbed3)))
                                     View.Button(text = "CarouselPage (various controls)", command=(fun () -> dispatch (SetRootPageKind Carousel)))
                                     View.Button(text = "NavigationPage with push/pop", command=(fun () -> dispatch (SetRootPageKind Navigation)))
                                     View.Button(text = "MasterDetail Page", command=(fun () -> dispatch (SetRootPageKind MasterDetail)))
                                     View.Button(text = "Infinite scrolling ListView", command=(fun () -> dispatch (SetRootPageKind InfiniteScrollList)))
                                     View.Button(text = "Animations", command=(fun () -> dispatch (SetRootPageKind Animations)))
                                     View.Button(text = "Pop-up", command=(fun () -> dispatch ShowPopup))
                                     View.Button(text = "WebRequest", command=(fun () -> dispatch (SetRootPageKind WebCall)))
                                     View.Button(text = "ScrollView", command=(fun () -> dispatch (SetRootPageKind ScrollView)))
                                     View.Button(text = "Shell", command=(fun () -> dispatch (SetRootPageKind ShellView)))
                                     View.Button(text = "CollectionView", command=(fun () -> dispatch (SetRootPageKind CollectionView)))
                                     View.Button(text = "CarouselView", command=(fun () -> dispatch (SetRootPageKind CarouselView)))
                                     View.Button(text = "Effects", command=(fun () -> dispatch (SetRootPageKind Effects)))
                                ])))
                     .ToolbarItems([View.ToolbarItem(text="about", command=(fun () -> dispatch (SetRootPageKind (Choice true))))] )
                     .TitleView(View.StackLayout(orientation=StackOrientation.Horizontal, children=[
                             View.Label(text="fabulous", verticalOptions=LayoutOptions.Center)
                             View.Label(text="rootpage", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.CenterAndExpand)
                         ]
                     ))

                  if showAbout then 
                    yield 
                        View.ContentPage(title="About", useSafeArea=true, 
                            padding = Thickness (10.0, 20.0, 10.0, 5.0), 
                            content= View.StackLayout(
                               children=[ 
                                   View.TestLabel(text = "Fabulous, version " + string (typeof<ViewElement>.Assembly.GetName().Version))
                                   View.Label(text = "Now with CSS styling", styleClasses = [ "cssCallout" ])
                                   View.Button(text = "Continue", command=(fun () -> dispatch (SetRootPageKind (Choice false)) ))
                               ]))
                ])

        | Carousel -> 
           View.CarouselPage(
                    useSafeArea=true,
                    currentPageChanged=(fun index -> 
                        match index with
                        | None -> printfn "No page selected"
                        | Some ind ->
                            printfn "Page changed : %i" ind
                            dispatch (SetCarouselCurrentPage ind)
                    ),
                    currentPage=model.CarouselCurrentPageIndex,
                    children=
             [ dependsOn model.Count (fun model count -> 
                   View.ScrollingContentPage("Button", 
                       [ View.Label(text="Label:")
                         View.Label(text= sprintf "%d" count, horizontalOptions=LayoutOptions.CenterAndExpand)
                 
                         View.Label(text="Button:")
                         View.Button(text="Increment", command=(fun () -> dispatch Increment), horizontalOptions=LayoutOptions.CenterAndExpand)
                 
                         View.Label(text="Button:")
                         View.Button(text="Decrement", command=(fun () -> dispatch Decrement), horizontalOptions=LayoutOptions.CenterAndExpand)

                         View.Button(text="Go to grid", cornerRadius=5, command=(fun () -> dispatch (SetCarouselCurrentPage 6)), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)
                         
                         MainPageButton
                      ]))

               dependsOn model.CountForActivityIndicator (fun model count -> 
                   View.ScrollingContentPage("ActivityIndicator", 
                       [View.Label(text="Label:")
                        View.Label(text= sprintf "%d" count, horizontalOptions=LayoutOptions.CenterAndExpand)
 
                        View.Label(text="ActivityIndicator (when count > 0):")
                        View.ActivityIndicator(isRunning=(count > 0), horizontalOptions=LayoutOptions.CenterAndExpand)
                  
                        View.Label(text="Button:")
                        View.Button(text="Increment", command=(fun () -> dispatch IncrementForActivityIndicator), horizontalOptions=LayoutOptions.CenterAndExpand)

                        View.Label(text="Button:")
                        View.Button(text="Decrement", command=(fun () -> dispatch DecrementForActivityIndicator), horizontalOptions=LayoutOptions.CenterAndExpand)
                        MainPageButton
                      ]))

               dependsOn (model.StartDate, model.EndDate) (fun model (startDate, endDate) -> 
                   View.ScrollingContentPage("DatePicker", 
                       [ View.Label(text="DatePicker (start):")
                         View.DatePicker(minimumDate= System.DateTime.Today, maximumDate=DateTime.Today + TimeSpan.FromDays(365.0), 
                             date=startDate, 
                             dateSelected=(fun args -> dispatch (StartDateSelected args.NewDate)), 
                             horizontalOptions=LayoutOptions.CenterAndExpand)

                         View.Label(text="DatePicker (end):")
                         View.DatePicker(minimumDate= startDate, maximumDate=startDate + TimeSpan.FromDays(365.0), 
                             date=endDate, 
                             dateSelected=(fun args -> dispatch (EndDateSelected args.NewDate)), 
                             horizontalOptions=LayoutOptions.CenterAndExpand)
                         MainPageButton
                       ]))

               dependsOn model.EditorText (fun model editorText -> 
                   View.ScrollingContentPage("Editor", 
                       [ View.Label(text="Editor:")
                         View.Editor(text= editorText, horizontalOptions=LayoutOptions.FillAndExpand, 
                            textChanged=(fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                            completed=(fun text -> dispatch (EditorEditCompleted text)))
                         MainPageButton
                       ]))

               dependsOn (model.EntryText, model.Password, model.Placeholder) (fun model (entryText, password, placeholder) -> 
                   View.ScrollingContentPage("Entry", 
                       [ View.Label(text="Entry:")
                         View.Entry(text= entryText, horizontalOptions=LayoutOptions.CenterAndExpand, 
                             textChanged=(fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                             completed=(fun text -> dispatch (EntryEditCompleted text)))

                         View.Label(text="Entry (password):")
                         View.Entry(text= password, isPassword=true, horizontalOptions=LayoutOptions.CenterAndExpand, 
                             textChanged=(fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                             completed=(fun text -> dispatch (PasswordEntryEditCompleted text)))

                         View.Label(text="Entry (placeholder):")
                         View.Entry(placeholder= placeholder, horizontalOptions=LayoutOptions.CenterAndExpand, 
                             textChanged=(fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                             completed=(fun text -> dispatch (PlaceholderEntryEditCompleted text)))

                         MainPageButton
                       ]) )

               dependsOn (model.NumTaps, model.NumTaps2) (fun model (numTaps, numTaps2) -> 
                   View.ScrollingContentPage("Frame", 
                       [ View.Label(text="Frame (hasShadow=true):")
                         View.Frame(hasShadow=true, backgroundColor=Color.AliceBlue, horizontalOptions=LayoutOptions.CenterAndExpand)

                         View.Label(text="Frame (tap once gesture):")
                         View.Frame(hasShadow=true, 
                             backgroundColor=snd (pickerItems.[numTaps % pickerItems.Length]), 
                             horizontalOptions=LayoutOptions.CenterAndExpand, 
                             gestureRecognizers=[ View.TapGestureRecognizer(command=(fun () -> dispatch FrameTapped)) ] )

                         View.Label(text="Frame (tap twice gesture):")
                         View.Frame(hasShadow=true, 
                             backgroundColor=snd (pickerItems.[numTaps2 % pickerItems.Length]), 
                             horizontalOptions=LayoutOptions.CenterAndExpand, 
                             gestureRecognizers=[ View.TapGestureRecognizer(numberOfTapsRequired=2, command=(fun () -> dispatch FrameTapped2)) ] )
                 
                         MainPageButton
                       ]))

               dependsOn () (fun model () -> 
                   View.NonScrollingContentPage("Grid", 
                       [ View.Label(text=sprintf "Grid (6x6, auto):")
                         View.Grid(rowdefs= [for i in 1 .. 6 -> Auto], 
                             coldefs=[for i in 1 .. 6 -> Auto], 
                             children = 
                                 [ for i in 1 .. 6 do 
                                      for j in 1 .. 6 -> 
                                         let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0)
                                         View.BoxView(color).Row(i-1).Column(j-1) ] )
                         MainPageButton
                       ]))
           ])

        | Tabbed1 ->
           View.TabbedPage(
                    useSafeArea=true,
                    currentPageChanged=(fun index ->
                        match index with
                        | None -> printfn "No tab selected"
                        | Some ind ->
                            printfn "Tab changed : %i" ind
                            dispatch (SetTabbed1CurrentPage ind)
                    ),
                    currentPage=model.Tabbed1CurrentPageIndex,
                    children=
             [
               dependsOn (model.MinimumForSlider, model.MaximumForSlider, model.CountForSlider, model.StepForSlider) (fun model (minimum, maximum, count, step) -> 
                  View.ScrollingContentPage("Slider", 
                     [ View.Label(text="Label:")
                       View.Label(text= sprintf "%d" count, horizontalOptions=LayoutOptions.CenterAndExpand)

                       View.Label(text="Button:")
                       View.Button(text="Increment", command=(fun () -> dispatch IncrementForSlider), horizontalOptions=LayoutOptions.CenterAndExpand)
                 
                       View.Label(text="Button:")
                       View.Button(text="Decrement", command=(fun () -> dispatch DecrementForSlider), horizontalOptions=LayoutOptions.CenterAndExpand)

                       View.Label(text="Button:")
                       View.Button(text="Set Minimum = 0 / Maximum = 10", command=(fun () -> dispatch ChangeMinimumMaximumForSlider1), horizontalOptions=LayoutOptions.CenterAndExpand)
                       View.Button(text="Set Minimum = 15 / Maximum = 20", command=(fun () -> dispatch ChangeMinimumMaximumForSlider2), horizontalOptions=LayoutOptions.CenterAndExpand)

                       View.Label(text=sprintf "Slider: (Minimum %d, Maximum %d, Value %d)" minimum maximum step)
                       View.Slider(minimumMaximum=(float minimum, float maximum), 
                           value=double step, 
                           valueChanged=(fun args -> dispatch (SliderValueChanged (int (args.NewValue + 0.5)))), 
                           horizontalOptions=LayoutOptions.Fill) 

                       View.Button(text="Go to Image", 
                            command=(fun () -> dispatch (SetTabbed1CurrentPage 4)), 
                            horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)
                       
                       MainPageButton
                    ]))

               dependsOn () (fun model () -> 
                   View.NonScrollingContentPage("Grid", 
                       [ View.Label(text=sprintf "Grid (6x6, *):")
                         View.Grid(rowdefs= [for i in 1 .. 6 -> Star], coldefs=[for i in 1 .. 6 -> Star], 
                            children = [ 
                                for i in 1 .. 6 do 
                                    for j in 1 .. 6 -> 
                                        let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0) 
                                        View.BoxView(color).Row(i-1).Column(j-1) ] )
                         MainPageButton
                        ]))

               dependsOn (model.GridSize, model.NewGridSize) (fun model (gridSize, newGridSize) -> 
                  View.NonScrollingContentPage("Grid+Pinch", 
                      [ View.Label(text=sprintf "Grid (nxn, pinch, size = %f):" newGridSize)
                        // The Grid doesn't change during the pinch...
                        dependsOn gridSize (fun _ _ -> 
                          View.Grid(rowdefs= [for i in 1 .. gridSize -> Star], coldefs=[for i in 1 .. gridSize -> Star], 
                              children = [ 
                                  for i in 1 .. gridSize do 
                                      for j in 1 .. gridSize -> 
                                         let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0) 
                                         View.BoxView(color).Row(i-1).Column(j-1) ]))
                        MainPageButton
                      ], 
                      gestureRecognizers=[ View.PinchGestureRecognizer(pinchUpdated=(fun pinchArgs -> 
                                              dispatch (UpdateNewGridSize (pinchArgs.Scale, pinchArgs.Status)))) ] ))

               dependsOn model.GridPortal (fun model gridPortal -> 
                  let dx, dy = gridPortal
                  View.NonScrollingContentPage("Grid+Pan", 
                      children=
                          [ View.Label(text= sprintf "Grid (nxn, auto, edit entries, 1-touch pan, (%d, %d):" dx dy)
                            View.Grid(rowdefs= [for row in 1 .. 6 -> Star], coldefs=[for col in 1 .. 6 -> Star], 
                               children = [ for row in 1 .. 6 do 
                                               for col in 1 .. 6 ->
                                                  let item = View.Label(text=sprintf "(%d, %d)" (col+dx) (row+dy), backgroundColor=Color.White, textColor=Color.Black) 
                                                  item.Row(row-1).Column(col-1) ])
                            MainPageButton
                      ], 
                      gestureRecognizers=[ View.PanGestureRecognizer(touchPoints=1, panUpdated=(fun panArgs -> 
                                              if panArgs.StatusType = GestureStatus.Running then 
                                                  dispatch (UpdateGridPortal (dx - int (panArgs.TotalX/10.0), dy - int (panArgs.TotalY/10.0))))) ] ))

               dependsOn () (fun model () -> 
                 View.NonScrollingContentPage("Image", 
                     [ View.Label(text="Image (URL):")
                       View.Image(source=Path "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg", 
                           horizontalOptions=LayoutOptions.FillAndExpand,
                           verticalOptions=LayoutOptions.FillAndExpand)
                       View.Label(text="Image (Embedded):", margin = Thickness (0., 20., 0., 0.))
                       View.Image(source=Source (ImageSource.FromResource("AllControls.Baboon_Serengeti.jpg", typeof<RootPageKind>.Assembly)), 
                              horizontalOptions=LayoutOptions.FillAndExpand,
                              verticalOptions=LayoutOptions.FillAndExpand) 
                       MainPageButton ]))
             ])

        | Tabbed2 ->
           View.TabbedPage(useSafeArea=true, children=
             [
               dependsOn (model.PickedColorIndex) (fun model (pickedColorIndex) -> 
                  View.ScrollingContentPage("Picker", 
                     [ View.Label(text="Picker:")
                       View.Picker(title="Choose Color:", textColor=snd pickerItems.[pickedColorIndex], selectedIndex=pickedColorIndex, items=(List.map fst pickerItems), horizontalOptions=LayoutOptions.CenterAndExpand, selectedIndexChanged=(fun (i, item) -> dispatch (PickerItemChanged i)))
                       MainPageButton
                     ]))
                      
               dependsOn () (fun model () -> 
                  View.ScrollingContentPage("ListView", 
                     [ MainPageButton
                       View.Label(text="ListView:")
                       View.ListView(
                           items = [ 
                               for i in 0 .. 10 do 
                                   yield View.TextCell "Ionide"
                                   yield View.ViewCell(
                                        view = View.Label(
                                            formattedText = View.FormattedString([
                                                View.Span(text="Visual ", backgroundColor = Color.Green)
                                                View.Span(text="Studio ", fontSize = FontSize 10.)
                                            ])
                                        )
                                   ) 
                                   yield View.TextCell "Emacs"
                                   yield View.ViewCell(
                                        view = View.Label(
                                            formattedText = View.FormattedString([
                                                View.Span(text="Visual ", fontAttributes=FontAttributes.Bold)
                                                View.Span(text="Studio ", fontAttributes=FontAttributes.Italic)
                                                View.Span(text="Code", foregroundColor = Color.Blue)
                                            ])
                                        )
                                   )
                                   yield View.TextCell "Rider" ], 
                           horizontalOptions=LayoutOptions.CenterAndExpand, 
                           itemSelected=(fun idx -> dispatch (ListViewSelectedItemChanged idx)))
                ]))

                      
               dependsOn (model.SearchTerm) (fun model (searchTerm) -> 
                  View.ScrollingContentPage("SearchBar", 
                     [ View.Label(text="SearchBar:")
                       View.SearchBar(
                            placeholder = "Enter search term",
                            searchCommand = (fun searchBarText -> dispatch (ExecuteSearch searchBarText)),
                            searchCommandCanExecute = true) 
                       View.Label(text="You searched for " + searchTerm) 
                       MainPageButton ]))

               dependsOn () (fun model () -> 
                   View.NonScrollingContentPage("ListViewGrouped", 
                       [ View.Label(text="ListView (grouped):")
                         View.ListViewGrouped(
                             showJumpList=true,
                             items= 
                                [ 
                                    "B", View.TextCell "B", [ View.TextCell "Baboon"; View.TextCell "Blue Monkey" ]
                                    "C", View.TextCell "C", [ View.TextCell "Capuchin Monkey"; View.TextCell "Common Marmoset" ]
                                    "G", View.TextCell "G", [ View.TextCell "Gibbon"; View.TextCell "Golden Lion Tamarin" ]
                                    "H", View.TextCell "H", [ View.TextCell "Howler Monkey" ]
                                    "J", View.TextCell "J", [ View.TextCell "Japanese Macaque" ]
                                    "M", View.TextCell "M", [ View.TextCell "Mandrill" ]
                                    "P", View.TextCell "P", [ View.TextCell "Proboscis Monkey"; View.TextCell "Pygmy Marmoset" ]
                                    "R", View.TextCell "R", [ View.TextCell "Rhesus Macaque" ]
                                    "S", View.TextCell "S", [ View.TextCell "Spider Monkey"; View.TextCell "Squirrel Monkey" ]
                                    "V", View.TextCell "V", [ View.TextCell "Vervet Monkey" ]
                                ], 
                             horizontalOptions=LayoutOptions.CenterAndExpand,
                             itemSelected=(fun idx -> dispatch (ListViewGroupedSelectedItemChanged idx)))
                         MainPageButton
                   ]))

             ])
        | Tabbed3 ->
           View.TabbedPage(useSafeArea=true, 
            children=
             [ 
               dependsOn model.Count (fun model count -> 
                   View.ContentPage(title="FlexLayout", useSafeArea=true,
                       padding = Thickness (10.0, 20.0, 10.0, 5.0), 
                       content= 
                           View.FlexLayout(
                            direction = FlexDirection.Column,
                            children = [
                                View.ScrollView(orientation=ScrollOrientation.Both,
                                  content = View.FlexLayout(
                                      children = [
                                          View.Frame(height=480.0, width=300.0, 
                                              content = View.FlexLayout( direction=FlexDirection.Column,
                                                  children = [ 
                                                      View.Label(text="Seated Monkey", margin=Thickness (0.0, 8.0), fontSize=Named NamedSize.Large, textColor=Color.Blue)
                                                      View.Label(text="This monkey is laid back and relaxed, and likes to watch the world go by.", margin=Thickness (0.0, 4.0), textColor=Color.Black)
                                                      View.Label(text="  • Often smiles mysteriously", margin=Thickness (0.0, 4.0), textColor=Color.Black)
                                                      View.Label(text="  • Sleeps sitting up", margin=Thickness (0.0, 4.0), textColor=Color.Black)
                                                      View.Image(height=240.0, 
                                                          width=160.0, 
                                                          source=Path "https://upload.wikimedia.org/wikipedia/commons/thumb/6/66/Vervet_monkey_Krugersdorp_game_reserve_%285657678441%29.jpg/160px-Vervet_monkey_Krugersdorp_game_reserve_%285657678441%29.jpg"
                                                      ).Order(-1).AlignSelf(FlexAlignSelf.Center)
                                                      View.Label(margin=Thickness (0.0, 4.0)).Grow(1.0)
                                                      View.Button(text="Learn More", fontSize=Named NamedSize.Large, textColor=Color.White, backgroundColor=Color.Green, cornerRadius=20) ]),
                                              backgroundColor=Color.LightYellow,
                                              borderColor=Color.Blue,
                                              margin=Thickness 10.0,
                                              cornerRadius=15.0)
                                          View.Frame(height=480.0, width=300.0, 
                                              content = View.FlexLayout( direction=FlexDirection.Column,
                                                  children = [ 
                                                      View.Label(text="Banana Monkey", margin=Thickness (0.0, 8.0), fontSize=Named NamedSize.Large, textColor=Color.Blue)
                                                      View.Label(text="Watch this monkey eat a giant banana.", margin=Thickness (0.0, 4.0), textColor=Color.Black)
                                                      View.Label(text="  • More fun than a barrel of monkeys", margin=Thickness (0.0, 4.0), textColor=Color.Black)
                                                      View.Label(text="  • Banana not included", margin=Thickness (0.0, 4.0), textColor=Color.Black)
                                                      View.Image(height=213.0, 
                                                          width=320.0, 
                                                          source=Path "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c1/Crab_eating_macaque_in_Ubud_with_banana.JPG/320px-Crab_eating_macaque_in_Ubud_with_banana.JPG"
                                                      ).Order(-1).AlignSelf(FlexAlignSelf.Center)
                                                      View.Label(margin=Thickness (0.0, 4.0)).Grow(1.0)
                                                      View.Button(text="Learn More", fontSize=Named NamedSize.Large, textColor=Color.White, backgroundColor=Color.Green, cornerRadius=20) ]),
                                              backgroundColor=Color.LightYellow,
                                              borderColor=Color.Blue,
                                              margin=Thickness 10.0,
                                              cornerRadius=15.0)
                                          
                                      ] ))
                                MainPageButton
                            ])) )

               dependsOn () (fun model () -> 
                View.ScrollingContentPage("TableView", 
                 [View.Label(text="TableView:")
                  View.TableView(
                    horizontalOptions = LayoutOptions.StartAndExpand,
                    root = View.TableRoot(
                        items = [
                            View.TableSection(
                                title = "Videos",
                                items = [
                                    View.SwitchCell(on=true, text="Luca 2008", onChanged=(fun args -> ()) ) 
                                    View.SwitchCell(on=true, text="Don 2010", onChanged=(fun args -> ()) )
                                ]
                            )
                            View.TableSection(
                                title = "Books",
                                items = [
                                    View.SwitchCell(on=true, text="Expert F#", onChanged=(fun args -> ()) ) 
                                    View.SwitchCell(on=false, text="Programming F#", onChanged=(fun args -> ()) )
                                ]
                            )
                            View.TableSection(
                                title = "Contact",
                                items = [
                                    View.EntryCell(label="Email", placeholder="foo@bar.com", completed=(fun args -> ()) )
                                    View.EntryCell(label="Phone", placeholder="+44 87654321", completed=(fun args -> ()) )
                                ]
                            )
                        ]
                    )
                  )
                  MainPageButton
                    ]))

               dependsOn model.Count (fun model count -> 
                 View.ContentPage(title="RelativeLayout", 
                  padding = Thickness (10.0, 20.0, 10.0, 5.0), 
                  content= View.RelativeLayout(
                      children=[ 
                          View.Label(text = "RelativeLayout Example", textColor = Color.Red)
                                .XConstraint(Constraint.RelativeToParent(fun parent -> 0.0))
                          View.Label(text = "Positioned relative to my parent", textColor = Color.Red)
                                .XConstraint(Constraint.RelativeToParent(fun parent -> parent.Width / 3.0))
                                .YConstraint(Constraint.RelativeToParent(fun parent -> parent.Height / 2.0))
                          MainPageButton
                                .XConstraint(Constraint.RelativeToParent(fun parent -> parent.Width / 2.0))
                      ])))


               dependsOn model.Count (fun model count -> 
                   View.ContentPage(title="AbsoluteLayout", useSafeArea=true,
                       padding = Thickness (10.0, 20.0, 10.0, 5.0), 
                       content= View.StackLayout(
                           children=[ 
                               View.Label(text = "AbsoluteLayout Demo", fontSize = Named NamedSize.Large, horizontalOptions = LayoutOptions.Center)
                               View.AbsoluteLayout(backgroundColor = Color.Blue.WithLuminosity(0.9), 
                                   verticalOptions = LayoutOptions.FillAndExpand, 
                                   children = [
                                      View.Label(text = "Top Left", textColor = Color.Black)
                                          .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                                          .LayoutBounds(Rectangle(0.0, 0.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize))
                                      View.Label(text = "Centered", textColor = Color.Black)
                                          .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                                          .LayoutBounds(Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize))
                                      View.Label(text = "Bottom Right", textColor = Color.Black)
                                          .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                                          .LayoutBounds(Rectangle(1.0, 1.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize)) ])
                               MainPageButton
                            ])))

                ])
         
        | Navigation -> 

         // NavigationPage example
           dependsOn model.PageStack (fun model pageStack -> 
              View.NavigationPage(pages=
                      [ for page in List.rev pageStack do
                          match page with 
                          | Some "Home" -> 
                              yield 
                                  View.ContentPage(useSafeArea=true,
                                    content=View.StackLayout(
                                     children=
                                       [ View.Label(text="Home Page", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                         View.Button(text="Push Page A", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "A")))
                                         View.Button(text="Push Page B", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "B")))
                
                                         View.Button(text="Main page", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)
                                        ]) ).HasNavigationBar(true).HasBackButton(false)
                          | Some "A" -> 
                              yield 
                                View.ContentPage(useSafeArea=true,
                                    content=
                                     View.StackLayout(
                                      children=
                                       [View.Label(text="Page A", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center)
                                        View.Button(text="Page B", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "B")))
                                        View.Button(text="Page C", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "C")))
                                        View.Button(text="Replace by Page B", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (ReplacePage "B")))
                                        View.Button(text="Replace by Page C", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (ReplacePage "C")))
                                        View.Button(text="Back", verticalOptions=LayoutOptions.Center, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch PopPage ))
                                        ]) ).HasNavigationBar(true).HasBackButton(true)
                          | Some "B" -> 
                              yield 
                                View.ContentPage(useSafeArea=true,
                                    content=View.StackLayout(
                                         children=
                                             [View.Label(text="Page B", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                              View.Label(text="(nb. no back button in navbar)", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                              View.Button(text="Page A", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "A")))
                                              View.Button(text="Page C", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "C")))
                                              View.Button(text="Back", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch PopPage ))
                                             ]) ).HasNavigationBar(true).HasBackButton(false)
                          | Some "C" -> 
                              yield 
                                View.ContentPage(useSafeArea=true,
                                    content=View.StackLayout(
                                      children=
                                       [View.Label(text="Page C", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                        View.Label(text="(nb. no navbar)", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center)
                                        View.Button(text="Page A", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "A")))
                                        View.Button(text="Page B", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch (PushPage "B")))
                                        View.Button(text="Back", verticalOptions=LayoutOptions.CenterAndExpand, horizontalOptions=LayoutOptions.Center, command=(fun () -> dispatch PopPage ))
                                        MainPageButton
                                        ]) ).HasNavigationBar(false).HasBackButton(false)

                          | _ -> 
                               ()  ], 
                     popped=(fun args -> dispatch PagePopped) , 
                     poppedToRoot=(fun args -> dispatch GoHomePage)  ))

        | MasterDetail -> 
         // MasterDetail where the Master acts as a hamburger-style menu
          dependsOn (model.DetailPage, model.IsMasterPresented) (fun model (detailPage, isMasterPresented) -> 
            View.MasterDetailPage(
               masterBehavior=MasterBehavior.Popover, 
               isPresented=isMasterPresented, 
               isPresentedChanged=(fun b -> dispatch (IsMasterPresentedChanged b)), 
               master = 
                 View.ContentPage(useSafeArea=true, title="Master", 
                  content = 
                    View.StackLayout(backgroundColor=Color.Gray, 
                      children=[ View.Button(text="Detail A", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetDetailPage "A")))
                                 View.Button(text="Detail B", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetDetailPage "B")))
                                 View.Button(text="Main page", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End) 
                               ]) ), 
               detail = 
                 View.NavigationPage( 
                   pages=[
                     View.ContentPage(title="Detail", useSafeArea=true,
                      content = 
                        View.StackLayout(backgroundColor=Color.Gray, 
                          children=[ View.Label(text="Detail " + detailPage, textColor=Color.White, backgroundColor=Color.Navy)
                                     View.Button(text="Main page", textColor=Color.White, backgroundColor=Color.Navy, command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)  ]) 
                          ).HasNavigationBar(true).HasBackButton(true) ], 
                   poppedToRoot=(fun args -> dispatch (IsMasterPresentedChanged true) ) ) ) )

         | InfiniteScrollList -> 
              dependsOn (model.InfiniteScrollMaxRequested ) (fun model max -> 
               View.ScrollingContentPage("ListView (InfiniteScrollList)", 
                [MainPageButton
                 View.Label(text="InfiniteScrollList:")
                 View.ListView(items = [ for i in 1 .. max do 
                                           yield dependsOn i (fun _ i -> View.TextCell("Item " + string i, textColor=(if i % 3 = 0 then Color.CadetBlue else Color.LightCyan))) ], 
                               horizontalOptions=LayoutOptions.CenterAndExpand, 
                               // Every time the last element is needed, grow the set of data to be at least 10 bigger then that index 
                               itemAppearing=(fun idx -> if idx >= max - 2 then dispatch (SetInfiniteScrollMaxIndex (idx + 10) ) )  )
                 ] ))

         | Animations -> 
               View.ScrollingContentPage("Animations", 
                  [ View.Label(text="Rotate", created=(fun l -> l.RotateTo (360.0, 2000u) |> ignore)) 
                    View.Label(text="Hello!", ref=animatedLabelRef) 
                    View.Button(text="Poke", command=(fun () -> dispatch AnimationPoked))
                    View.Button(text="Poke2", command=(fun () -> dispatch AnimationPoked2))
                    View.Button(text="Poke3", command=(fun () -> dispatch AnimationPoked3))
                    View.Button(text="Main page", cornerRadius=5, command=(fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions=LayoutOptions.CenterAndExpand, verticalOptions=LayoutOptions.End)
                    ] )
         | WebCall ->
            let data = match model.WebCallData with
                        | Some v -> v
                        | None -> ""

            View.ContentPage(
                content = View.StackLayout(
                    children = [
                        View.Button(text="Get Data", command=(fun () -> dispatch ReceiveData))
                        View.ActivityIndicator(isRunning=model.IsRunning)
                        View.Label(text=data)
                        MainPageButton
                    ]
            ))
         | ScrollView ->
            let scrollToValue (x, y) animated =
                (x, y, animated)

            View.ContentPage(
                content = View.StackLayout(
                    children = [
                        MainPageButton
                        View.Label(text = (sprintf "Is scrolling: %b" model.IsScrolling))
                        View.Button(text = "Scroll to top", command=(fun() -> dispatch (ScrollFabulous (0.0, 0.0, Animated))))
                        View.ScrollView(
                            ref = scrollViewRef,
                            ?scrollTo= (if model.IsScrollingWithFabulous then Some (scrollToValue model.ScrollPosition model.AnimatedScroll) else None),
                            scrolled=(fun args -> dispatch (Scrolled (args.ScrollX, args.ScrollY))),
                            content = View.StackLayout(
                                children = [
                                    yield View.Button(text="Scroll animated with Fabulous", command=(fun() -> dispatch (ScrollFabulous (0.0, 750.0, Animated))))
                                    yield View.Button(text="Scroll not animated with Fabulous", command=(fun() -> dispatch (ScrollFabulous (0.0, 750.0, NotAnimated))))
                                    yield View.Button(text="Scroll animated with Xamarin.Forms", command=(fun() -> dispatch (ScrollXamarinForms (0.0, 750.0, Animated))))
                                    yield View.Button(text="Scroll not animated with Xamarin.Forms", command=(fun() -> dispatch (ScrollXamarinForms (0.0, 750.0, NotAnimated))))

                                    for i = 0 to 75 do
                                        yield View.Label(text=(sprintf "Item %i" i))
                                ]
                            )
                        )
                    ]
                ) 
            )
         | ShellView ->
            
            match Device.RuntimePlatform with
                | Device.iOS | Device.Android -> 
                    
                    View.Shell( title = "TitleShell",
                        items = [
                            View.ShellItem(
                                items = [
                                    View.ShellSection(items = [
                                        View.ShellContent(content=View.ContentPage(content=MainPageButton, title="ContentpageTitle"))         
                                    ])
                                ])
                        ])
                | _ -> View.ContentPage(content = View.Label(text="Your Platform does not support Shell"))

         | CollectionView ->
            match Device.RuntimePlatform with
                | Device.iOS | Device.Android -> 
                    View.ContentPage(content=View.StackLayout(children = [
                            MainPageButton
                            // use Collectionview instead of listview 
                            View.CollectionView(items= [
                                View.Label(text="Person 1") 
                                View.Label(text="Person2")
                                View.Label(text="Person3")
                                View.Label(text="Person4")
                                View.Label(text="Person5")
                                View.Label(text="Person6")
                                View.Label(text="Person7")
                                View.Label(text="Person8")
                                View.Label(text="Person9")
                                View.Label(text="Person11")
                                View.Label(text="Person12")
                                View.Label(text="Person13")
                                View.Label(text="Person14")] )
                        ]
                    ))

                | _ -> View.ContentPage(content = View.StackLayout( children = [
                                            MainPageButton
                                            View.Label(text="Your Platform does not support CollectionView")
                                        ]))

         | CarouselView ->
            match Device.RuntimePlatform with
                | Device.iOS | Device.Android -> 
                    View.ContentPage(content=
                        View.StackLayout(children = [
                            MainPageButton
                            View.CarouselView(items = [
                                View.Label(text="Person1") 
                                View.Label(text="Person2")
                                View.Label(text="Person3")
                                View.Label(text="Person4")
                                View.Label(text="Person5")
                                View.Label(text="Person6")
                                View.Label(text="Person7")
                                View.Label(text="Person8")
                                View.Label(text="Person9")
                                View.Label(text="Person11")
                                View.Label(text="Person12")
                                View.Label(text="Person13")
                                View.Label(text="Person14")
                            ], margin= Thickness 10.)
                        ]
                    ))

                | _ -> View.ContentPage(content = View.StackLayout( children = [
                                            MainPageButton
                                            View.Label(text="Your Platform does not support CarouselView")
                                        ]))
                
        | Effects ->
            View.ScrollingContentPage("Effects", [
                View.Label("Samples available on iOS and Android only")
                
                View.Label("Focus effect (no properties)", fontSize=FontSize 5., margin=Thickness (0., 30., 0., 0.))
                View.Label("Classic Entry field", margin=Thickness (0., 15., 0., 0.))
                View.Entry()
                View.Label("Entry field with Focus effect", margin=Thickness (0., 15., 0., 0.))
                View.Entry(effects = [
                    View.Effect("FabulousXamarinForms.FocusEffect")
                ])
                
                View.Label("Shadow effect (with properties)", fontSize=FontSize 15., margin=Thickness (0., 30., 0., 0.))
                View.Label("Classic Label field", margin=Thickness (0., 15., 0., 0.))
                View.Label("This is a label without shadows")
                View.Label("Label field with Shadow effect", margin=Thickness (0., 15., 0., 0.))
                View.Label("This is a label with shadows", effects = [
                    View.ShadowEffect(color=Color.Red, radius=15., distanceX=10., distanceY=10.)
                ])
            ])

    
type App () as app = 
    inherit Application ()
    do app.Resources.Add(Xamarin.Forms.StyleSheets.StyleSheet.FromAssemblyResource(System.Reflection.Assembly.GetExecutingAssembly(), "AllControls.styles.css"))

    let runner = 
        Program.mkProgram App.init App.update App.view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app

    member __.Program = runner
