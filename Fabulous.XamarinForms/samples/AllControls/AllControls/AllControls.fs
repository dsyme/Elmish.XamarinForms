// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace AllControls

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open FSharp.Data
open FSharp.Data.Adaptive
open SkiaSharp
open SkiaSharp.Views.Forms
open AllControls.Effects

type RootPageKind = 
    | Choice of bool
    | SkiaCanvas
    | SkiaCanvas2
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
    | RefreshView

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
    // For RefreshView
    RefreshViewIsRefreshing: bool
    SKSurfaceTouchCount: int
    }

type AdaptiveModel = 
    { RootPageKind: RootPageKind cval
      Count : int cval
      CountForSlider : int cval
      CountForActivityIndicator : int cval
      StepForSlider : int  cval
      MinimumForSlider : int cval
      MaximumForSlider : int cval
      StartDate : System.DateTime cval
      EndDate : System.DateTime cval
      EditorText : string cval
      EntryText : string cval
      Placeholder : string cval
      Password : string cval
      NumTaps : int  cval
      NumTaps2 : int  cval
      PickedColorIndex: int cval
      GridSize: int cval
      NewGridSize: double  cval
      GridPortal: (int * int) cval
      IsMasterPresented: bool  cval
      DetailPage: string cval
      PageStack: string option list  cval
      InfiniteScrollMaxRequested: int  cval
      SearchTerm: string cval
      CarouselCurrentPageIndex: int cval
      Tabbed1CurrentPageIndex: int cval
      IsRunning: bool cval
      ReceivedData: bool cval
      WebCallData: string option cval
      ScrollPosition: (float * float) cval
      AnimatedScroll: AnimationKind cval
      IsScrollingWithFabulous: bool cval
      IsScrolling: bool cval
      RefreshViewIsRefreshing: bool cval
      SKSurfaceTouchCount: int cval
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
    | SetGridSize of int
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
    | SKSurfaceTouched
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
    // For RefreshView
    | RefreshViewRefreshing
    | RefreshViewRefreshDone

[<AutoOpen>]
module MyExtension = 
    /// Test the extension API be making a 2nd wrapper for "Label":
    let TestLabelTextAttribKey = AttributeKey<_> "TestLabel_Text"
    let TestLabelFontFamilyAttribKey = AttributeKey<_> "TestLabel_FontFamily"

    type View with 

        static member TestLabel(?text: aval<string>, ?fontFamily: aval<string>, ?backgroundColor, ?rotation) = 

            // Get the attributes for the base element. The number is the expected number of attributes.
            // You can add additional base element attributes here if you like
            let attribCount = 0
            let attribCount = match text with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match fontFamily with Some _ -> attribCount + 1 | None -> attribCount
            let attribs = ViewBuilders.BuildView(attribCount, ?backgroundColor = backgroundColor, ?rotation = rotation) 
            let attribs = attribs.Retarget<Xamarin.Forms.Label>()

            let updater1 = ViewExtensions.ValueUpdater(text, (fun (target: Xamarin.Forms.Label) v -> target.Text <- v))
            let updater2 = ViewExtensions.ValueUpdater(fontFamily, (fun (target: Xamarin.Forms.Label) v -> target.FontFamily <- v))

            // Add our own attributes. They must have unique names.
            match text with None -> () | Some v -> attribs.Add(TestLabelTextAttribKey, v, updater1) 
            match fontFamily with None -> () | Some v -> attribs.Add(TestLabelFontFamilyAttribKey, v, updater2) 

            // The creation method
            let create () = new Xamarin.Forms.Label()

            ViewElement.Create<Xamarin.Forms.Label>(create, attribs.Close())

    // Test some adhoc functional abstractions
    type View with 
        static member ScrollingContentPage(title, children) =
            View.ContentPage(title=title, content=View.ScrollView(View.StackLayout(padding= c (Thickness 20.0), children=children) ), useSafeArea = c true)

        static member NonScrollingContentPage(title, children, ?gestureRecognizers) =
            View.ContentPage(title=title, content=View.StackLayout(padding=c (Thickness 20.0), children=children, ?gestureRecognizers=gestureRecognizers), useSafeArea = c true)


module App = 
    let init () : Model * _ = 
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
          IsScrolling = false
          RefreshViewIsRefreshing = false 
          SKSurfaceTouchCount = 0 }, Cmd.none

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

    let refreshAsync () =
        (async {
            do! Async.Sleep 2000
            return RefreshViewRefreshDone
        }) |> Cmd.ofAsyncMsg        
    
    let update msg (model: Model) =
        match msg with
        | SKSurfaceTouched -> { model with SKSurfaceTouchCount = model.SKSurfaceTouchCount + 1 }, Cmd.none
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
        | SetGridSize sz -> { model with GridSize = sz }, Cmd.none
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
        // For RefreshView
        | RefreshViewRefreshing ->
            { model with RefreshViewIsRefreshing = true }, refreshAsync ()
        | RefreshViewRefreshDone ->
            { model with RefreshViewIsRefreshing = false }, Cmd.none

    let pickerItems = 
        [ ("Aqua", Color.Aqua); ("Black", Color.Black);
           ("Blue", Color.Blue); ("Fucshia", Color.Fuchsia);
           ("Gray", Color.Gray); ("Green", Color.Green);
           ("Lime", Color.Lime); ("Maroon", Color.Maroon);
           ("Navy", Color.Navy); ("Olive", Color.Olive);
           ("Purple", Color.Purple); ("Red", Color.Red);
           ("Silver", Color.Silver); ("Teal", Color.Teal);
           ("White", Color.White); ("Yellow", Color.Yellow ) ]
        
    let updateViewEffects () =
        View.ScrollingContentPage(c "Effects", cs [
            View.Label(c "Samples available on iOS and Android only")
            
            View.Label(c "Focus effect (no properties)", fontSize= c (FontSize 5.), margin = c (Thickness (0., 30., 0., 0.)))
            View.Label(c "Classic Entry field", margin= c (Thickness (0., 15., 0., 0.)))
            View.Entry()
            View.Label(c "Entry field with Focus effect", margin= c (Thickness (0., 15., 0., 0.)))
            View.Entry(effects = cs [
                View.Effect(c "FabulousXamarinForms.FocusEffect")
            ])
            
            View.Label(c "Shadow effect (with properties)", fontSize=c (FontSize 15.), margin= c (Thickness (0., 30., 0., 0.)))
            View.Label(c "Classic Label field", margin= c (Thickness (0., 15., 0., 0.)))
            View.Label(c "This is a label without shadows")
            View.Label(c "Label field with Shadow effect", margin= c (Thickness (0., 15., 0., 0.)))
            View.Label(c "This is a label with shadows", effects = cs [
                View.ShadowEffect(color = c Color.Red, radius = c 15., distanceX = c 10., distanceY = c 10.)
            ])
        ])

    let view (model: AdaptiveModel) dispatch =
      aval {
        let MainPageButton = 
            View.Button(text = c "Main page", 
                        command = c (fun () -> dispatch (SetRootPageKind (Choice false))), 
                        minimumWidth = c 100.0,
                        minimumHeight = c 40.0,
                        horizontalOptions = c LayoutOptions.CenterAndExpand)

        let! rootPageKind = model.RootPageKind
        match rootPageKind with 
        | Choice showAbout -> 
          return
            View.NavigationPage(pages=
              cs [ 
                  yield 
                      View.ContentPage(useSafeArea=c true,
                        padding = c (Thickness (10.0, 20.0, 10.0, 5.0)), 
                        content = View.ScrollView(
                            content = View.StackLayout(
                                children = cs [ 
                                     View.Button(text = c "Skia Canvas", command = c (fun () -> dispatch (SetRootPageKind SkiaCanvas)))
                                     View.Button(text = c "Skia Canvas2", command = c (fun () -> dispatch (SetRootPageKind SkiaCanvas2)))
                                     View.Button(text = c "TabbedPage #1 (various controls)", command = c (fun () -> dispatch (SetRootPageKind Tabbed1)))
                                     View.Button(text = c "TabbedPage #2 (various controls)", command = c (fun () -> dispatch (SetRootPageKind Tabbed2)))
                                     View.Button(text = c "TabbedPage #3 (various controls)", command = c (fun () -> dispatch (SetRootPageKind Tabbed3)))
                                     View.Button(text = c "CarouselPage (various controls)", command = c (fun () -> dispatch (SetRootPageKind Carousel)))
                                     View.Button(text = c "NavigationPage with push/pop", command = c (fun () -> dispatch (SetRootPageKind Navigation)))
                                     View.Button(text = c "MasterDetail Page", command = c (fun () -> dispatch (SetRootPageKind MasterDetail)))
                                     View.Button(text = c "Infinite scrolling ListView", command = c (fun () -> dispatch (SetRootPageKind InfiniteScrollList)))
                                     View.Button(text = c "Animations", command = c (fun () -> dispatch (SetRootPageKind Animations)))
                                     View.Button(text = c "Pop-up", command = c (fun () -> dispatch ShowPopup))
                                     View.Button(text = c "WebRequest", command = c (fun () -> dispatch (SetRootPageKind WebCall)))
                                     View.Button(text = c "ScrollView", command = c (fun () -> dispatch (SetRootPageKind ScrollView)))
                                     View.Button(text = c "Shell", command = c (fun () -> dispatch (SetRootPageKind ShellView)))
                                     View.Button(text = c "CollectionView", command = c (fun () -> dispatch (SetRootPageKind CollectionView)))
                                     View.Button(text = c "CarouselView", command = c (fun () -> dispatch (SetRootPageKind CarouselView)))
                                     View.Button(text = c "Effects", command = c (fun () -> dispatch (SetRootPageKind Effects)))
                                     View.Button(text = c "RefreshView", command = c (fun () -> dispatch (SetRootPageKind RefreshView)))
                                ])))
                        .ToolbarItems(cs [View.ToolbarItem(text = c "about", command = c (fun () -> dispatch (SetRootPageKind (Choice true))))] )
                        .TitleView(View.StackLayout(orientation = c StackOrientation.Horizontal, children = cs [
                             View.Label(text = c "fabulous", verticalOptions = c LayoutOptions.Center)
                             View.Label(text = c "rootpage", verticalOptions = c LayoutOptions.Center, horizontalOptions = c LayoutOptions.CenterAndExpand)
                            ]
                        ))

                  if showAbout then 
                    yield 
                        View.ContentPage(title = c "About", useSafeArea = c true, 
                            padding = c (Thickness (10.0, 20.0, 10.0, 5.0)), 
                            content= View.StackLayout(
                               children = cs [ 
                                   View.TestLabel(text = c ("Fabulous, version " + string (typeof<ViewElement>.Assembly.GetName().Version)))
                                   View.Label(text = c "Now with CSS styling", styleClasses = c [ "cssCallout" ])
                                   View.Button(text = c "Continue", command = c (fun () -> dispatch (SetRootPageKind (Choice false)) ))
                               ]))
                ])

        | Carousel -> 
         return
            View.CarouselPage(
                    useSafeArea = c true,
                    currentPageChanged = c (fun index -> 
                        match index with
                        | None -> printfn "No page selected"
                        | Some ind ->
                            printfn "Page changed : %i" ind
                            dispatch (SetCarouselCurrentPage ind)
                    ),
                    currentPage = model.CarouselCurrentPageIndex,
                    children=
             cs [  View.ScrollingContentPage(c "Button", 
                    cs [ View.Label(text = c "Label:")
                         View.Label(text= (model.Count |> AVal.map (sprintf "%d")), horizontalOptions = c LayoutOptions.CenterAndExpand)
                 
                         View.Label(text = c "Button:")
                         View.Button(text = c "Increment", command = c (fun () -> dispatch Increment), horizontalOptions = c LayoutOptions.CenterAndExpand)
                 
                         View.Label(text = c "Button:")
                         View.Button(text = c "Decrement", command = c (fun () -> dispatch Decrement), horizontalOptions = c LayoutOptions.CenterAndExpand)

                         View.Button(text = c "Go to grid", cornerRadius = c 5, command = c (fun () -> dispatch (SetCarouselCurrentPage 6)), horizontalOptions = c LayoutOptions.CenterAndExpand, verticalOptions = c LayoutOptions.End)
                         
                         MainPageButton
                      ])

                   View.ScrollingContentPage(c "ActivityIndicator", 
                    cs [View.Label(text = c "Label:")
                        View.Label(text= (model.Count |> AVal.map (sprintf "%d")), horizontalOptions = c LayoutOptions.CenterAndExpand)
 
                        View.Label(text = c "ActivityIndicator (when count > 0):")
                        View.ActivityIndicator(isRunning=(model.Count |> AVal.map (fun count -> count > 0)), horizontalOptions = c LayoutOptions.CenterAndExpand)
                  
                        View.Label(text = c "Button:")
                        View.Button(text = c "Increment", command = c (fun () -> dispatch IncrementForActivityIndicator), horizontalOptions = c LayoutOptions.CenterAndExpand)

                        View.Label(text = c "Button:")
                        View.Button(text = c "Decrement", command = c (fun () -> dispatch DecrementForActivityIndicator), horizontalOptions = c LayoutOptions.CenterAndExpand)
                        MainPageButton
                      ])

                   View.ScrollingContentPage(c "DatePicker", 
                    cs [ View.Label(text = c "DatePicker (start):")
                         View.DatePicker(minimumDate = c System.DateTime.Today, maximumDate= c (DateTime.Today + TimeSpan.FromDays(365.0)), 
                             date= model.StartDate, 
                             dateSelected=c (fun args -> dispatch (StartDateSelected args.NewDate)), 
                             horizontalOptions = c LayoutOptions.CenterAndExpand)

                         View.Label(text = c "DatePicker (end):")
                         View.DatePicker(minimumDate= model.StartDate, 
                             maximumDate =  (model.StartDate |> AVal.map (fun startDate -> startDate + TimeSpan.FromDays(365.0))), 
                             date = model.EndDate, 
                             dateSelected = c (fun args -> dispatch (EndDateSelected args.NewDate)), 
                             horizontalOptions = c LayoutOptions.CenterAndExpand)
                         MainPageButton
                       ])

                   View.ScrollingContentPage(c "Editor", 
                    cs [ View.Label(text = c "Editor:")
                         View.Editor(text= model.EditorText, horizontalOptions = c LayoutOptions.FillAndExpand, 
                            textChanged= c (fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                            completed=c (fun text -> dispatch (EditorEditCompleted text)))
                         MainPageButton
                       ])

                   View.ScrollingContentPage(c "Entry", 
                    cs [ View.Label(text = c "Entry:")
                         View.Entry(text = model.EntryText, horizontalOptions = c LayoutOptions.CenterAndExpand, 
                             textChanged = c (fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                             completed = c (fun text -> dispatch (EntryEditCompleted text)))

                         View.Label(text = c "Entry (password):")
                         View.Entry(text = model.Password, isPassword = c true, horizontalOptions = c LayoutOptions.CenterAndExpand, 
                             textChanged = c (fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                             completed = c (fun text -> dispatch (PasswordEntryEditCompleted text)))

                         View.Label(text = c "Entry (placeholder):")
                         View.Entry(placeholder = model.Placeholder, horizontalOptions = c LayoutOptions.CenterAndExpand, 
                             textChanged = c (fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                             completed = c (fun text -> dispatch (PlaceholderEntryEditCompleted text)))

                         MainPageButton
                       ]) 

                   View.ScrollingContentPage(c "Frame", 
                    cs [ View.Label(text = c "Frame (hasShadow = c true):")
                         View.Frame(hasShadow = c true, backgroundColor = c Color.AliceBlue, horizontalOptions = c LayoutOptions.CenterAndExpand)

                         View.Label(text = c "Frame (tap once gesture):")
                         View.Frame(hasShadow = c true, 
                             backgroundColor = (model.NumTaps |> AVal.map (fun numTaps -> snd (pickerItems.[numTaps % pickerItems.Length]))), 
                             horizontalOptions = c LayoutOptions.CenterAndExpand, 
                             gestureRecognizers = cs [ View.TapGestureRecognizer(command = c (fun () -> dispatch FrameTapped)) ] )

                         View.Label(text = c "Frame (tap twice gesture):")
                         View.Frame(hasShadow = c true, 
                             backgroundColor = (model.NumTaps2 |> AVal.map (fun numTaps2 -> snd (pickerItems.[numTaps2 % pickerItems.Length]))), 
                             horizontalOptions = c LayoutOptions.CenterAndExpand, 
                             gestureRecognizers = cs [ View.TapGestureRecognizer(numberOfTapsRequired = c 2, command = c (fun () -> dispatch FrameTapped2)) ] )
                 
                         MainPageButton
                       ])

                   View.NonScrollingContentPage(c "Grid", 
                    cs [ View.Label(text = c "Grid (6x6, auto):")
                         View.Grid(rowdefs = c [for i in 1 .. 6 -> Auto], 
                             coldefs = c [for i in 1 .. 6 -> Auto], 
                             children = 
                              cs [ for i in 1 .. 6 do 
                                      for j in 1 .. 6 -> 
                                         let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0)
                                         View.BoxView(c color).Row(c (i-1)).Column(c (j-1)) ] )
                         MainPageButton
                       ])
           ])

        | SkiaCanvas ->
         return
            View.ScrollingContentPage(c "SkiaCanvas", cs [ 
                View.SKCanvasView(enableTouchEvents = c true, 
                    paintSurface = c (fun args -> 
                        let info = args.Info
                        let surface = args.Surface
                        let canvas = surface.Canvas

                        canvas.Clear() 
                        use paint = new SKPaint(Style = SKPaintStyle.Stroke, Color = Color.Red.ToSKColor(), StrokeWidth = 25.0f)
                        canvas.DrawCircle(float32 (info.Width / 2), float32 (info.Height / 2), 100.0f, paint)
                    ),
                    touch = c (fun args -> 
                        dispatch SKSurfaceTouched
                    ))
            ])
        | SkiaCanvas2 ->
         return
            View.ScrollingContentPage(c "SkiaCanvas #2", cs [ 

                View.SKCanvasView2(
                    shapes = cs [ SKShape.Circle(10.0, 20.0) ],
                    touch = c (fun args -> 
                        dispatch SKSurfaceTouched
                    ))
                       
                MainPageButton
            ])
        | Tabbed1 ->
         return
           View.TabbedPage(
                    useSafeArea = c true,
                    currentPageChanged = c (fun index ->
                        match index with
                        | None -> printfn "No tab selected"
                        | Some ind ->
                            printfn "Tab changed : %i" ind
                            dispatch (SetTabbed1CurrentPage ind)
                    ),
                    currentPage = model.Tabbed1CurrentPageIndex,
                    children = cs [
                     View.ScrollingContentPage(c "Slider", cs [ 
                           View.Label(text = c "Label:")
                           View.Label(text= (model.CountForSlider |> AVal.map (sprintf "%d")), 
                               horizontalOptions = c LayoutOptions.CenterAndExpand)

                           View.Label(text = c "Button:")
                           View.Button(text = c "Increment", 
                               command = c (fun () -> dispatch IncrementForSlider),
                               horizontalOptions = c LayoutOptions.CenterAndExpand)
                 
                           View.Label(text = c "Button:")
                           View.Button(text = c "Decrement", 
                               command = c (fun () -> dispatch DecrementForSlider), 
                               horizontalOptions = c LayoutOptions.CenterAndExpand)

                           View.Label(text = c "Button:")
                           View.Button(text = c "Set Minimum = 0 / Maximum = 10", 
                               command = c (fun () -> dispatch ChangeMinimumMaximumForSlider1),
                               horizontalOptions = c LayoutOptions.CenterAndExpand)

                           View.Button(text = c "Set Minimum = 15 / Maximum = 20",
                               command = c (fun () -> dispatch ChangeMinimumMaximumForSlider2),
                               horizontalOptions = c LayoutOptions.CenterAndExpand)

                           View.Label(text= ((model.MinimumForSlider, model.MaximumForSlider, model.StepForSlider) |||> AVal.map3 (fun minimum maximum step -> 
                               sprintf "Slider: (Minimum %d, Maximum %d, Value %d)" minimum maximum step)))

                           View.Slider(minimumMaximum = ((model.MinimumForSlider, model.MaximumForSlider) ||> AVal.map2 (fun minimum maximum ->  (float minimum, float maximum))), 
                               value = (model.StepForSlider |> AVal.map float), 
                               valueChanged = c (fun args -> dispatch (SliderValueChanged (int (args.NewValue + 0.5)))), 
                               horizontalOptions = c LayoutOptions.Fill) 

                           View.Button(text = c "Go to Image", 
                                command = c (fun () -> dispatch (SetTabbed1CurrentPage 4)), 
                                horizontalOptions = c LayoutOptions.CenterAndExpand, verticalOptions = c LayoutOptions.End)
                       
                           MainPageButton
                        ])

                     View.NonScrollingContentPage(c "Grid", 
                        cs [ View.Label(text = c "Grid (6x6, *):")
                             View.Grid(rowdefs= c [for i in 1 .. 6 -> Star], coldefs = c [for i in 1 .. 6 -> Star], 
                                children = cs [ 
                                    for i in 1 .. 6 do 
                                        for j in 1 .. 6 -> 
                                            let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0) 
                                            View.BoxView(c color).Row(c (i-1)).Column(c (j-1)) ] )
                             MainPageButton
                            ])

                     View.NonScrollingContentPage(c "Grid+Pinch", 
                       alist { 
                           let! gridSize = model.GridSize
                           View.Label(text = (model.NewGridSize |> AVal.map (sprintf "Grid (nxn, pinch, size = %f):")))
                           // The Grid doesn't change during the pinch...
                           View.Grid(rowdefs= c [ for i in 1 .. gridSize -> Star ], 
                                coldefs = c [ for i in 1 .. gridSize -> Star ], 
                                children = cs [ 
                                    for i in 1 .. gridSize do 
                                       for j in 1 .. gridSize -> 
                                           let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0) 
                                           View.BoxView(c color).Row(c (i-1)).Column(c (j-1)) ] )
                           View.Button(text = c (sprintf "Increase to %d" (gridSize+1)), command = c (fun _ -> dispatch (SetGridSize (gridSize + 1))))
                           View.Button(text = c (sprintf "Decrease to %d" (gridSize-1)), command = c (fun _ -> dispatch (SetGridSize (gridSize - 1))))
                           MainPageButton
                       }, 
                       gestureRecognizers = cs [ View.PinchGestureRecognizer(pinchUpdated = c (fun pinchArgs -> 
                                                    dispatch (UpdateNewGridSize (pinchArgs.Scale, pinchArgs.Status)))) ] )

                  //let dx, dy = gridPortal
                     //View.NonScrollingContentPage(c "Grid+Pan", 
                     //     children=
                     //      cs [ View.Label(text= sprintf "Grid (nxn, auto, edit entries, 1-touch pan, (%d, %d):" dx dy)
                     //           View.Grid(rowdefs= c [for row in 1 .. 6 -> Star], coldefs = c [for col in 1 .. 6 -> Star], 
                     //              children = cs [ for row in 1 .. 6 do 
                     //                               for col in 1 .. 6 ->
                     //                                 let item = View.Label(text=sprintf "(%d, %d)" (col+dx) (row+dy), backgroundColor = c Color.White, textColor = c Color.Black) 
                     //                                 item.Row(row-1).Column(col-1) ])
                     //           MainPageButton
                     //     ], 
                     //     gestureRecognizers = cs [ View.PanGestureRecognizer(touchPoints=1, panUpdated=(fun panArgs -> 
                     //                                if panArgs.StatusType = GestureStatus.Running then 
                     //                                    dispatch (UpdateGridPortal (dx - int (panArgs.TotalX/10.0), dy - int (panArgs.TotalY/10.0))))) ] )

                     View.NonScrollingContentPage(c "Image", 
                      cs [ View.Label(text = c "Image (URL):")
                           View.Image(source= c (Path "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg"), 
                               horizontalOptions = c LayoutOptions.FillAndExpand,
                               verticalOptions = c LayoutOptions.FillAndExpand)
                           View.Label(text = c "Image (Embedded):", margin = c (Thickness (0., 20., 0., 0.)))
                           View.Image(source= c (Source (ImageSource.FromResource("AllControls.Baboon_Serengeti.jpg", typeof<RootPageKind>.Assembly))), 
                                  horizontalOptions = c LayoutOptions.FillAndExpand,
                                  verticalOptions = c LayoutOptions.FillAndExpand) 
                           MainPageButton ])
                 ])
        | Tabbed2 ->
           return 
            View.TabbedPage(useSafeArea=c true, children = cs [
                View.ScrollingContentPage(c "Picker", 
                  cs [ View.Label(text = c "Picker:")
                       View.Picker(title = c "Choose Color:", 
                           textColor= (model.PickedColorIndex |> AVal.map (fun idx -> snd pickerItems.[idx])),
                           selectedIndex = model.PickedColorIndex , 
                           items= c (List.map fst pickerItems), 
                           horizontalOptions = c LayoutOptions.CenterAndExpand, 
                           selectedIndexChanged = c (fun (i, item) -> dispatch (PickerItemChanged i)))
                       MainPageButton
                     ])
                      
                View.ScrollingContentPage(c "ListView", 
                  cs [ MainPageButton
                       View.Label(text = c "ListView:")
                       View.ListView(
                           items = cs [ 
                               for i in 0 .. 10 do 
                                   yield View.TextCell (c "Ionide")
                                   yield View.ViewCell(
                                        view = View.Label(
                                            formattedText = View.FormattedString(cs [
                                                View.Span(text = c "Visual ", backgroundColor = c Color.Green)
                                                View.Span(text = c "Studio ", fontSize = c (FontSize 10.0))
                                            ])
                                        )
                                   ) 
                                   yield View.TextCell (c "Emacs")
                                   yield View.ViewCell(
                                        view = View.Label(
                                            formattedText = View.FormattedString(cs [
                                                View.Span(text = c "Visual ", fontAttributes = c FontAttributes.Bold)
                                                View.Span(text = c "Studio ", fontAttributes = c FontAttributes.Italic)
                                                View.Span(text = c "Code", foregroundColor = c Color.Blue)
                                            ])
                                        )
                                   )
                                   yield View.TextCell (c "Rider") ], 
                           horizontalOptions = c LayoutOptions.CenterAndExpand, 
                           itemSelected = c (fun idx -> dispatch (ListViewSelectedItemChanged idx)))
                    ])

                      
                View.ScrollingContentPage(c "SearchBar", 
                  cs [ View.Label(text = c "SearchBar:")
                       View.SearchBar(
                            placeholder = c "Enter search term",
                            searchCommand = c (fun searchBarText -> dispatch (ExecuteSearch searchBarText)),
                            searchCommandCanExecute = c true) 
                       View.Label(text = (model.SearchTerm |> AVal.map (fun searchTerm -> "You searched for " + searchTerm)))
                       MainPageButton ])

                //View.NonScrollingContentPage(c "ListViewGrouped", 
                //    cs [ View.Label(text = c "ListView (grouped):")
                //         View.ListViewGrouped(
                //             showJumpList=true,
                //             items= 
                //                [ 
                //                    "B", View.TextCell "B", [ View.TextCell "Baboon"; View.TextCell "Blue Monkey" ]
                //                    "C", View.TextCell "C", [ View.TextCell "Capuchin Monkey"; View.TextCell "Common Marmoset" ]
                //                    "G", View.TextCell "G", [ View.TextCell "Gibbon"; View.TextCell "Golden Lion Tamarin" ]
                //                    "H", View.TextCell "H", [ View.TextCell "Howler Monkey" ]
                //                    "J", View.TextCell "J", [ View.TextCell "Japanese Macaque" ]
                //                    "M", View.TextCell "M", [ View.TextCell "Mandrill" ]
                //                    "P", View.TextCell "P", [ View.TextCell "Proboscis Monkey"; View.TextCell "Pygmy Marmoset" ]
                //                    "R", View.TextCell "R", [ View.TextCell "Rhesus Macaque" ]
                //                    "S", View.TextCell "S", [ View.TextCell "Spider Monkey"; View.TextCell "Squirrel Monkey" ]
                //                    "V", View.TextCell "V", [ View.TextCell "Vervet Monkey" ]
                //                ], 
                //             horizontalOptions = c LayoutOptions.CenterAndExpand,
                //             itemSelected=(fun idx -> dispatch (ListViewGroupedSelectedItemChanged idx)))
                //         MainPageButton
                //       ])

               ])
        | Tabbed3 ->
           return
             View.TabbedPage(useSafeArea=c true, 
              children= cs [ 
                   View.ContentPage(title = c "FlexLayout", useSafeArea = c true,
                       padding = c (Thickness (10.0, 20.0, 10.0, 5.0)), 
                       content= 
                           View.FlexLayout(
                            direction = c FlexDirection.Column,
                            children = cs [
                                View.ScrollView(orientation = c ScrollOrientation.Both,
                                  content = View.FlexLayout(
                                      children = cs [
                                          View.Frame(height = c 480.0, width = c 300.0, 
                                              content = View.FlexLayout( direction = c FlexDirection.Column,
                                                  children = cs [ 
                                                      View.Label(text = c "Seated Monkey", margin = c (Thickness (0.0, 8.0)), 
                                                          fontSize = c (Named NamedSize.Large),
                                                          textColor = c Color.Blue)
                                                      View.Label(text = c "This monkey is laid back and relaxed, and likes to watch the world go by.",
                                                          margin = c (Thickness (0.0, 4.0)),
                                                          textColor = c Color.Black)
                                                      View.Label(text = c "  • Often smiles mysteriously",
                                                          margin = c (Thickness (0.0, 4.0)),
                                                          textColor = c Color.Black)
                                                      View.Label(text = c "  • Sleeps sitting up", 
                                                          margin = c (Thickness (0.0, 4.0)),
                                                          textColor = c Color.Black)
                                                      View.Image(height = c 240.0, 
                                                          width = c 160.0, 
                                                          source = c  (Path "https://upload.wikimedia.org/wikipedia/commons/thumb/6/66/Vervet_monkey_Krugersdorp_game_reserve_%285657678441%29.jpg/160px-Vervet_monkey_Krugersdorp_game_reserve_%285657678441%29.jpg")
                                                      ).Order(c -1).AlignSelf(c FlexAlignSelf.Center)
                                                      View.Label(margin= c (Thickness (0.0, 4.0))).Grow(c 1.0)
                                                      View.Button(text = c "Learn More",
                                                          fontSize = c (Named NamedSize.Large),
                                                          textColor = c Color.White,
                                                          backgroundColor = c Color.Green,
                                                          cornerRadius = c 20) ]),
                                              backgroundColor = c Color.LightYellow,
                                              borderColor = c Color.Blue,
                                              margin = c (Thickness 10.0),
                                              cornerRadius = c 15.0)
                                          View.Frame(height = c 480.0, width = c 300.0, 
                                              content = View.FlexLayout( direction = c FlexDirection.Column,
                                                  children = cs [ 
                                                      View.Label(text = c "Banana Monkey", 
                                                          margin = c (Thickness (0.0, 8.0)), 
                                                          fontSize = c (Named NamedSize.Large),
                                                          textColor = c Color.Blue)
                                                      View.Label(text = c "Watch this monkey eat a giant banana.", 
                                                          margin = c (Thickness (0.0, 4.0)),
                                                          textColor = c Color.Black)
                                                      View.Label(text = c "  • More fun than a barrel of monkeys",
                                                          margin = c (Thickness (0.0, 4.0)),
                                                          textColor = c Color.Black)
                                                      View.Label(text = c "  • Banana not included",
                                                          margin = c (Thickness (0.0, 4.0)),
                                                          textColor = c Color.Black)
                                                      View.Image(height = c 213.0, 
                                                          width = c 320.0, 
                                                          source = c (Path "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c1/Crab_eating_macaque_in_Ubud_with_banana.JPG/320px-Crab_eating_macaque_in_Ubud_with_banana.JPG")
                                                      ).Order(c -1).AlignSelf(c FlexAlignSelf.Center)
                                                      View.Label(margin = c (Thickness (0.0, 4.0))).Grow(c 1.0)
                                                      View.Button(text = c "Learn More",
                                                          fontSize = c (Named NamedSize.Large),
                                                          textColor = c Color.White,
                                                          backgroundColor = c Color.Green,
                                                          cornerRadius = c 20) ]),
                                              backgroundColor = c Color.LightYellow,
                                              borderColor = c Color.Blue,
                                              margin = c (Thickness 10.0),
                                              cornerRadius = c 15.0)
                                          
                                      ] ))
                                MainPageButton
                            ])) 

                   View.ScrollingContentPage(c "TableView", cs [
                      View.Label(text = c "TableView:")
                      View.TableView(
                        horizontalOptions = c LayoutOptions.StartAndExpand,
                        root = View.TableRoot(
                            items = cs [
                                View.TableSection(
                                    title = c "Videos",
                                    items = cs [
                                        View.SwitchCell(on = c true, text = c "Luca 2008", onChanged = c (fun args -> ()) ) 
                                        View.SwitchCell(on = c true, text = c "Don 2010", onChanged = c (fun args -> ()) )
                                    ]
                                )
                                View.TableSection(
                                    title = c "Books",
                                    items = cs [
                                        View.SwitchCell(on = c true, text = c "Expert F#", onChanged = c (fun args -> ()) ) 
                                        View.SwitchCell(on = c false, text = c "Programming F#", onChanged = c (fun args -> ()) )
                                    ]
                                )
                                View.TableSection(
                                    title = c "Contact",
                                    items = cs [
                                        View.EntryCell(label = c "Email", placeholder = c "foo@bar.com", completed = c (fun args -> ()) )
                                        View.EntryCell(label = c "Phone", placeholder = c "+44 87654321", completed = c (fun args -> ()) )
                                    ]
                                )
                            ]
                        )
                       )
                      MainPageButton ])

                   View.ContentPage(title = c "RelativeLayout", 
                     padding = c (Thickness (10.0, 20.0, 10.0, 5.0)), 
                     content= View.RelativeLayout(
                      children = cs [ 
                          View.Label(text = c "RelativeLayout Example", textColor = c Color.Red)
                                .XConstraint(c (Constraint.RelativeToParent(fun parent -> 0.0)))
                          View.Label(text = c "Positioned relative to my parent", textColor = c Color.Red)
                                .XConstraint(c (Constraint.RelativeToParent(fun parent -> parent.Width / 3.0)))
                                .YConstraint(c (Constraint.RelativeToParent(fun parent -> parent.Height / 2.0)))
                          MainPageButton
                                .XConstraint(c (Constraint.RelativeToParent(fun parent -> parent.Width / 2.0)))
                      ]))


                   View.ContentPage(title = c "AbsoluteLayout", useSafeArea = c true,
                       padding = c (Thickness (10.0, 20.0, 10.0, 5.0)), 
                       content= View.StackLayout(
                           children = cs [ 
                               View.Label(text = c "AbsoluteLayout Demo",
                                   fontSize = c (Named NamedSize.Large), horizontalOptions = c LayoutOptions.Center)
                               View.AbsoluteLayout(backgroundColor = c (Color.Blue.WithLuminosity(0.9)), 
                                   verticalOptions = c LayoutOptions.FillAndExpand, 
                                   children = cs [
                                      View.Label(text = c "Top Left", textColor = c Color.Black)
                                          .LayoutFlags(c AbsoluteLayoutFlags.PositionProportional)
                                          .LayoutBounds(c (Rectangle(0.0, 0.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize)))
                                      View.Label(text = c "Centered", textColor = c Color.Black)
                                          .LayoutFlags(c AbsoluteLayoutFlags.PositionProportional)
                                          .LayoutBounds(c (Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize)))
                                      View.Label(text = c "Bottom Right", textColor = c Color.Black)
                                          .LayoutFlags(c AbsoluteLayoutFlags.PositionProportional)
                                          .LayoutBounds(c (Rectangle(1.0, 1.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize))) ])
                               MainPageButton
                            ]))

                ])
         
        | Navigation -> 
          let! pageStack = model.PageStack
          return
         // NavigationPage example
              View.NavigationPage(pages=
                   cs [ for page in List.rev pageStack do
                          match page with 
                          | Some "Home" -> 
                              yield 
                                  View.ContentPage(useSafeArea = c true,
                                    content=View.StackLayout(
                                     children= cs [
                                         View.Label(text = c "Home Page", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center)
                                         View.Button(text = c "Push Page A", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "A")))
                                         View.Button(text = c "Push Page B", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "B")))
                
                                         View.Button(text = c "Main page", textColor = c Color.White, backgroundColor = c Color.Navy, command = c (fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions = c LayoutOptions.CenterAndExpand, verticalOptions = c LayoutOptions.End)
                                        ]) ).HasNavigationBar(c true).HasBackButton(c false)
                          | Some "A" -> 
                              yield 
                                View.ContentPage(useSafeArea = c true,
                                    content=
                                     View.StackLayout(
                                      children = cs [
                                        View.Label(text = c "Page A", verticalOptions = c LayoutOptions.Center, horizontalOptions = c LayoutOptions.Center)
                                        View.Button(text = c "Page B", verticalOptions = c LayoutOptions.Center, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "B")))
                                        View.Button(text = c "Page C", verticalOptions = c LayoutOptions.Center, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "C")))
                                        View.Button(text = c "Replace by Page B", verticalOptions = c LayoutOptions.Center, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (ReplacePage "B")))
                                        View.Button(text = c "Replace by Page C", verticalOptions = c LayoutOptions.Center, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (ReplacePage "C")))
                                        View.Button(text = c "Back", verticalOptions = c LayoutOptions.Center, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch PopPage ))
                                        ]) ).HasNavigationBar(c true).HasBackButton(c true)
                          | Some "B" -> 
                              yield 
                                View.ContentPage(useSafeArea = c true,
                                    content=View.StackLayout(
                                         children= cs [
                                              View.Label(text = c "Page B", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center)
                                              View.Label(text = c "(nb. no back button in navbar)", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center)
                                              View.Button(text = c "Page A", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "A")))
                                              View.Button(text = c "Page C", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "C")))
                                              View.Button(text = c "Back", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch PopPage ))
                                             ]) ).HasNavigationBar(c true).HasBackButton(c false)
                          | Some "C" -> 
                              yield 
                                View.ContentPage(useSafeArea = c true,
                                    content=View.StackLayout(
                                      children = cs [
                                        View.Label(text = c "Page C", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center)
                                        View.Label(text = c "(nb. no navbar)", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center)
                                        View.Button(text = c "Page A", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "A")))
                                        View.Button(text = c "Page B", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "B")))
                                        View.Button(text = c "Back", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch PopPage ))
                                        MainPageButton
                                        ]) ).HasNavigationBar(c false).HasBackButton(c false)

                          | _ -> 
                               ()  ], 
                     popped = c (fun args -> dispatch PagePopped) , 
                     poppedToRoot = c (fun args -> dispatch GoHomePage)  )

        | MasterDetail -> 
          return
         // MasterDetail where the Master acts as a hamburger-style menu
            View.MasterDetailPage(
               masterBehavior = c MasterBehavior.Popover, 
               isPresented = model.IsMasterPresented, 
               isPresentedChanged = c (fun b -> dispatch (IsMasterPresentedChanged b)), 
               master = 
                 View.ContentPage(useSafeArea = c true, title = c "Master", 
                  content = 
                    View.StackLayout(backgroundColor = c Color.Gray, 
                      children = cs [ 
                          View.Button(text = c "Detail A", textColor = c Color.White, backgroundColor = c Color.Navy, command = c (fun () -> dispatch (SetDetailPage "A")))
                          View.Button(text = c "Detail B", textColor = c Color.White, backgroundColor = c Color.Navy, command = c (fun () -> dispatch (SetDetailPage "B")))
                          View.Button(text = c "Main page", textColor = c Color.White, backgroundColor = c Color.Navy, command = c (fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions = c LayoutOptions.CenterAndExpand, verticalOptions = c LayoutOptions.End) 
                               ]) ), 
               detail = 
                 View.NavigationPage( 
                   pages = cs [
                     View.ContentPage(title = c "Detail", useSafeArea = c true,
                      content = 
                        View.StackLayout(backgroundColor = c Color.Gray, 
                          children = 
                              cs [ View.Label(text = c "Detail <detailPage>", textColor = c Color.White, backgroundColor = c Color.Navy)
                                   View.Button(text = c "Main page", textColor = c Color.White, backgroundColor = c Color.Navy, command = c (fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions = c LayoutOptions.CenterAndExpand, verticalOptions = c LayoutOptions.End)  ]) 
                          ).HasNavigationBar(c true).HasBackButton(c true) ], 
                   poppedToRoot = c (fun args -> dispatch (IsMasterPresentedChanged true) ) ) ) 

         | InfiniteScrollList -> 
          return
               View.ScrollingContentPage(c "ListView (InfiniteScrollList)", cs [
                   MainPageButton
                   View.Label(text = c "InfiniteScrollList:")
                   View.ListView(
                       items = alist { let! max = model.InfiniteScrollMaxRequested in  for i in 1 .. max -> View.TextCell(c ("Item " + string i), textColor=c (if i % 3 = 0 then Color.CadetBlue else Color.LightCyan)) }, 
                       horizontalOptions = c LayoutOptions.CenterAndExpand, 
                       // Every time the last element is needed, grow the set of data to be at least 10 bigger then that index 
                       itemAppearing = aval { let! max = model.InfiniteScrollMaxRequested in return (fun idx -> if idx >= max - 2 then dispatch (SetInfiniteScrollMaxIndex (idx + 10) ) ) } )
                 ] )

         | Animations -> 
          return
               View.ScrollingContentPage(c "Animations", cs [ 
                    View.Label(text = c "Rotate", created = c (fun l -> l.RotateTo (360.0, 2000u) |> ignore)) 
                    View.Label(text = c "Hello!", ref=c animatedLabelRef) 
                    View.Button(text = c "Poke", command = c (fun () -> dispatch AnimationPoked))
                    View.Button(text = c "Poke2", command = c (fun () -> dispatch AnimationPoked2))
                    View.Button(text = c "Poke3", command = c (fun () -> dispatch AnimationPoked3))
                    View.Button(text = c "Main page", cornerRadius = c 5, command = c (fun () -> dispatch (SetRootPageKind (Choice false))), horizontalOptions = c LayoutOptions.CenterAndExpand, verticalOptions = c LayoutOptions.End)
                  ] )
         | WebCall ->
            let! callData = model.WebCallData
            let data = 
                match callData with
                | Some v -> v
                | None -> ""

            return
              View.ContentPage(
                content = View.StackLayout(
                    children = cs [
                        View.Button(text = c "Get Data", command = c (fun () -> dispatch ReceiveData))
                        View.ActivityIndicator(isRunning = model.IsRunning)
                        View.Label(text = c data)
                        MainPageButton
                    ]
            ))
         | ScrollView ->
            let scrollToValue (x, y) animated =
                (x, y, animated)

            return
              View.ContentPage(
                content = View.StackLayout(
                    children = cs [
                        MainPageButton
                        View.Label(text = (model.IsScrolling |> AVal.map (fun isScrolling -> sprintf "Is scrolling: %b" isScrolling)))
                        View.Button(text = c "Scroll to top", command = c (fun () -> dispatch (ScrollFabulous (0.0, 0.0, Animated))))
                        View.ScrollView(
                            ref = c scrollViewRef,
                            //?scrollTo= (model.IsScrollingWithFabulous |> AVal.map (fun isScrollingWithFabulous -> Some (scrollToValue model.ScrollPosition model.AnimatedScroll) else None),
                            scrolled = c (fun args -> dispatch (Scrolled (args.ScrollX, args.ScrollY))),
                            content = View.StackLayout(
                                children = cs [
                                    yield View.Button(text = c "Scroll animated with Fabulous", command = c (fun () -> dispatch (ScrollFabulous (0.0, 750.0, Animated))))
                                    yield View.Button(text = c "Scroll not animated with Fabulous", command = c (fun () -> dispatch (ScrollFabulous (0.0, 750.0, NotAnimated))))
                                    yield View.Button(text = c "Scroll animated with Xamarin.Forms", command = c (fun () -> dispatch (ScrollXamarinForms (0.0, 750.0, Animated))))
                                    yield View.Button(text = c "Scroll not animated with Xamarin.Forms", command = c (fun () -> dispatch (ScrollXamarinForms (0.0, 750.0, NotAnimated))))

                                    for i = 0 to 75 do
                                        yield View.Label(text = c (sprintf "Item %i" i))
                                ]
                            )
                        )
                    ]
                ) 
            )
         | ShellView ->
          return
            
            match Device.RuntimePlatform with
                | Device.iOS | Device.Android -> 
                    
                    View.Shell( title = c "TitleShell",
                        items = cs [
                            View.ShellItem(
                                items = cs [
                                    View.ShellSection(items = cs [
                                        View.ShellContent(content=View.ContentPage(content=MainPageButton, title = c "ContentpageTitle"))         
                                    ])
                                ])
                        ])
                | _ -> View.ContentPage(content = View.Label(text = c "Your Platform does not support Shell"))

         | CollectionView ->
          return
            match Device.RuntimePlatform with
                | Device.iOS | Device.Android -> 
                    View.ContentPage(content=View.StackLayout(children = cs [
                            MainPageButton
                            // use Collectionview instead of listview 
                            View.CollectionView(items= cs [
                                View.Label(text = c "Person 1") 
                                View.Label(text = c "Person2")
                                View.Label(text = c "Person3")
                                View.Label(text = c "Person4")
                                View.Label(text = c "Person5")
                                View.Label(text = c "Person6")
                                View.Label(text = c "Person7")
                                View.Label(text = c "Person8")
                                View.Label(text = c "Person9")
                                View.Label(text = c "Person11")
                                View.Label(text = c "Person12")
                                View.Label(text = c "Person13")
                                View.Label(text = c "Person14")] )
                        ]
                    ))

                | _ -> View.ContentPage(content = View.StackLayout( children = cs [
                                            MainPageButton
                                            View.Label(text = c "Your Platform does not support CollectionView")
                                        ]))

         | CarouselView ->
          return
            match Device.RuntimePlatform with
                | Device.iOS | Device.Android -> 
                    View.ContentPage(content=
                        View.StackLayout(children = cs [
                            MainPageButton
                            View.CarouselView(items = cs [
                                View.Label(text = c "Person1") 
                                View.Label(text = c "Person2")
                                View.Label(text = c "Person3")
                                View.Label(text = c "Person4")
                                View.Label(text = c "Person5")
                                View.Label(text = c "Person6")
                                View.Label(text = c "Person7")
                                View.Label(text = c "Person8")
                                View.Label(text = c "Person9")
                                View.Label(text = c "Person11")
                                View.Label(text = c "Person12")
                                View.Label(text = c "Person13")
                                View.Label(text = c "Person14")
                            ], margin= c (Thickness 10.))
                        ]
                    ))

                | _ -> View.ContentPage(content = View.StackLayout( children = cs [
                                            MainPageButton
                                            View.Label(text = c "Your Platform does not support CarouselView")
                                        ]))
                
        | Effects ->
          return
            updateViewEffects ()
            
        | RefreshView ->
          return
            View.ContentPage(
                View.RefreshView(
                    isRefreshing = model.RefreshViewIsRefreshing,
                    refreshing = c (fun () -> dispatch RefreshViewRefreshing),
                    content = View.ScrollView(
                        View.BoxView(
                            height = c 150.,
                            width = c 150.,
                            color = (model.RefreshViewIsRefreshing |> AVal.map (fun isRefreshing -> if isRefreshing then Color.Red else Color.Blue))
                        )
                    )
                )
            )
        }

    let ainit (model: Model) : AdaptiveModel = 
        { RootPageKind = cval model.RootPageKind
          Count = cval model.Count
          CountForSlider = cval model.CountForSlider
          CountForActivityIndicator = cval model.CountForActivityIndicator
          StepForSlider = cval model.StepForSlider
          MinimumForSlider = cval model.MinimumForSlider
          MaximumForSlider = cval model.MaximumForSlider
          StartDate = cval model.StartDate
          EndDate = cval model.EndDate
          EditorText = cval model.EditorText
          EntryText = cval model.EntryText
          Placeholder = cval model.Placeholder
          Password = cval model.Password
          NumTaps = cval model.NumTaps
          NumTaps2 = cval model.NumTaps2
          PickedColorIndex = cval model.PickedColorIndex
          GridSize = cval model.GridSize
          NewGridSize= cval model.NewGridSize
          GridPortal= cval model.GridPortal
          IsMasterPresented= cval model.IsMasterPresented
          DetailPage= cval model.DetailPage
          PageStack= cval model.PageStack
          InfiniteScrollMaxRequested= cval model.InfiniteScrollMaxRequested
          SearchTerm= cval model.SearchTerm
          CarouselCurrentPageIndex= cval model.CarouselCurrentPageIndex
          Tabbed1CurrentPageIndex= cval model.Tabbed1CurrentPageIndex
          IsRunning= cval model.IsRunning
          ReceivedData= cval model.ReceivedData
          WebCallData= cval model.WebCallData
          ScrollPosition = cval model.ScrollPosition
          AnimatedScroll= cval model.AnimatedScroll
          IsScrollingWithFabulous= cval model.IsScrollingWithFabulous
          IsScrolling= cval model.IsScrolling
          RefreshViewIsRefreshing= cval model.RefreshViewIsRefreshing
          SKSurfaceTouchCount = cval model.SKSurfaceTouchCount
          }

    let adelta (model: Model) (amodel: AdaptiveModel) =
        transact (fun () -> 
            if model.RootPageKind <> amodel.RootPageKind.Value then 
                amodel.RootPageKind.Value <- model.RootPageKind 
            if model.Count <> amodel.Count.Value then 
                amodel.Count.Value <- model.Count
            if model.CountForSlider <> amodel.CountForSlider.Value then 
                amodel.CountForSlider.Value <- model.CountForSlider
            if model.CountForActivityIndicator <> amodel.CountForActivityIndicator.Value then 
                amodel.CountForActivityIndicator.Value <- model.CountForActivityIndicator
            if model.StepForSlider <> amodel.StepForSlider.Value then 
                amodel.StepForSlider.Value <- model.StepForSlider
            if model.MinimumForSlider <> amodel.MinimumForSlider.Value then 
                amodel.MinimumForSlider.Value <- model.MinimumForSlider
            if model.MaximumForSlider <> amodel.MaximumForSlider.Value then 
                amodel.MaximumForSlider.Value <- model.MaximumForSlider
            if model.StartDate <> amodel.StartDate.Value then 
                amodel.StartDate.Value <- model.StartDate
            if model.EndDate <> amodel.EndDate.Value then 
                amodel.EndDate.Value <- model.EndDate
            if model.EditorText <> amodel.EditorText.Value then 
                amodel.EditorText.Value <- model.EditorText
            if model.EntryText <> amodel.EntryText.Value then 
                amodel.EntryText.Value <- model.EntryText
            if model.Placeholder <> amodel.Placeholder.Value then 
                amodel.Placeholder.Value <- model.Placeholder
            if model.Password <> amodel.Password.Value then 
                amodel.Password.Value <- model.Password
            if model.NumTaps <> amodel.NumTaps.Value then 
                amodel.NumTaps.Value <- model.NumTaps
            if model.NumTaps2 <> amodel.NumTaps2.Value then 
                amodel.NumTaps2.Value <- model.NumTaps2
            if model.PickedColorIndex <> amodel.PickedColorIndex.Value then 
                amodel.PickedColorIndex.Value <- model.PickedColorIndex
            if model.GridSize <> amodel.GridSize.Value then 
                amodel.GridSize.Value <- model.GridSize
            if model.NewGridSize <> amodel.NewGridSize.Value then 
                amodel.NewGridSize.Value <- model.NewGridSize
            if model.GridPortal <> amodel.GridPortal.Value then 
                amodel.GridPortal.Value <- model.GridPortal
            if model.IsMasterPresented <> amodel.IsMasterPresented.Value then 
                amodel.IsMasterPresented.Value <- model.IsMasterPresented
            if model.DetailPage <> amodel.DetailPage.Value then 
                amodel.DetailPage.Value <- model.DetailPage
            if model.PageStack <> amodel.PageStack.Value then 
                amodel.PageStack.Value <- model.PageStack
            if model.InfiniteScrollMaxRequested <> amodel.InfiniteScrollMaxRequested.Value then 
                amodel.InfiniteScrollMaxRequested.Value <- model.InfiniteScrollMaxRequested
            if model.SearchTerm <> amodel.SearchTerm.Value then 
                amodel.SearchTerm.Value <- model.SearchTerm
            if model.CarouselCurrentPageIndex <> amodel.CarouselCurrentPageIndex.Value then 
                amodel.CarouselCurrentPageIndex.Value <- model.CarouselCurrentPageIndex
            if model.Tabbed1CurrentPageIndex <> amodel.Tabbed1CurrentPageIndex.Value then 
                amodel.Tabbed1CurrentPageIndex.Value <- model.Tabbed1CurrentPageIndex
            if model.IsRunning <> amodel.IsRunning.Value then 
                amodel.IsRunning.Value <- model.IsRunning
            if model.ReceivedData <> amodel.ReceivedData.Value then 
                amodel.ReceivedData.Value <- model.ReceivedData
            if model.WebCallData <> amodel.WebCallData.Value then 
                amodel.WebCallData.Value <- model.WebCallData
            if model.ScrollPosition <> amodel.ScrollPosition.Value then 
                amodel.ScrollPosition.Value <- model.ScrollPosition
            if model.AnimatedScroll <> amodel.AnimatedScroll.Value then 
                amodel.AnimatedScroll.Value <- model.AnimatedScroll
            if model.IsScrollingWithFabulous <> amodel.IsScrollingWithFabulous.Value then 
                amodel.IsScrollingWithFabulous.Value <- model.IsScrollingWithFabulous
            if model.IsScrolling <> amodel.IsScrolling.Value then 
                amodel.IsScrolling.Value <- model.IsScrolling
            if model.RefreshViewIsRefreshing <> amodel.RefreshViewIsRefreshing.Value then 
                amodel.RefreshViewIsRefreshing.Value <- model.RefreshViewIsRefreshing
            if model.SKSurfaceTouchCount <> amodel.SKSurfaceTouchCount.Value then 
                amodel.SKSurfaceTouchCount.Value <- model.SKSurfaceTouchCount
        )

    
type App () as app = 
    inherit Application ()
    do app.Resources.Add(Xamarin.Forms.StyleSheets.StyleSheet.FromAssemblyResource(System.Reflection.Assembly.GetExecutingAssembly(), "AllControls.styles.css"))

    let runner = 
        Program.mkProgram App.init App.update App.ainit App.adelta App.view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app

    member __.Program = runner
