// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace AllControls

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Xamarin.Forms.Maps
open FSharp.Data
open FSharp.Data.Adaptive
open SkiaSharp
open SkiaSharp.Views.Forms
open OxyPlot
open OxyPlot.Axes
open OxyPlot.Series
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
    | RefreshView
    | SkiaCanvas
    | SkiaCanvas2
    | MapSamples
    | OxyPlotSamples
    //| VideoSamples
    //| CachedImageSamples

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
    SKPoints: IndexList<SKPoint>
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
      SKPoints: SKPoint clist
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
    | SKSurfaceTouched of SKPoint
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
            View.ContentPage(title = c title, content = View.ScrollView(View.StackLayout(padding= c (Thickness 20.0), children = cs children) ), useSafeArea = c true)

        static member NonScrollingContentPage(title, children, ?gestureRecognizers) =
            View.ContentPage(title = c title, content = View.StackLayout(padding=c (Thickness 20.0), children = cs children, ?gestureRecognizers=gestureRecognizers), useSafeArea = c true)


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
          SKPoints = IndexList.empty  }, Cmd.none

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
        | SetInfiniteScrollMaxIndex idx -> 
            if idx + 10 >= max (idx + 10) model.InfiniteScrollMaxRequested then 
                { model with InfiniteScrollMaxRequested = idx + 10 }, Cmd.none 
            else model, Cmd.none
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
        | SKSurfaceTouched point -> { model with SKPoints = model.SKPoints.Add(point) }, Cmd.none


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
          SKPoints = ChangeableIndexList model.SKPoints
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
            amodel.SKPoints.UpdateTo(model.SKPoints, id, (fun p _ -> p))
            //System.Diagnostics.Debug.WriteLine (sprintf "#points = %d" amodel.SKPoints.Count)
            //amodel.SKPoints.UpdateTo(model.SKPoints)
        )
    let pickerItems = 
        [ ("Aqua", Color.Aqua); ("Black", Color.Black);
           ("Blue", Color.Blue); ("Fucshia", Color.Fuchsia);
           ("Gray", Color.Gray); ("Green", Color.Green);
           ("Lime", Color.Lime); ("Maroon", Color.Maroon);
           ("Navy", Color.Navy); ("Olive", Color.Olive);
           ("Purple", Color.Purple); ("Red", Color.Red);
           ("Silver", Color.Silver); ("Teal", Color.Teal);
           ("White", Color.White); ("Yellow", Color.Yellow ) ]


    let frontPage model showAbout dispatch =
        View.NavigationPage(pages=
            cs 
             [yield 
                View.ContentPage(
                    View.ScrollView(
                        View.StackLayout(
                            children = cs [ 
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
                                    View.Button(text = c "Skia Canvas", command = c (fun () -> dispatch (SetRootPageKind SkiaCanvas)))
                                    View.Button(text = c "Skia Canvas Adaptive", command = c (fun () -> dispatch (SetRootPageKind SkiaCanvas2)))
                                    View.Button(text = c "Map Samples", command = c (fun () -> dispatch (SetRootPageKind MapSamples)))
                                    View.Button(text = c "OxyPlot Samples", command = c (fun () -> dispatch (SetRootPageKind OxyPlotSamples)))
                                    //View.Button(text = c "VideoManager Samples", command = c (fun () -> dispatch (SetRootPageKind VideoSamples)))
                                    //View.Button(text = c "CachedImage Samples", command = c (fun () -> dispatch (SetRootPageKind CachedImageSamples)))
                            ])),
                    useSafeArea = c true,
                    padding = c (Thickness (10.0, 20.0, 10.0, 5.0))
                )
                .ToolbarItems(cs [ View.ToolbarItem(text = c "about", command = c (fun () -> dispatch (SetRootPageKind (Choice true))))] )
                .TitleView(
                    View.StackLayout(
                     cs [
                            View.Label(text = c "fabulous", verticalOptions = c LayoutOptions.Center)
                            View.Label(text = c "rootpage", verticalOptions = c LayoutOptions.Center, horizontalOptions = c LayoutOptions.CenterAndExpand)
                        ],
                        orientation = c StackOrientation.Horizontal
                    ))

              if showAbout then 
                yield 
                    View.ContentPage(title = c "About", useSafeArea = c true, 
                        padding = c (Thickness (10.0, 20.0, 10.0, 5.0)), 
                        content= View.StackLayout(
                            children =
                             cs [ 
                                View.TestLabel(text = c ("Fabulous, version " + string (typeof<ViewElement>.Assembly.GetName().Version)))
                                View.Label(text = c "Now with CSS styling", styleClasses = c [ "cssCallout" ])
                                View.Button(text = c "Continue", command = c (fun () -> dispatch (SetRootPageKind (Choice false)) ))
                             ]))
            ])
    

    let mainPageButton dispatch = 
        View.Button(text = c "Main page", 
                    command = c (fun () -> dispatch (SetRootPageKind (Choice false))), 
                    horizontalOptions = c LayoutOptions.CenterAndExpand)

    let carouselPageSample model dispatch =
        View.CarouselPage(
            useSafeArea = c true,
            currentPageChanged = c (fun index -> 
                match index with
                | None -> printfn "No page selected"
                | Some ind ->
                    printfn "Page changed : %i" ind
                    dispatch (SetCarouselCurrentPage ind)
            ),
            currentPage=model.CarouselCurrentPageIndex,
            children=
              cs [ 
                  View.ScrollingContentPage("Button", 
                    [ View.Label(text = c "Label:")
                      View.Label(text = (model.Count |> AVal.map (sprintf "%d")), horizontalOptions = c LayoutOptions.CenterAndExpand)
                 
                      View.Label(text = c "Button:")
                      View.Button(text = c "Increment", command = c (fun () -> dispatch Increment), horizontalOptions = c LayoutOptions.CenterAndExpand)
                 
                      View.Label(text = c "Button:")
                      View.Button(text = c "Decrement", command = c (fun () -> dispatch Decrement), horizontalOptions = c LayoutOptions.CenterAndExpand)

                      View.Button(text = c "Go to grid", cornerRadius = c 5, command = c (fun () -> dispatch (SetCarouselCurrentPage 6)), horizontalOptions = c LayoutOptions.CenterAndExpand, verticalOptions = c LayoutOptions.End)
                         
                      mainPageButton dispatch
                    ])

                  View.ScrollingContentPage("ActivityIndicator", 
                   [View.Label(text = c "Label:")
                    View.Label(text= (model.Count |> AVal.map (sprintf "%d")), horizontalOptions = c LayoutOptions.CenterAndExpand)
 
                    View.Label(text = c "ActivityIndicator (when count > 0):")
                    View.ActivityIndicator(isRunning=(model.Count |> AVal.map (fun count -> count > 0)), horizontalOptions = c LayoutOptions.CenterAndExpand)
                  
                    View.Label(text = c "Button:")
                    View.Button(text = c "Increment", command = c (fun () -> dispatch IncrementForActivityIndicator), horizontalOptions = c LayoutOptions.CenterAndExpand)

                    View.Label(text = c "Button:")
                    View.Button(text = c "Decrement", command = c (fun () -> dispatch DecrementForActivityIndicator), horizontalOptions = c LayoutOptions.CenterAndExpand)
                    mainPageButton dispatch
                   ])

                  View.ScrollingContentPage("DatePicker", 
                    [ View.Label(text = c "DatePicker (start):")
                      View.DatePicker(minimumDate= c System.DateTime.Today, maximumDate = c (DateTime.Today + TimeSpan.FromDays(365.0)), 
                            date = model.StartDate, 
                            dateSelected = c (fun args -> dispatch (StartDateSelected args.NewDate)), 
                            horizontalOptions = c LayoutOptions.CenterAndExpand)

                      View.Label(text = c "DatePicker (end):")
                      View.DatePicker(minimumDate = model.StartDate, maximumDate = (model.StartDate |> AVal.map (fun d -> d + TimeSpan.FromDays(365.0))), 
                            date = model.EndDate, 
                            dateSelected = c (fun args -> dispatch (EndDateSelected args.NewDate)), 
                            horizontalOptions = c LayoutOptions.CenterAndExpand)
                      mainPageButton dispatch
                    ])

                  View.ScrollingContentPage("Editor", 
                    [ View.Label(text = c "Editor:")
                      View.Editor(text = model.EditorText, horizontalOptions = c LayoutOptions.FillAndExpand, 
                        textChanged = c (fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                        completed = c (fun text -> dispatch (EditorEditCompleted text)))
                      mainPageButton dispatch
                    ])

                  View.ScrollingContentPage("Entry", 
                    [ View.Label(text = c "Entry:")
                      View.Entry(text= model.EntryText, horizontalOptions = c LayoutOptions.CenterAndExpand, 
                            textChanged = c (fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                            completed = c (fun text -> dispatch (EntryEditCompleted text)))

                      View.Label(text = c "Entry (password):")
                      View.Entry(text= model.Password, isPassword = c true, horizontalOptions = c LayoutOptions.CenterAndExpand, 
                            textChanged = c (fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                            completed = c (fun text -> dispatch (PasswordEntryEditCompleted text)))

                      View.Label(text = c "Entry (placeholder):")
                      View.Entry(placeholder= model.Placeholder, horizontalOptions = c LayoutOptions.CenterAndExpand, 
                            textChanged = c (fun args -> dispatch (TextChanged(args.OldTextValue, args.NewTextValue))), 
                            completed = c (fun text -> dispatch (PlaceholderEntryEditCompleted text)))

                      mainPageButton dispatch
                    ]) 

                  View.ScrollingContentPage("Frame", 
                    [ View.Label(text = c "Frame (hasShadow=true):")
                      View.Frame(hasShadow = c true, backgroundColor = c Color.AliceBlue, horizontalOptions = c LayoutOptions.CenterAndExpand)

                      View.Label(text = c "Frame (tap once gesture):")
                      View.Frame(hasShadow = c true, 
                            backgroundColor = (model.NumTaps |> AVal.map (fun numTaps -> snd (pickerItems.[numTaps % pickerItems.Length]))), 
                            horizontalOptions = c LayoutOptions.CenterAndExpand, 
                            gestureRecognizers = cs [ View.TapGestureRecognizer(command = c (fun () -> dispatch FrameTapped)) ] )

                      View.Label(text = c "Frame (tap twice gesture):")
                      View.Frame(hasShadow = c true, 
                            backgroundColor = (model.NumTaps2 |> AVal.map (fun numTaps -> snd (pickerItems.[numTaps % pickerItems.Length]))), 
                            horizontalOptions = c LayoutOptions.CenterAndExpand, 
                            gestureRecognizers = cs [ View.TapGestureRecognizer(numberOfTapsRequired = c 2, command = c (fun () -> dispatch FrameTapped2)) ] )
                 
                      mainPageButton dispatch
                    ])

                  View.NonScrollingContentPage("Grid", 
                    [ View.Label(text = c (sprintf "Grid (6x6, auto):"))
                      View.Grid(rowdefs = c [for i in 1 .. 6 -> Auto], 
                            coldefs = c [for i in 1 .. 6 -> Auto], 
                            children = 
                             cs [ for i in 1 .. 6 do 
                                    for j in 1 .. 6 -> 
                                        let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0)
                                        View.BoxView(c color).Row(c (i-1)).Column(c (j-1)) ] )
                      mainPageButton dispatch
                    ])
        ])

    let tabbedPageSamples1 model dispatch = 
        View.TabbedPage(
            useSafeArea = c true,
            currentPageChanged = c (fun index ->
                match index with
                | None -> printfn "No tab selected"
                | Some ind ->
                    printfn "Tab changed : %i" ind
                    dispatch (SetTabbed1CurrentPage ind)
            ),
            currentPage=model.Tabbed1CurrentPageIndex,
            children=
              cs [
                View.ScrollingContentPage("Slider", 
                    [ View.Label(text = c "Label:")
                      View.Label(text = (model.CountForSlider |> AVal.map (sprintf "%d")), horizontalOptions = c LayoutOptions.CenterAndExpand)

                      View.Label(text = c "Button:")
                      View.Button(text = c "Increment", command = c (fun () -> dispatch IncrementForSlider), horizontalOptions = c LayoutOptions.CenterAndExpand)
                 
                      View.Label(text = c "Button:")
                      View.Button(text = c "Decrement", command = c (fun () -> dispatch DecrementForSlider), horizontalOptions = c LayoutOptions.CenterAndExpand)

                      View.Label(text = c "Button:")
                      View.Button(text = c "Set Minimum = 0 / Maximum = 10", command = c (fun () -> dispatch ChangeMinimumMaximumForSlider1), horizontalOptions = c LayoutOptions.CenterAndExpand)
                      View.Button(text = c "Set Minimum = 15 / Maximum = 20", command = c (fun () -> dispatch ChangeMinimumMaximumForSlider2), horizontalOptions = c LayoutOptions.CenterAndExpand)

                      View.Label(text = ((model.MinimumForSlider, model.MaximumForSlider, model.StepForSlider) |||> AVal.map3 (fun min max step ->sprintf "Slider: (Minimum %d, Maximum %d, Value %d)" min max step)))
                      View.Slider(minimumMaximum = ((model.MinimumForSlider, model.MaximumForSlider) ||> AVal.map2 (fun min max -> (float min, float max))), 
                        value = (model.StepForSlider |> AVal.map double), 
                        valueChanged = c (fun args -> dispatch (SliderValueChanged (int (args.NewValue + 0.5)))), 
                        horizontalOptions = c LayoutOptions.Fill) 

                      View.Button(text = c "Go to Image", 
                        command = c (fun () -> dispatch (SetTabbed1CurrentPage 4)), 
                        horizontalOptions = c LayoutOptions.CenterAndExpand, verticalOptions = c LayoutOptions.End)
                       
                      mainPageButton dispatch
                  ])

                View.NonScrollingContentPage("Grid", 
                    [ View.Label(text = c (sprintf "Grid (6x6, *):"))
                      View.Grid(rowdefs = c [for i in 1 .. 6 -> Star], coldefs = c [for i in 1 .. 6 -> Star], 
                        children = 
                          cs [ 
                            for i in 1 .. 6 do 
                                for j in 1 .. 6 -> 
                                    let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0) 
                                    View.BoxView(c color).Row(c (i-1)).Column(c (j-1)) ] )
                      mainPageButton dispatch
                    ])

                View.NonScrollingContentPage("Grid+Pinch", 
                    [ View.Label(text = (model.NewGridSize |> AVal.map (sprintf "Grid (nxn, pinch, size = %f):")))
                      // The Grid doesn't change during the pinch...
                      View.Grid(
                          rowdefs= aval { let! gridSize = model.GridSize in return [ for i in 1 .. gridSize -> Star ] }, 
                          coldefs = aval { let! gridSize = model.GridSize in return [ for i in 1 .. gridSize -> Star ] }, 
                          children = 
                             alist {
                                let! gridSize = model.GridSize
                                for i in 1 .. gridSize do 
                                    for j in 1 .. gridSize -> 
                                        let color = Color((1.0/float i), (1.0/float j), (1.0/float (i+j)), 1.0) 
                                        View.BoxView(c color).Row(c (i-1)).Column(c (j-1) )
                              })
                      mainPageButton dispatch
                    ], 
                    gestureRecognizers = cs [ View.PinchGestureRecognizer(pinchUpdated = c (fun pinchArgs -> 
                                            dispatch (UpdateNewGridSize (pinchArgs.Scale, pinchArgs.Status)))) ] )

                //let dx, dy = gridPortal
                View.NonScrollingContentPage("Grid+Pan", 
                    children=
                        [ View.Label(text = (model.GridPortal |> AVal.map (fun (dx, dy) -> sprintf "Grid (nxn, auto, edit entries, 1-touch pan, (%d, %d):" dx dy)))
                          View.Grid(
                            rowdefs = c [for row in 1 .. 6 -> Star],
                            coldefs = c [for col in 1 .. 6 -> Star], 
                            children = 
                                alist { 
                                   let! dx, dy = model.GridPortal
                                   for row in 1 .. 6 do 
                                       for col in 1 .. 6 do
                                           let item = View.Label(text = c (sprintf "(%d, %d)" (col+dx) (row+dy)), backgroundColor = c Color.White, textColor = c Color.Black) 
                                           yield item.Row(c (row-1)).Column(c (col-1)) 
                                })
                          mainPageButton dispatch
                    ], 
                    gestureRecognizers = 
                        cs [ View.PanGestureRecognizer(touchPoints = c 1, panUpdated = (model.GridPortal |> AVal.map (fun (dx, dy) -> (fun panArgs -> 
                            if panArgs.StatusType = GestureStatus.Running then 
                                dispatch (UpdateGridPortal (dx - int (panArgs.TotalX/10.0), dy - int (panArgs.TotalY/10.0))))))) ] )

                View.NonScrollingContentPage("Image", 
                    [ View.Label(text = c "Image (URL):")
                      View.Image(source = c (Path "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg"), 
                          horizontalOptions = c LayoutOptions.FillAndExpand,
                          verticalOptions = c LayoutOptions.FillAndExpand)
                      View.Label(text = c "Image (Embedded):", margin = c (Thickness (0., 20., 0., 0.)))
                      View.Image(source = c (Source (ImageSource.FromResource("AllControls.Baboon_Serengeti.jpg", typeof<RootPageKind>.Assembly))), 
                          horizontalOptions = c LayoutOptions.FillAndExpand,
                          verticalOptions = c LayoutOptions.FillAndExpand) 
                      mainPageButton dispatch ])
               ])

    let tabbedPageSamples2 model dispatch =
        View.TabbedPage(useSafeArea = c true, 
          children= 
           cs [
                View.ScrollingContentPage("Picker", 
                    [ View.Label(text = c "Picker:")
                      View.Picker(title = c "Choose Color:", 
                          textColor = (model.PickedColorIndex |> AVal.map (fun i -> snd pickerItems.[i])), 
                          selectedIndex = model.PickedColorIndex, 
                          items = c (List.map fst pickerItems), 
                          horizontalOptions = c LayoutOptions.CenterAndExpand, 
                          selectedIndexChanged = c (fun (i, item) -> dispatch (PickerItemChanged i)))
                      mainPageButton dispatch
                    ])
                      
                View.ScrollingContentPage("ListView", 
                    [ mainPageButton dispatch
                      View.Label(text = c "ListView:")
                      View.ListView( 
                          items = cs [ 
                            for i in 0 .. 10 do 
                                yield View.TextCell (c "Ionide")
                                yield View.ViewCell(
                                    view = View.Label(
                                        formattedText = View.FormattedString(
                                          cs [
                                            View.Span(text = c "Visual ", backgroundColor = c Color.Green)
                                            View.Span(text = c "Studio ", fontSize = c (FontSize 10.))
                                          ])
                                    )
                                ) 
                                yield View.TextCell (c "Emacs")
                                yield View.ViewCell(
                                    view = View.Label(
                                        formattedText = View.FormattedString(
                                          cs [
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
                      
                View.ScrollingContentPage("SearchBar", 
                    [ View.Label(text = c "SearchBar:")
                      View.SearchBar(
                        placeholder = c "Enter search term",
                        searchCommand = c (fun searchBarText -> dispatch (ExecuteSearch searchBarText)),
                        searchCommandCanExecute = c true) 
                      View.Label(text = (model.SearchTerm |> AVal.map (fun x -> "You searched for " + x))) 
                      mainPageButton dispatch ])

                //View.NonScrollingContentPage("ListViewGrouped", 
                //    [ View.Label(text = c "ListView (grouped):")
                //      View.ListViewGrouped(
                //            showJumpList = c true,
                //            items= [ 
                //                "B", View.TextCell "B", [ View.TextCell "Baboon"; View.TextCell "Blue Monkey" ]
                //                "C", View.TextCell "C", [ View.TextCell "Capuchin Monkey"; View.TextCell "Common Marmoset" ]
                //                "G", View.TextCell "G", [ View.TextCell "Gibbon"; View.TextCell "Golden Lion Tamarin" ]
                //                "H", View.TextCell "H", [ View.TextCell "Howler Monkey" ]
                //                "J", View.TextCell "J", [ View.TextCell "Japanese Macaque" ]
                //                "M", View.TextCell "M", [ View.TextCell "Mandrill" ]
                //                "P", View.TextCell "P", [ View.TextCell "Proboscis Monkey"; View.TextCell "Pygmy Marmoset" ]
                //                "R", View.TextCell "R", [ View.TextCell "Rhesus Macaque" ]
                //                "S", View.TextCell "S", [ View.TextCell "Spider Monkey"; View.TextCell "Squirrel Monkey" ]
                //                "V", View.TextCell "V", [ View.TextCell "Vervet Monkey" ]
                //            ], 
                //            horizontalOptions = c LayoutOptions.CenterAndExpand,
                //            itemSelected = c (fun idx -> dispatch (ListViewGroupedSelectedItemChanged idx)))
                //      mainPageButton dispatch
                //    ])

            ])

    let tabbedPageSamples3 model dispatch =
        View.TabbedPage(useSafeArea = c true, 
            children=
             cs [ 
                View.ContentPage(title = c "FlexLayout", useSafeArea = c true,
                       padding = c (Thickness (10.0, 20.0, 10.0, 5.0)), 
                       content= 
                           View.FlexLayout(
                            direction = c FlexDirection.Column,
                            children = cs [
                                View.ScrollView(
                                  View.FlexLayout(
                                      children = cs [
                                          View.Frame(height = c 480.0, width = c 300.0, 
                                              content = View.FlexLayout( direction = c FlexDirection.Column,
                                                  children = cs [ 
                                                      View.Label(text = c "Seated Monkey", margin = c (Thickness (0.0, 8.0)), fontSize = c (Named NamedSize.Large), textColor = c Color.Blue)
                                                      View.Label(text = c "This monkey is laid back and relaxed, and likes to watch the world go by.", margin = c (Thickness (0.0, 4.0)), textColor = c Color.Black)
                                                      View.Label(text = c "  • Often smiles mysteriously", margin = c (Thickness (0.0, 4.0)), textColor = c Color.Black)
                                                      View.Label(text = c "  • Sleeps sitting up", margin = c (Thickness (0.0, 4.0)), textColor = c Color.Black)
                                                      View.Image(height =c 240.0, 
                                                          width = c 160.0, 
                                                          source = c (Path "https://upload.wikimedia.org/wikipedia/commons/thumb/6/66/Vervet_monkey_Krugersdorp_game_reserve_%285657678441%29.jpg/160px-Vervet_monkey_Krugersdorp_game_reserve_%285657678441%29.jpg")
                                                      ).Order(c -1).AlignSelf(c FlexAlignSelf.Center)
                                                      View.Label(margin = c (Thickness (0.0, 4.0))).Grow(c 1.0)
                                                      View.Button(text = c "Learn More", fontSize = c (Named NamedSize.Large), textColor = c Color.White, backgroundColor = c Color.Green, cornerRadius = c 20) ]),
                                              backgroundColor = c Color.LightYellow,
                                              borderColor = c Color.Blue,
                                              margin = c (Thickness 10.0),
                                              cornerRadius = c 15.0)
                                          View.Frame(height = c 480.0, width = c 300.0, 
                                              content = View.FlexLayout( direction = c FlexDirection.Column,
                                                  children = cs [ 
                                                      View.Label(text = c "Banana Monkey", margin = c (Thickness (0.0, 8.0)), fontSize = c (Named NamedSize.Large), textColor = c Color.Blue)
                                                      View.Label(text = c "Watch this monkey eat a giant banana.", margin = c (Thickness (0.0, 4.0)), textColor = c Color.Black)
                                                      View.Label(text = c "  • More fun than a barrel of monkeys", margin = c (Thickness (0.0, 4.0)), textColor = c Color.Black)
                                                      View.Label(text = c "  • Banana not included", margin = c (Thickness (0.0, 4.0)), textColor = c Color.Black)
                                                      View.Image(height = c 213.0, 
                                                          width = c 320.0, 
                                                          source = c (Path "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c1/Crab_eating_macaque_in_Ubud_with_banana.JPG/320px-Crab_eating_macaque_in_Ubud_with_banana.JPG")
                                                      ).Order(c -1).AlignSelf(c FlexAlignSelf.Center)
                                                      View.Label(margin = c (Thickness (0.0, 4.0))).Grow(c 1.0)
                                                      View.Button(text = c "Learn More", fontSize = c (Named NamedSize.Large), textColor = c Color.White, backgroundColor = c Color.Green, cornerRadius = c 20) ]),
                                              backgroundColor = c Color.LightYellow,
                                              borderColor = c Color.Blue,
                                              margin = c (Thickness 10.0),
                                              cornerRadius = c 15.0)

                                      ] ),
                                  orientation = c ScrollOrientation.Both)
                                mainPageButton dispatch
                            ])) 

                View.ScrollingContentPage("TableView", 
                 [View.Label(text = c "TableView:")
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
                  mainPageButton dispatch
                    ])

                View.ContentPage(title = c "RelativeLayout", 
                  padding = c (Thickness (10.0, 20.0, 10.0, 5.0)), 
                  content= View.RelativeLayout(
                      children = cs [ 
                          View.Label(text = c "RelativeLayout Example", textColor = c Color.Red)
                                .XConstraint(c (Constraint.RelativeToParent(fun parent -> 0.0)))
                          View.Label(text = c "Positioned relative to my parent", textColor = c Color.Red)
                                .XConstraint(c (Constraint.RelativeToParent(fun parent -> parent.Width / 3.0)))
                                .YConstraint(c (Constraint.RelativeToParent(fun parent -> parent.Height / 2.0)))
                          (mainPageButton dispatch)
                                .XConstraint(c (Constraint.RelativeToParent(fun parent -> parent.Width / 2.0)))
                      ]))


                View.ContentPage(title = c "AbsoluteLayout", useSafeArea = c true,
                       padding = c (Thickness (10.0, 20.0, 10.0, 5.0)), 
                       content= View.StackLayout(
                           children = cs [ 
                               View.Label(text = c "AbsoluteLayout Demo", fontSize = c (Named NamedSize.Large), horizontalOptions = c LayoutOptions.Center)
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
                               mainPageButton dispatch
                            ]))

              ])

    let navigationPageSample (model: AdaptiveModel) dispatch =

         // NavigationPage example
              View.NavigationPage(
                 pages=
                   alist { 
                      let! pageStack = model.PageStack
                      for page in List.rev pageStack do
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
                                      children=
                                       cs [
                                        View.Label(text = c "Page C", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center)
                                        View.Label(text = c "(nb. no navbar)", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center)
                                        View.Button(text = c "Page A", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "A")))
                                        View.Button(text = c "Page B", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch (PushPage "B")))
                                        View.Button(text = c "Back", verticalOptions = c LayoutOptions.CenterAndExpand, horizontalOptions = c LayoutOptions.Center, command = c (fun () -> dispatch PopPage ))
                                        mainPageButton dispatch
                                        ]) ).HasNavigationBar(c false).HasBackButton(c false)

                          | _ -> 
                               ()  }, 
                 popped = c (fun args -> dispatch PagePopped) , 
                 poppedToRoot = c (fun args -> dispatch GoHomePage)  )

    let viewEffectsSample dispatch =
        match Device.RuntimePlatform with
        | Device.iOS | Device.Android -> 
            View.ScrollingContentPage("Effects", 
              [
                View.Label(c "Effects samples available on iOS and Android only")
                mainPageButton dispatch
                View.Label(c "Focus effect (no properties)", fontSize = c (FontSize 5.), margin = c (Thickness (0., 30., 0., 0.)))
                View.Label(c "Classic Entry field", margin= c (Thickness (0., 15., 0., 0.)))
                View.Entry()
                View.Label(c "Entry field with Focus effect", margin = c (Thickness (0., 15., 0., 0.)))
                View.Entry(effects = 
                  cs [
                       View.Effect(c "FabulousXamarinForms.FocusEffect")
                  ])
            
                View.Label(c "Shadow effect (with properties)", fontSize = c (FontSize 15.), margin = c (Thickness (0., 30., 0., 0.)))
                View.Label(c "Classic Label field", margin = c (Thickness (0., 15., 0., 0.)))
                View.Label(c "This is a label without shadows")
                View.Label(c "Label field with Shadow effect", margin = c (Thickness (0., 15., 0., 0.)))
                View.Label(c "This is a label with shadows", effects = 
                  cs [
                       View.ShadowEffect(color = c Color.Red, radius = c 15., distanceX = c 10., distanceY = c 10.)
                  ])
              ])
        | _ -> 
            View.ContentPage(
                View.StackLayout( 
                  cs [
                    mainPageButton dispatch
                    View.Label(text = c "Effects samples available on iOS and Android only")
                  ]))

    let carouselViewSample model dispatch =
        match Device.RuntimePlatform with
        | Device.iOS | Device.Android -> 
            View.ContentPage(
                View.StackLayout 
                  (cs [
                    mainPageButton dispatch
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
                  ])
            )

        | _ -> 
            View.ContentPage(
                View.StackLayout (cs [
                    mainPageButton dispatch
                    View.Label(text = c "Your Platform does not support CarouselView")
                  ]))

    let masterDetailPageSample model dispatch =
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

    let infiniteScrollListSample (model: AdaptiveModel) dispatch =
               View.ScrollingContentPage("ListView (InfiniteScrollList)", 
                [mainPageButton dispatch
                 View.Label(text = c "InfiniteScrollList:")
                 View.ListView(items = 
                                 alist { 
                                         // TODO: this could use MinRequested/MaxRequested AList.range - much, much more efficient
                                         let! max = model.InfiniteScrollMaxRequested
                                         for i in 1 .. max do
                                           yield View.TextCell(c ("Item " + string i), textColor= c (if i % 3 = 0 then Color.CadetBlue else Color.LightCyan)) 
                                 }, 
                               horizontalOptions = c LayoutOptions.CenterAndExpand, 
                               // Every time the last element is needed, grow the set of data to be at least 10 bigger then that index 
                               itemAppearing = c (fun idx -> dispatch (SetInfiniteScrollMaxIndex idx) )  )
                 ] )

    let animationSamples model dispatch =
        View.ScrollingContentPage("Animations", 
          [ 
            View.Label(text = c "Rotate", created = c (fun l -> l.RotateTo (360.0, 2000u) |> ignore)) 
            View.Label(text = c "Hello!", ref = c animatedLabelRef) 
            View.Button(text = c "Poke", command = c (fun () -> dispatch AnimationPoked))
            View.Button(text = c "Poke2", command = c (fun () -> dispatch AnimationPoked2))
            View.Button(text = c "Poke3", command = c (fun () -> dispatch AnimationPoked3))
            View.Button(text = c "Main page",
               cornerRadius = c 5,
               command = c (fun () -> dispatch (SetRootPageKind (Choice false))),
               horizontalOptions = c LayoutOptions.CenterAndExpand,
               verticalOptions = c LayoutOptions.End)
          ])

    let webCallSample model dispatch =

        View.ContentPage(
            View.StackLayout(
                children = cs [
                    View.Button(text = c "Get Data", command = c (fun () -> dispatch ReceiveData))
                    View.ActivityIndicator(isRunning=model.IsRunning)
                    View.Label(text = (model.WebCallData |> AVal.map (function Some v -> v | None -> "")))
                    mainPageButton dispatch
                ]
        ))

    let scrollViewSample (model: AdaptiveModel) dispatch =
        let scrollTrigger = 
            (model.IsScrollingWithFabulous, model.ScrollPosition, model.AnimatedScroll) |||> AVal.map3 (fun b (x, y) animate -> 
                if b then Fabulous.Trigger (x, y, animate) else Fabulous.Trigger.None)

        View.ContentPage(
            View.StackLayout(
                children = cs [
                    mainPageButton dispatch
                    View.Label(text = (model.IsScrolling |> AVal.map (sprintf "Is scrolling: %b")))
                    View.Button(text = c "Scroll to top", command = c (fun() -> dispatch (ScrollFabulous (0.0, 0.0, Animated))))
                    View.ScrollView(
                        ref = c scrollViewRef,
                        scrollTo= scrollTrigger,
                        scrolled = c (fun args -> dispatch (Scrolled (args.ScrollX, args.ScrollY))),
                        content = View.StackLayout(
                            children = cs [
                                yield View.Button(text = c "Scroll animated with Fabulous", command = c (fun() -> dispatch (ScrollFabulous (0.0, 750.0, Animated))))
                                yield View.Button(text = c "Scroll not animated with Fabulous", command = c (fun() -> dispatch (ScrollFabulous (0.0, 750.0, NotAnimated))))
                                yield View.Button(text = c "Scroll animated with Xamarin.Forms", command = c (fun() -> dispatch (ScrollXamarinForms (0.0, 750.0, Animated))))
                                yield View.Button(text = c "Scroll not animated with Xamarin.Forms", command = c (fun() -> dispatch (ScrollXamarinForms (0.0, 750.0, NotAnimated))))

                                for i = 0 to 75 do
                                    yield View.Label(text=c (sprintf "Item %i" i))
                            ]
                        )
                    )
                  ]
            ) 
        )

    let shellViewSample model dispatch =
            
        match Device.RuntimePlatform with
        | Device.iOS | Device.Android -> 

            View.Shell( title = c "TitleShell", 
              items = cs [
                View.ShellItem(
                  items = cs [
                    View.ShellSection(
                      items = cs [
                        View.ShellContent(content=View.ContentPage(content=mainPageButton dispatch, title = c "ContentpageTitle"))         
                      ])
                  ])
              ])
        | _ -> 
            View.ContentPage(
                View.StackLayout (cs [
                    mainPageButton dispatch
                    View.Label(text = c "Your Platform does not support Shell")
                  ]))

    let collectionViewSample model dispatch =
        match Device.RuntimePlatform with
        | Device.iOS | Device.Android -> 
            View.ContentPage(
              View.StackLayout (cs [
                    mainPageButton dispatch
                    // use Collectionview instead of listview 
                    View.CollectionView (cs [
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
                        View.Label(text = c "Person14")
                     ])
                ] ))

        | _ ->
            View.ContentPage(
              View.StackLayout (cs [
                  mainPageButton dispatch
                  View.Label(text = c "Your Platform does not support CollectionView")
                ]))

    let refreshViewSample model dispatch =
        View.ContentPage(
           View.StackLayout (cs [
            View.RefreshView(
                View.ScrollView(
                    View.BoxView(
                        height = c 150.,
                        width = c 150.,
                        color = (model.RefreshViewIsRefreshing |> AVal.map (fun b -> if b then Color.Red else Color.Blue))
                    )),
                isRefreshing = model.RefreshViewIsRefreshing,
                refreshing = c (fun () -> dispatch RefreshViewRefreshing)
            )
            mainPageButton dispatch
          ]))

    let skiaCanvasSample1 model dispatch = 
          View.ScrollingContentPage("SkiaCanvas", 
           [ 
            View.SKCanvasView(enableTouchEvents = c true, 
                paintSurface = c (fun args -> 
                    let info = args.Info
                    let surface = args.Surface
                    let canvas = surface.Canvas

                    canvas.Clear() 
                    use paint = new SKPaint(Style = SKPaintStyle.Stroke, Color = Color.Red.ToSKColor(), StrokeWidth = 25.0f)
                    canvas.DrawCircle(float32 (info.Width / 2), float32 (info.Height / 2), 100.0f, paint)
                ),
                horizontalOptions = c LayoutOptions.FillAndExpand, 
                verticalOptions = c LayoutOptions.FillAndExpand, 
                touch = c (fun args -> 
                    if args.InContact then
                        dispatch (SKSurfaceTouched args.Location)
                ))

            mainPageButton dispatch
           ])

    let skiaCanvasSample2 model dispatch = 
       View.ScrollingContentPage("SkiaCanvas #2", [ 

           View.SKCanvasView2(enableTouchEvents = c true, 
               shapes = alist { for p in model.SKPoints -> SKShape.Circle(p, 50.0f, 5.0f) },
               verticalOptions = c LayoutOptions.FillAndExpand, 
               horizontalOptions = c LayoutOptions.FillAndExpand, 
               touch = c (fun args -> 
                   if args.InContact then
                       dispatch (SKSurfaceTouched args.Location)
               ))
              
           mainPageButton dispatch
       ])
                   
    let skiaCanvasSampleProtect f model dispatch = 
        match Device.RuntimePlatform with
        | Device.Android  | Device.macOS   | Device.iOS  | Device.Tizen | Device.UWP   | Device.WPF -> 
          f model dispatch 
        | _ -> 
            View.ContentPage( 
                View.StackLayout (cs [
                    mainPageButton dispatch
                    View.Label(text = c "Your Platform does not support SkiaSharp")
                    View.Label(text = c "For GTK status see https://github.com/mono/SkiaSharp/issues/379")
                  ]))
                   

    let mapSamplesActual model dispatch = 
        let sample1 = View.Map(hasZoomEnabled = c true, hasScrollEnabled = c true)

        let sample2 = 
            let timbuktu = Position(16.7666, -3.0026)
            View.Map(hasZoomEnabled = c true, hasScrollEnabled = c true,
                     requestedRegion = c (MapSpan.FromCenterAndRadius(timbuktu, Distance.FromKilometers(1.0))))

        let sample3 = 
            let paris = Position(48.8566, 2.3522)
            let london = Position(51.5074, -0.1278)
            let calais = Position(50.9513, 1.8587)
            View.Map(hasZoomEnabled = c true, hasScrollEnabled = c true, 
                     pins = cs [ View.Pin(c paris, label = c "Paris", pinType = c PinType.Place)
                                 View.Pin(c london, label = c "London", pinType = c PinType.Place) ] ,
                     requestedRegion = c (MapSpan.FromCenterAndRadius(calais, Distance.FromKilometers(300.0))))

        View.ScrollingContentPage("Map Samples", 
          [ 
            yield View.Label (c "Note, may require setup to access maps, see ")
            yield View.Label (c "fsprojects.github.io/Fabulous/Fabulous.XamarinForms/views-maps.html")
            yield View.Label (c "")
            yield View.Label (c "Android - put your Google Maps API Key in AllControls\Droid\Properties\AndroidManifest.xml")
            for map in [ sample1; sample2; sample3] do
                yield map
                yield mainPageButton dispatch
          ])


    let mapSamples model dispatch = 
        match Device.RuntimePlatform with
        | Device.GTK  -> 
            View.ContentPage( 
                View.StackLayout (cs [
                    mainPageButton dispatch
                    View.Label(text = c "When last tested Xamarin.Forms.Maps on GTK does not work correctly")
                    View.Label(text = c "Please uncomment the code in AllControls.fs and try again")
                  ]))
        | _ -> 
            mapSamplesActual model dispatch
       
    let oxyPlotSamplesActual model dispatch = 
        let plotModelCos =
            let model = PlotModel(Title = "Example 1")
            model.Series.Add(new OxyPlot.Series.FunctionSeries(Math.Cos, 0.0, 10.0, 0.1, "cos(x)"))
            model

        let plotModelHeatMap =
            let model = PlotModel (Title = "Heatmap")
            model.Axes.Add(LinearColorAxis (Palette = OxyPalettes.Rainbow(100)))
            let singleData = [ for x in 0 .. 99 -> Math.Exp((-1.0 / 2.0) * Math.Pow(((double)x - 50.0) / 20.0, 2.0)) ]
            let data = Array2D.init 100 100 (fun x y -> singleData.[x] * singleData.[(y + 30) % 100] * 100.0)
            let heatMapSeries =
                HeatMapSeries(X0 = 0.0, X1 = 99.0, Y0 = 0.0, Y1 = 99.0, Interpolate = true,
                                RenderMethod = HeatMapRenderMethod.Bitmap, Data = data)
            model.Series.Add(heatMapSeries)
            model

        let plotModels = [ plotModelCos; plotModelHeatMap ]

        View.ScrollingContentPage("Plot Samples", 
            [ for m in plotModels do
                yield mainPageButton dispatch
                yield View.PlotView(c m,
                        horizontalOptions = c LayoutOptions.FillAndExpand, 
                        verticalOptions = c LayoutOptions.FillAndExpand) ])

    let oxyPlotSamples model dispatch = 
        match Device.RuntimePlatform with
        | Device.Android  | Device.iOS  -> 
            oxyPlotSamplesActual model dispatch 
        | _ -> 
            View.ContentPage( 
                View.StackLayout (cs [
                    mainPageButton dispatch
                    View.Label(text = c "OxyPlot for XamarinForms 1.0.0 does not support your platform")
                    View.Label(text = c "For status see https://github.com/oxyplot/oxyplot-xamarin")
                  ]))
       
    // let videoSamples model dispatch = 
    //     View.ScrollingContentPage("VideoManager Sample", [ 
    //         mainPageButton dispatch
    //         View.VideoView(
    //             source = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
    //             showControls = false,
    //             height = 500.,
    //             width = 200.) ])

    //let chachedImageSamplesActual model dispatch =
    //    View.ScrollingContentPage("CachedImage Sample", 
    //      [ 
    //        View.Label "Note, when last checked this did not work on Android"
    //        View.Label "However maybe the sample is not configured correctly"
    //        mainPageButton dispatch
    //        View.CachedImage(
    //            source = Path "http://loremflickr.com/600/600/nature?filename=simple.jpg",
    //            //loadingPlaceholder = Path "path/to/loading-placeholder.png",
    //            //errorPlaceholder = Path "path/to/error-placeholder.png",
    //            height = 600.,
    //            width = 600. ) 
    //      ])

    //let chachedImageSamples model dispatch =
        //match Device.RuntimePlatform with
        //| Device.Android  | Device.iOS  | Device.Tizen | Device.UWP | Device.macOS -> 
        //    chachedImageSamplesActual model dispatch
        //| _ -> 
            //View.ContentPage( 
                //View.StackLayout [
                  //  mainPageButton dispatch
                  //  View.Label(text = c "Theis version of FFImageLoading for XamarinForms does not support your platform")
                  //  View.Label(text = c "For status see https://github.com/luberda-molinet/FFImageLoading")
                  //])

    let view (model: AdaptiveModel) dispatch =
     aval {
       let! k = model.RootPageKind
       return
        match k with 
        | Choice showAbout ->  frontPage model showAbout dispatch
        | Carousel -> carouselPageSample model dispatch
        | Tabbed1 -> tabbedPageSamples1 model dispatch
        | Tabbed2 -> tabbedPageSamples2 model dispatch
        | Tabbed3 -> tabbedPageSamples3 model dispatch
        | Navigation -> navigationPageSample model dispatch
        | MasterDetail -> masterDetailPageSample model dispatch
        | InfiniteScrollList -> infiniteScrollListSample model dispatch
        | Animations ->  animationSamples model dispatch
        | WebCall -> webCallSample model dispatch
        | ScrollView -> scrollViewSample model dispatch
        | ShellView -> shellViewSample model dispatch
        | CollectionView -> collectionViewSample model dispatch
        | CarouselView -> carouselViewSample model dispatch
        | Effects -> viewEffectsSample dispatch
        | RefreshView -> refreshViewSample model dispatch
        | SkiaCanvas -> skiaCanvasSampleProtect skiaCanvasSample1 model dispatch
        | SkiaCanvas2 -> skiaCanvasSampleProtect skiaCanvasSample2 model dispatch
        | MapSamples -> mapSamples model dispatch
        //| VideoSamples -> videoSamples model dispatch
        //| CachedImageSamples -> chachedImageSamples model dispatch
        | OxyPlotSamples -> oxyPlotSamples model dispatch
   }
    
type App () as app = 
    inherit Application ()
    do app.Resources.Add(Xamarin.Forms.StyleSheets.StyleSheet.FromAssemblyResource(System.Reflection.Assembly.GetExecutingAssembly(), "AllControls.styles.css"))
    
    let runner = 
        Program.mkProgram App.init App.update App.ainit App.adelta App.view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app

    member __.Program = runner
