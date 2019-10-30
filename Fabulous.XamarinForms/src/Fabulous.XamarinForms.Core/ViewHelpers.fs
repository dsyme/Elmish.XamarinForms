// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open Xamarin.Forms
open System
open System.Collections.Concurrent
open System.Threading

[<AutoOpen>]
module ViewHelpers =
    /// Checks whether two objects are reference-equal
    let identical (x: 'T) (y:'T) = System.Object.ReferenceEquals(x, y)
    
    /// Checks whether an underlying control can be reused given the previous and new view elements
    let rec canReuseView (prevChild:ViewElement) (newChild:ViewElement) =
        if prevChild.TargetType = newChild.TargetType && canReuseAutomationId prevChild newChild then
            if newChild.TargetType.IsAssignableFrom(typeof<NavigationPage>) then
                canReuseNavigationPage prevChild newChild
            elif newChild.TargetType.IsAssignableFrom(typeof<CustomEffect>) then
                canReuseCustomEffect prevChild newChild
            else
                true
        else
            false

    /// Checks whether an underlying NavigationPage control can be reused given the previous and new view elements
    //
    // NavigationPage can be reused only if the pages don't change their type (added/removed pages don't prevent reuse)
    // E.g. If the first page switch from ContentPage to TabbedPage, the NavigationPage can't be reused.
    and internal canReuseNavigationPage (prevChild:ViewElement) (newChild:ViewElement) =
        let prevPages = prevChild.TryGetAttribute<ViewElement[]>("Pages")
        let newPages = newChild.TryGetAttribute<ViewElement[]>("Pages")

        match prevPages, newPages with
        | ValueSome prevPages, ValueSome newPages -> (prevPages, newPages) ||> Seq.forall2 canReuseView
        | _, _ -> true

    /// Checks whether the control can be reused given the previous and the new AutomationId.
    /// Xamarin.Forms can't change an already set AutomationId
    and internal canReuseAutomationId (prevChild: ViewElement) (newChild: ViewElement) =
        let prevAutomationId = prevChild.TryGetAttribute<string>("AutomationId")
        let newAutomationId = newChild.TryGetAttribute<string>("AutomationId")

        match prevAutomationId with
        | ValueSome _ when prevAutomationId <> newAutomationId -> false
        | _ -> true
        
    /// Checks whether the CustomEffect can be reused given the previous and the new Effect name
    /// The effect is instantiated by Effect.Resolve and can't be reused when asking for a new effect
    and internal canReuseCustomEffect (prevChild:ViewElement) (newChild:ViewElement) =
        let prevName = prevChild.TryGetAttribute<string>("Name")
        let newName = newChild.TryGetAttribute<string>("Name")

        match prevName with
        | ValueSome _ when prevName <> newName -> false
        | _ -> true
        
    /// Debounce multiple calls to a single function
    let debounce<'T> =
        let memoization = ConcurrentDictionary<obj, CancellationTokenSource>(HashIdentity.Structural)
        fun (timeout: int) (fn: 'T -> unit) value ->
            let key = fn.GetType()
            match memoization.TryGetValue(key) with
            | true, previousCts -> previousCts.Cancel()
            | _ -> ()

            let cts = new CancellationTokenSource()
            memoization.[key] <- cts

            Device.StartTimer(TimeSpan.FromMilliseconds(float timeout), (fun () ->
                match cts.IsCancellationRequested with
                | true -> ()
                | false ->
                    memoization.TryRemove(key) |> ignore
                    fn value
                false // Do not let the timer trigger a second time
            ))

    /// Looks for a view element with the given Automation ID in the view hierarchy.
    /// This function is not optimized for efficiency and may execute slowly.
    let rec tryFindViewElement automationId (element:ViewElement) =
        let elementAutomationId = element.TryGetAttribute<string>("AutomationId")
        match elementAutomationId with
        | ValueSome automationIdValue when automationIdValue = automationId -> Some element
        | _ ->
            let childElements =
                match element.TryGetAttribute<ViewElement>("Content") with
                | ValueSome content -> [| content |]
                | ValueNone ->
                    match element.TryGetAttribute<ViewElement[]>("Pages") with
                    | ValueSome pages -> pages
                    | ValueNone ->
                        match element.TryGetAttribute<ViewElement[]>("Children") with
                        | ValueNone -> [||]
                        | ValueSome children -> children

            childElements
            |> Seq.choose (tryFindViewElement automationId)
            |> Seq.tryHead
     
    /// Looks for a view element with the given Automation ID in the view hierarchy
    /// Throws an exception if no element is found
    let findViewElement automationId element =
        match tryFindViewElement automationId element with
        | None -> failwithf "No element with automation id '%s' found" automationId
        | Some viewElement -> viewElement

    let ContentsAttribKey = AttributeKey<(obj -> ViewElement)> "Stateful_Contents"

    let localStateTable = System.Runtime.CompilerServices.ConditionalWeakTable<obj, obj option>()

    type View with

        /// Describes an element in the view which uses localized mutable state unrelated to the model
        /// (and hence un-persisted), and can optionally access the underlying control. The 'init'
        /// function is called only when the underlying control is created (and each time it is re-created,
        /// if ever). The generated state object is associated with the underlying control.
        static member Stateful (init: (unit -> 'State), contents: 'State -> ViewElement, ?onCreate: ('State -> obj -> unit), ?onUpdate: ('State -> obj -> unit)) : _ when 'State : not struct =

            let attribs = AttributesBuilder(1)
            attribs.Add(ContentsAttribKey, (fun stateObj -> contents (unbox (stateObj))))

            // The create method
            let create () = 
                let state = init()
                let desc = contents state
                let item = desc.Create()
                localStateTable.Add(item, Some (box state))
                match onCreate with None -> () | Some f -> f state item
                item

            // The update method
            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: obj) = 
                let state = unbox<'State> ((snd (localStateTable.TryGetValue(target))).Value)
                let contents = source.TryGetAttributeKeyed(ContentsAttribKey).Value
                let realSource = contents state
                realSource.Update(prevOpt, source, target)
                match onUpdate with None -> () | Some f -> f state target

            // The element
            ViewElement.Create(create, update, attribs)

        static member OnCreate (contents : ViewElement, onCreate: (obj -> unit)) =
            View.Stateful (init = (fun () -> ()), contents = (fun _ -> contents), onCreate = (fun _ obj -> onCreate obj))

        static member WithInternalModel(init: (unit -> 'InternalModel), 
                                        update: ('InternalMessage -> 'InternalModel -> 'InternalModel), 
                                        view : ('InternalModel -> ('InternalMessage -> unit) -> ViewElement)) =
            let internalDispatch (state: 'InternalModel ref) msg = state.Value <- update msg state.Value
            View.Stateful (init = (fun () -> ref (init ())), contents = (fun state -> view state.Value (internalDispatch state)))

    // Keep a table to make sure we create a unique ViewElement for each external object
    let externalsTable = System.Runtime.CompilerServices.ConditionalWeakTable<obj, obj>()
    type View with

        /// Describes an element in the view implemented by an external object, e.g. an external
        /// Xamarin.Forms Page or View. The element must have a type appropriate for the place in
        /// the view where the object occurs.
        static member External (externalObj: 'T) : _ when 'T : not struct =

            match externalsTable.TryGetValue(externalObj) with 
            | true, v -> (v :?> ViewElement)
            | _ -> 
                let attribs = AttributesBuilder(0)
                let create () = box externalObj 
                let update (_prevOpt: ViewElement voption) (_source: ViewElement) (_target: obj) = ()
                let res = ViewElement(externalObj.GetType(), create, update, attribs)
                externalsTable.Add(externalObj, res)
                res
