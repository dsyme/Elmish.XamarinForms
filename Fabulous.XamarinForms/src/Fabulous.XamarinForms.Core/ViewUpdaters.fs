// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open System.Collections.ObjectModel
open System.Collections.Generic
open Xamarin.Forms
open Xamarin.Forms.StyleSheets
open FSharp.Data.Adaptive
open System.Windows.Input


[<AutoOpen>]
module ViewUpdaters =
    /// Update a control given the previous and new view elements
    let inline updateChild (token: AdaptiveToken) (newChild: ViewElement) targetChild = 
        // TODO make this a ViewElementUpdater?
        newChild.Updater token targetChild

    /// Incremental list maintenance: given a collection, and a previous version of that collection, perform
    /// a reduced number of clear/add/remove/insert operations
    let updateCollectionGeneric
           (coll: 'T alist) 
           (create: AdaptiveToken -> 'T -> 'TargetT)  // create a target element in the collection
           (attach: AdaptiveToken -> 'T -> 'TargetT -> unit) // adjust attached properties
           (canReuse: 'T -> 'T -> bool) // Used to check if reuse is possible
           (update: AdaptiveToken -> 'T -> 'TargetT -> unit) // Incremental element-wise update, only if element reuse is allowed
        =
      let mutable children : IndexList<(AdaptiveToken -> unit)>  = IndexList.empty
      let mutable subNodes : IndexList<'T>  = IndexList.empty
      let mutable dirtyInner = System.Collections.Generic.HashSet<(AdaptiveToken -> unit)>()
      let reader = coll.GetReader()
      (fun token (targetColl: IList<'TargetT>) -> 
        let changes = reader.GetChanges(token) 
        for (idx, op) in IndexListDelta.toSeq changes do
            match op with
            | Set node ->
                let insert (element : 'T) = 
                    let (_, s, r) = IndexList.neighbours idx subNodes

                    match s with
                    | Some(si, _) ->
                        match IndexList.tryGetPosition si subNodes with
                        | Some i -> targetColl.[i] <- unbox element
                        | None -> failwith "inconsistent"
                    | None ->
                        match r with
                        | Some (ri, _) ->
                            match IndexList.tryGetPosition ri children with
                            | Some i -> targetColl.Insert(i, unbox element)
                            | None -> failwith "inconsistent"
                        | None ->
                            // no right => last
                            targetColl.Add(unbox element)

                    subNodes <- IndexList.set idx element subNodes
                    { new System.IDisposable with
                        member x.Dispose() =   
                            match IndexList.tryGetPosition idx subNodes with
                            | Some i -> 
                                targetColl.RemoveAt i
                                subNodes <- IndexList.remove idx subNodes
                            | None -> 
                                ()
                                
                    }

                let u = (fun _ -> ()) //NodeUpdater(node, insert)
                children <- IndexList.set idx u children
                dirtyInner.Add u |> ignore

            | Remove ->
                match IndexList.tryRemove idx children with
                | Some (c, rest) ->
                    dirtyInner.Remove c |> ignore
                    //c.Destroy()
                    children <- rest
                | None ->
                    ()
        )
(*
 for (idx, change) in changes.ToList() do
           IndexList.tryGetPosition idx
        idx. IndexListDelta.
        if (coll = null || coll.Length = 0) then
            targetColl.Clear()
        else
            // Remove the excess targetColl
            while (targetColl.Count > coll.Length) do
                targetColl.RemoveAt (targetColl.Count - 1)

            // Count the existing targetColl
            // Unused variable n' introduced as a temporary workaround for https://github.com/fsprojects/Fabulous/issues/343
            let _ = targetColl.Count
            let n = targetColl.Count

            // Adjust the existing targetColl and create the new targetColl
            for i in 0 .. coll.Length-1 do
                let newChild = coll.[i]
                let prevChildOpt = match prevCollOpt with ValueNone -> ValueNone | ValueSome coll when i < n -> ValueSome coll.[i] | _ -> ValueNone
                let prevChildOpt, targetChild = 
                    if (match prevChildOpt with ValueNone -> true | ValueSome prevChild -> not (identical prevChild newChild)) then
                        let mustCreate = (i >= n || match prevChildOpt with ValueNone -> true | ValueSome prevChild -> not (canReuse prevChild newChild))
                        if mustCreate then
                            let targetChild = create newChild
                            if i >= n then
                                targetColl.Insert(i, targetChild)
                            else
                                targetColl.[i] <- targetChild
                            ValueNone, targetChild
                        else
                            let targetChild = targetColl.[i]
                            update prevChildOpt.Value newChild targetChild
                            prevChildOpt, targetChild
                    else
                        prevChildOpt, targetColl.[i]
                attach prevChildOpt newChild targetChild
*)

    /// Update the attached properties for each item in an already updated collection
    let updateAttachedPropertiesForCollection
           (coll: 'T alist)
           (attach: _ -> 'T -> 'TargetT -> unit) =
        let mutable children : IndexList<(AdaptiveToken -> unit)>  = IndexList.empty
        let mutable subNodes : IndexList<'T>  = IndexList.empty
        let mutable dirtyInner = System.Collections.Generic.HashSet<(AdaptiveToken -> unit)>()
        let reader = coll.GetReader()
        fun token (targetColl: IList<'TargetT>) ->
            ()
            //for i in 0 .. coll.Length-1 do
            //    let targetChild = targetColl.[i]
            //    let newChild = coll.[i]
            //    let prevChildOpt = match prevCollOpt with ValueNone -> ValueNone | ValueSome coll when i < coll.Length -> ValueSome coll.[i] | _ -> ValueNone
            //    attach prevChildOpt newChild targetChild

    /// Update the attached properties for each item in Layout<T>.Children
    let updateAttachedPropertiesForLayoutOfT coll attach =
        let updater = updateAttachedPropertiesForCollection coll attach
        fun token (target: Xamarin.Forms.Layout<'T>) -> updater token target.Children
                    
    /// Update the items in a ItemsView control, given previous and current view elements
    let updateItemsViewItems (coll: ViewElement alist) = 
        let updater = updateCollectionGeneric coll (fun token elem -> ViewElementHolder(elem)) (fun _ _ _ -> ()) ViewHelpers.canReuseView (fun token curr holder -> holder.ViewElement <- curr)
        fun token (target: Xamarin.Forms.ItemsView) ->
            let targetColl = 
                match target.ItemsSource with 
                | :? ObservableCollection<ViewElementHolder> as oc -> oc
                | _ -> 
                    let oc = ObservableCollection<ViewElementHolder>()
                    target.ItemsSource <- oc
                    oc
            updater token targetColl
                    
    /// Update the items in a ItemsView<'T> control, given previous and current view elements
    let updateItemsViewOfTItems<'T when 'T :> Xamarin.Forms.BindableObject> (coll: ViewElement alist) =
        let updater = updateCollectionGeneric coll (fun token elem -> ViewElementHolder(elem)) (fun _ _ _ -> ()) ViewHelpers.canReuseView (fun token curr holder -> holder.ViewElement <- curr)
        fun token (target: Xamarin.Forms.ItemsView<'T>) ->
            let targetColl = 
                match target.ItemsSource with 
                | :? ObservableCollection<ViewElementHolder> as oc -> oc
                | _ -> 
                    let oc = ObservableCollection<ViewElementHolder>()
                    target.ItemsSource <- oc
                    oc
            updater token targetColl
        
    /// Update the items in a SearchHandler control, given previous and current view elements
    let updateSearchHandlerItems (coll: ViewElement alist) = 
        let updater = updateCollectionGeneric coll (fun token elem -> ViewElementHolder(elem)) (fun _ _ _ -> ()) ViewHelpers.canReuseView (fun token curr holder -> holder.ViewElement <- curr)
        fun token (target: Xamarin.Forms.SearchHandler) ->
            let targetColl = 
                match target.ItemsSource with 
                | :? ObservableCollection<ViewElementHolder> as oc -> oc
                | _ -> 
                    let oc = ObservableCollection<ViewElementHolder>()
                    target.ItemsSource <- oc
                    oc
            updater token targetColl
        
    let private updateViewElementHolderGroup (currShortName: string, currKey, currColl: ViewElement alist) =
        let updater = updateCollectionGeneric currColl (fun token elem -> ViewElementHolder(elem)) (fun _ _ _ -> ()) ViewHelpers.canReuseView (fun token curr target -> target.ViewElement <- curr) 
        fun token (target: ViewElementHolderGroup) ->
            target.ShortName <- currShortName
            target.ViewElement <- currKey
            updater token target

(*
    /// Update the items in a GroupedListView control, given previous and current view elements
    let updateListViewGroupedItems (coll: (string * ViewElement * ViewElement alist) alist) (target: Xamarin.Forms.ListView) = 
        let updater = updateCollectionGeneric coll (fun token elem -> ViewElementHolderGroup(elem)) (fun _ _ _ -> ()) (fun (_, prevKey, _) (_, currKey, _) -> ViewHelpers.canReuseView prevKey currKey) updateViewElementHolderGroup
        fun token (target: Xamarin.Forms.ListView) ->
            let targetColl = 
                match target.ItemsSource with 
                | :? ObservableCollection<ViewElementHolderGroup> as oc -> oc
                | _ -> 
                    let oc = ObservableCollection<ViewElementHolderGroup>()
                    target.ItemsSource <- oc
                    oc
            updater token targetColl
                

    /// Update the ShowJumpList property of a GroupedListView control, given previous and current view elements
    let updateListViewGroupedShowJumpList token (prevOpt: bool voption) (currOpt: bool voption) (target: Xamarin.Forms.ListView) =
        let updateTarget enableJumpList = target.GroupShortNameBinding <- (if enableJumpList then new Binding("ShortName") else null)

        match (prevOpt, currOpt) with
        | ValueNone, ValueSome curr -> updateTarget curr
        | ValueSome prev, ValueSome curr when prev <> curr -> updateTarget curr
        | ValueSome _ -> target.GroupShortNameBinding <- null
        | _, _ -> ()
*)

    /// Update the items of a TableSectionBase<'T> control, given previous and current view elements
    let updateTableSectionBaseOfTItems<'T when 'T :> Xamarin.Forms.BindableObject> (coll: ViewElement alist) _attach =
        let updater = updateCollectionGeneric coll (fun token desc -> desc.Create(token) :?> 'T) (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild
        fun token (target: Xamarin.Forms.TableSectionBase<'T>) ->
            updater token target

    let updateResources (coll: (string * obj) alist) =
        fun token (target: Xamarin.Forms.VisualElement) ->
            // TODO - Resources
            ()
        (*
    /// Update the resources of a control, given previous and current view elements describing the resources
    let updateResources (prevCollOpt: (string * obj)[] voption) (coll: (string * obj)[] voption) (target: Xamarin.Forms.VisualElement) = 
        match prevCollOpt, coll with 
        | ValueNone, ValueNone -> ()
        | ValueSome prevColl, ValueSome newColl when identical prevColl newColl -> ()
        | _, ValueNone -> target.Resources.Clear()
        | _, ValueSome coll ->
            let targetColl = target.Resources
            if (coll = null || coll.Length = 0) then
                targetColl.Clear()
            else
                for (key, newChild) in coll do 
                    if targetColl.ContainsKey(key) then 
                        let prevChildOpt = 
                            match prevCollOpt with 
                            | ValueNone -> ValueNone 
                            | ValueSome prevColl -> 
                                match prevColl |> Array.tryFind(fun (prevKey, _) -> key = prevKey) with 
                                | Some (_, prevChild) -> ValueSome prevChild
                                | None -> ValueNone
                        if (match prevChildOpt with ValueNone -> true | ValueSome prevChild -> not (identical prevChild newChild)) then
                            targetColl.Add(key, newChild)                            
                        else
                            targetColl.[key] <- newChild
                    else
                        targetColl.Remove(key) |> ignore
                for (KeyValue(key, _newChild)) in targetColl do 
                   if not (coll |> Array.exists(fun (key2, _v2) -> key = key2)) then 
                       targetColl.Remove(key) |> ignore
*)

    /// Update the style sheets of a control, given previous and current view elements describing them
    // Note, style sheets can't be removed
    // Note, style sheets are compared by object identity
    let updateStyleSheets (coll: StyleSheet alist) = 
        fun token (target: Xamarin.Forms.VisualElement) ->
            // TODO 
            ()
(*
         match prevCollOpt, coll with 
        | ValueNone, ValueNone -> ()
        | ValueSome prevColl, ValueSome newColl when identical prevColl newColl -> ()
        | _, ValueNone -> target.Resources.Clear()
        | _, ValueSome coll ->
            let targetColl = target.Resources
            if (coll = null || coll.Length = 0) then
                targetColl.Clear()
            else
                for styleSheet in coll do 
                    let prevChildOpt = 
                        match prevCollOpt with 
                        | ValueNone -> None 
                        | ValueSome prevColl -> prevColl |> Array.tryFind(fun prevStyleSheet -> identical styleSheet prevStyleSheet)
                    match prevChildOpt with 
                    | None -> targetColl.Add(styleSheet)                            
                    | Some _ -> ()
                match prevCollOpt with 
                | ValueNone -> ()
                | ValueSome prevColl -> 
                    for prevStyleSheet in prevColl do 
                        let childOpt = 
                            match prevCollOpt with 
                            | ValueNone -> None 
                            | ValueSome prevColl -> prevColl |> Array.tryFind(fun styleSheet -> identical styleSheet prevStyleSheet)
                        match childOpt with 
                        | None -> 
                            eprintfn "**** WARNING: style sheets may not be removed, and are compared by object identity, so should be created independently of your update or view functions ****"
                        | Some _ -> ()
*)

    /// Update the styles of a control, given previous and current view elements describing them
    // Note, styles can't be removed
    // Note, styles are compared by object identity
    let updateStyles (coll: Style alist) = 
        fun token (target: Xamarin.Forms.VisualElement) ->
            // TODO
            ()
(*
     let targetColl = target.Resources
            if (coll = null || coll.Length = 0) then
                targetColl.Clear()
            else
                for styleSheet in coll do 
                    let prevChildOpt = 
                        match prevCollOpt with 
                        | ValueNone -> None 
                        | ValueSome prevColl -> prevColl |> Array.tryFind(fun prevStyleSheet -> identical styleSheet prevStyleSheet)
                    match prevChildOpt with 
                    | None -> targetColl.Add(styleSheet)                            
                    | Some _ -> ()
                match prevCollOpt with 
                | ValueNone -> ()
                | ValueSome prevColl -> 
                    for prevStyle in prevColl do 
                        let childOpt = 
                            match prevCollOpt with 
                            | ValueNone -> None 
                            | ValueSome prevColl -> prevColl |> Array.tryFind(fun style-> identical style prevStyle)
                        match childOpt with 
                        | None -> 
                            eprintfn "**** WARNING: styles may not be removed, and are compared by object identity. They should be created independently of your update or view functions ****"
                        | Some _ -> ()
                        *)
    /// Incremental NavigationPage maintenance: push/pop the right pages
    let updateNavigationPages (coll: ViewElement alist) _attach =
       (fun token target -> ())
       (*
            let create token (desc: ViewElement) = (desc.Creator(token ) :?> Page)
            if (coll = null || coll.Length = 0) then
                failwith "Error while updating NavigationPage pages: the pages collection should never be empty for a NavigationPage"
            else
                // Count the existing pages
                let prevCount = target.Pages |> Seq.length
                let newCount = coll.Length
                printfn "Updating NavigationPage, prevCount = %d, newCount = %d" prevCount newCount

                // Remove the excess pages
                if newCount = 1 && prevCount > 1 then 
                    printfn "Updating NavigationPage --> PopToRootAsync" 
                    target.PopToRootAsync() |> ignore
                elif prevCount > newCount then
                    for i in prevCount - 1 .. -1 .. newCount do 
                        printfn "PopAsync, page number %d" i
                        target.PopAsync () |> ignore
                
                let n = min prevCount newCount
                // Push and/or adjust pages
                for i in 0 .. newCount-1 do
                    let newChild = coll.[i]
                    let prevChildOpt = match prevCollOpt with ValueNone -> ValueNone | ValueSome coll when i < coll.Length && i < n -> ValueSome coll.[i] | _ -> ValueNone
                    let prevChildOpt, targetChild = 
                        if (match prevChildOpt with ValueNone -> true | ValueSome prevChild -> not (identical prevChild newChild)) then
                            let mustCreate = (i >= n || match prevChildOpt with ValueNone -> true | ValueSome prevChild -> not (ViewHelpers.canReuseView prevChild newChild))
                            if mustCreate then
                                //printfn "Creating child %d, prevChildOpt = %A, newChild = %A" i prevChildOpt newChild
                                let targetChild = create newChild
                                if i >= n then
                                    printfn "PushAsync, page number %d" i
                                    target.PushAsync(targetChild) |> ignore
                                else
                                    failwith "Error while updating NavigationPage pages: can't change type of one of the pages in the navigation chain during navigation"
                                ValueNone, targetChild
                            else
                                printfn "Adjust page number %d" i
                                let targetChild = target.Pages |> Seq.item i
                                updateChild token prevChildOpt.Value newChild targetChild
                                prevChildOpt, targetChild
                        else
                            //printfn "Skipping child %d" i
                            let targetChild = target.Pages |> Seq.item i
                            prevChildOpt, targetChild
                    attach prevChildOpt newChild targetChild
                    *)

    /// Update a value if it has changed
    let inline valueUpdater (value: aval<_>) f = 
        let mutable prevOpt = ValueNone
        (fun token target ->
            let curr = value.GetValue(token)
            f target prevOpt curr
            prevOpt <- ValueSome curr)

    /// Update a ViewELement
    let creationUpdater f = 
        let mutable created = false
        (fun token target ->
            f token target created
            created <- true)

    /// Update the OnSizeAllocated callback of a control, given previous and current values
    let updateOnSizeAllocated (value: aval<_>) = 
        valueUpdater value (fun (target: obj) prevOpt curr ->
            let target = (target :?> CustomContentPage)
            match prevOpt with ValueNone -> () | ValueSome f -> target.SizeAllocated.RemoveHandler(f)
            target.SizeAllocated.AddHandler(curr))
        
    /// Converts an F# function to a Xamarin.Forms ICommand
    let makeCommand f =
        let ev = Event<_,_>()
        { new ICommand with
            member __.add_CanExecuteChanged h = ev.Publish.AddHandler h
            member __.remove_CanExecuteChanged h = ev.Publish.RemoveHandler h
            member __.CanExecute _ = true
            member __.Execute _ = f() }

    /// Converts an F# function to a Xamarin.Forms ICommand, with a CanExecute value
    let makeCommandCanExecute f canExecute =
        let ev = Event<_,_>()
        { new ICommand with
            member __.add_CanExecuteChanged h = ev.Publish.AddHandler h
            member __.remove_CanExecuteChanged h = ev.Publish.RemoveHandler h
            member __.CanExecute _ = canExecute
            member __.Execute _ = f() }

    /// Update the Command and CanExecute properties of a control, given previous and current values
    let inline updateCommand (canExecute: aval<bool> option) argTransform setter (command: aval<_>) =
        match canExecute with 
        | None -> 
            let mutable prevCommandValueOpt = ValueNone
            fun token (target: 'TTarget) -> 
                let commandValue = command.GetValue(token)
                match prevCommandValueOpt with 
                | ValueSome prevf when identical prevf commandValue -> ()
                | _ -> setter target (makeCommand (fun () -> commandValue (argTransform target)))
                prevCommandValueOpt <- ValueSome commandValue
        | Some canExecute -> 
            let mutable prevCommandValueOpt = ValueNone
            let mutable prevCanExecuteValueOpt = ValueNone
            fun token target -> 
                let commandValue = command.GetValue(token)
                let canExecuteValue = canExecute.GetValue(token)
                match prevCommandValueOpt, prevCanExecuteValueOpt with 
                | ValueSome prevf, ValueSome prevx when identical prevf commandValue && prevx = canExecuteValue -> ()
                | ValueSome prevf, ValueNone when identical prevf commandValue -> ()
                | _ -> setter target (makeCommandCanExecute (fun () -> commandValue (argTransform target)) canExecuteValue)
                prevCommandValueOpt <- ValueSome commandValue
                prevCanExecuteValueOpt <- ValueSome canExecuteValue

    /// Update the CurrentPage of a control, given previous and current values
    let updateMultiPageOfTCurrentPage<'a when 'a :> Xamarin.Forms.Page> (value: aval<_>) = 
        valueUpdater value (fun (target: Xamarin.Forms.MultiPage<'a>) prevOpt curr ->
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> target.CurrentPage <- target.Children.[curr])

    /// Update the Minium and Maximum values of a slider, given previous and current values
    let updateSliderMinimumMaximum (value: aval<_>) =
        valueUpdater value (fun (target: obj) prevOpt curr -> 
            let control = target :?> Xamarin.Forms.Slider
            let defaultValue = (0.0, 1.0)
            let updateFunc (_, prevMaximum) (newMinimum, newMaximum) =
                if newMinimum > prevMaximum then
                    control.Maximum <- newMaximum
                    control.Minimum <- newMinimum
                else
                    control.Minimum <- newMinimum
                    control.Maximum <- newMaximum

            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | ValueSome prev -> updateFunc prev curr
            | ValueNone -> updateFunc defaultValue curr)

    /// Update the Minimum and Maximum values of a stepper, given previous and current values
    let updateStepperMinimumMaximum (value: aval<_>) =
        valueUpdater value (fun (target: obj) prevOpt curr ->
            let control = target :?> Xamarin.Forms.Stepper
            let defaultValue = (0.0, 1.0)
            let updateFunc (_, prevMaximum) (newMinimum, newMaximum) =
                if newMinimum > prevMaximum then
                    control.Maximum <- newMaximum
                    control.Minimum <- newMinimum
                else
                    control.Minimum <- newMinimum
                    control.Maximum <- newMaximum

            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | ValueSome prev -> updateFunc prev curr
            | ValueNone -> updateFunc defaultValue curr)

    /// Update the AcceleratorProperty of a MenuItem, given previous and current Accelerator
    let updateMenuItemAccelerator (value: aval<_>) =
        valueUpdater value (fun (target: Xamarin.Forms.MenuItem) prevOpt curr ->
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Xamarin.Forms.MenuItem.SetAccelerator(target, Xamarin.Forms.Accelerator.FromString curr))

    /// Update the items of a Shell, given previous and current view elements
    let updateShellItems (coll: ViewElement alist) _attach =
        let create token (desc: ViewElement) =
            match desc.Create(token) with
            | :? ShellContent as shellContent -> ShellItem.op_Implicit shellContent
            | :? TemplatedPage as templatedPage -> ShellItem.op_Implicit templatedPage
            | :? ShellSection as shellSection -> ShellItem.op_Implicit shellSection
            | :? MenuItem as menuItem -> ShellItem.op_Implicit menuItem
            | :? ShellItem as shellItem -> shellItem
            | child -> failwithf "%s is not compatible with the type ShellItem" (child.GetType().Name)

        let update token (currViewElement: ViewElement) (target: ShellItem) =
            let realTarget =
                match currViewElement.TargetType with
                | t when t = typeof<ShellContent> -> target.Items.[0].Items.[0] :> Element
                | t when t = typeof<TemplatedPage> -> target.Items.[0].Items.[0] :> Element
                | t when t = typeof<ShellSection> -> target.Items.[0] :> Element
                | t when t = typeof<MenuItem> -> target.GetType().GetProperty("MenuItem").GetValue(target) :?> Element // MenuShellItem is marked as internal
                | _ -> target :> Element
            updateChild token currViewElement realTarget

        let updater = updateCollectionGeneric coll create (fun _ _ _ -> ()) (fun _ _ -> true) update
        fun token (target: Xamarin.Forms.Shell) ->
            updater token target.Items 
        
    /// Update the menu items of a ShellContent, given previous and current view elements
    let updateShellContentMenuItems (coll: ViewElement alist) =
        let create token (desc: ViewElement) =
            desc.Create(token) :?> Xamarin.Forms.MenuItem

        let updater = updateCollectionGeneric coll create (fun _ _ _ -> ()) (fun _ _ -> true) updateChild
        fun token (target: Xamarin.Forms.ShellContent) ->
            updater token target.MenuItems 

    /// Update the items of a ShellItem, given previous and current view elements
    let updateShellItemItems (coll: ViewElement alist) _attach =
        let create token (desc: ViewElement) =
            match desc.Create(token) with
            | :? ShellContent as shellContent -> ShellSection.op_Implicit shellContent
            | :? TemplatedPage as templatedPage -> ShellSection.op_Implicit templatedPage
            | :? ShellSection as shellSection -> shellSection
            | child -> failwithf "%s is not compatible with the type ShellSection" (child.GetType().Name)

        let update token (currViewElement: ViewElement) (target: ShellSection) =
            let realTarget =
                match currViewElement.TargetType with
                | t when t = typeof<ShellContent> -> target.Items.[0] :> BaseShellItem
                | t when t = typeof<TemplatedPage> -> target.Items.[0] :> BaseShellItem
                | _ -> target :> BaseShellItem
            updateChild token currViewElement realTarget

        let updater = updateCollectionGeneric coll create (fun _ _ _ -> ()) (fun _ _ -> true) update
        fun token (target: Xamarin.Forms.ShellItem) ->
            updater token target.Items

    /// Update the items of a ShellSection, given previous and current view elements
    let updateShellSectionItems (coll: ViewElement alist) _attach =
        let create token (desc: ViewElement) =
            desc.Create(token) :?> Xamarin.Forms.ShellContent

        let updater = updateCollectionGeneric coll create (fun _ _ _ -> ()) (fun _ _ -> true) updateChild
        fun token (target: Xamarin.Forms.ShellSection) ->
            updater token target.Items

    /// Trigger ScrollView.ScrollToAsync if needed, given the current values
    ///
    /// TODO: this re-executes repeatedly
    let triggerScrollToAsync (value: aval<(float * float * AnimationKind)>) =
        fun token (target: Xamarin.Forms.ScrollView) ->
            let (x, y, animationKind) = value.GetValue(token)
            if x <> target.ScrollX || y <> target.ScrollY then
                let animated =
                    match animationKind with
                    | Animated -> true
                    | NotAnimated -> false
                target.ScrollToAsync(x, y, animated) |> ignore

    /// Trigger ItemsView.ScrollTo if needed, given the current values
    ///
    /// TODO: this re-executes repeatedly
    let triggerScrollTo (value: aval<(obj * obj * ScrollToPosition * AnimationKind)>) =
        fun token (target: Xamarin.Forms.ItemsView) ->
            let (x, y, scrollToPosition, animationKind) = value.GetValue(token)
            let animated =
                match animationKind with
                | Animated -> true
                | NotAnimated -> false
            target.ScrollTo(x,y, scrollToPosition, animated)

    /// Trigger Shell.GoToAsync if needed, given the current values
    ///
    /// TODO: this re-executes repeatedly
    let triggerShellGoToAsync (curr: aval<(ShellNavigationState * AnimationKind)>) =
        fun token (target: Xamarin.Forms.Shell) ->
            let (navigationState, animationKind) = curr.GetValue(token)
            let animated =
                match animationKind with
                | Animated -> true
                | NotAnimated -> false
            target.GoToAsync(navigationState, animated) |> ignore

    let updatePageShellSearchHandler (element: ViewElement) =
        creationUpdater (fun token target created ->
            // TODO: this will do repeated updates
            match created with
            | false -> 
                let handler = element.Create(token) :?> Xamarin.Forms.SearchHandler
                Shell.SetSearchHandler(target, handler)
            | true -> 
                let handler = Shell.GetSearchHandler(target)
                // TODO make this a ViewElementUpdater?
                element.Updater token (box handler))

    let updateShellBackgroundColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetBackgroundColor(target, curr))
            //| ValueSome _ -> Shell.SetBackgroundColor(target, Color.Default)

    let updateShellForegroundColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetForegroundColor(target, curr))
            //| ValueSome _ -> Shell.SetForegroundColor(target, Color.Default)

    let updateShellTitleColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetTitleColor(target, curr))
            //| ValueSome _ -> Shell.SetTitleColor(target, Color.Default)

    let updateShellDisabledColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetDisabledColor(target, curr))
            //| ValueSome _ -> Shell.SetDisabledColor(target, Color.Default)

    let updateShellUnselectedColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetUnselectedColor(target, curr))
            //| ValueSome _ -> Shell.SetUnselectedColor(target, Color.Default)

    let updateShellTabBarBackgroundColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetTabBarBackgroundColor(target, curr))
            //| ValueSome _ -> Shell.SetTabBarBackgroundColor(target, Color.Default)

    let updateShellTabBarForegroundColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetTabBarForegroundColor(target, curr))

    let updateShellBackButtonBehavior (element: ViewElement) =
        creationUpdater (fun token target created ->
            // TODO: this will do repeated updates
            match created with
            | false -> 
                let behavior = element.Create(token) :?> BackButtonBehavior
                Shell.SetBackButtonBehavior(target, behavior)
            | true -> 
                let behavior = Shell.GetBackButtonBehavior(target)
                // TODO make this a ViewElementUpdater?
                element.Updater token (box behavior))

    let updateShellTitleView (element: ViewElement) =
        creationUpdater (fun token target created ->
            // TODO: this will do repeated updates
            match created with
            | false ->
                let view = element.Create(token) :?> View
                Shell.SetTitleView(target, view)
            | true -> 
                let view = Shell.GetTitleView(target)
                // TODO make this a ViewElementUpdater?
                element.Updater token (box view))

    let updateShellFlyoutBehavior (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetFlyoutBehavior(target, curr))
            //| ValueSome _ -> Shell.SetFlyoutBehavior(target, FlyoutBehavior.Flyout)

    let updateShellTabBarIsVisible (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetTabBarIsVisible(target, curr))
            //| ValueSome _ -> Shell.SetTabBarIsVisible(target, true)

    let updateShellNavBarIsVisible (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetNavBarIsVisible(target, curr))
            //| ValueSome _ -> Shell.SetNavBarIsVisible(target, true)

    let updateShellTabBarDisabledColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetTabBarDisabledColor(target, curr))
            //| ValueSome _ -> Shell.SetTabBarDisabledColor(target, Xamarin.Forms.Color.Default)

    let updateShellTabBarTitleColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetTabBarTitleColor(target, curr))
            //| ValueSome _ -> Shell.SetTabBarTitleColor(target, Xamarin.Forms.Color.Default)

    let updateShellTabBarUnselectedColor (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> Shell.SetTabBarUnselectedColor(target, curr))
            //| ValueSome _ -> Shell.SetTabBarUnselectedColor(target, Xamarin.Forms.Color.Default)

    let updateNavigationPageHasNavigationBar (value: aval<_>) =
        valueUpdater value (fun target prevOpt curr -> 
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | _ -> NavigationPage.SetHasNavigationBar(target, curr))
            //| ValueSome _ -> NavigationPage.SetHasNavigationBar(target, true)

    let updateShellContentContentTemplate (curr : ViewElement) =
        creationUpdater (fun token (target : Xamarin.Forms.ShellContent) created ->
            match created with 
            | false -> 
                target.ContentTemplate <- DirectViewElementDataTemplate(curr)
            | true -> ())
            // TODO: varying templates
            //| ValueSome prev ->
            //    target.ContentTemplate <- DirectViewElementDataTemplate(curr)
            //    let realTarget = (target :> Xamarin.Forms.IShellContentController).Page
            //    if realTarget <> null then curr.Update(token, realTarget)            
            //| ValueSome _ -> target.ContentTemplate <- null
        
    let updatePageUseSafeArea (value: aval<bool>) =
        valueUpdater value (fun (target : Xamarin.Forms.Page) prevOpt curr -> 
            let setUseSafeArea newValue =
                    Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(
                        (target : Xamarin.Forms.Page).On<Xamarin.Forms.PlatformConfiguration.iOS>(),
                        newValue
                    ) |> ignore
        
            match prevOpt with
            | ValueSome prev when prev = curr -> ()
            | ValueNone -> ()
            | _ -> setUseSafeArea curr)

    let triggerWebViewReload (value: aval<bool>) =
        fun token (target: Xamarin.Forms.WebView) ->
            let curr = value.GetValue(token)
            if curr = true then target.Reload()
    
    let updateEntryCursorPosition (value: aval<int>) =
        fun token (target: Xamarin.Forms.Entry) ->
            let curr = value.GetValue(token)
            if target.CursorPosition <> curr then 
                target.CursorPosition <- value.GetValue(token)
    
    let updateEntrySelectionLength (value: aval<int>) =
        fun token (target: Xamarin.Forms.Entry) ->
            let curr = value.GetValue(token)
            if target.SelectionLength <> curr then 
                target.SelectionLength <- value.GetValue(token)
        
    let updateMenuChildren (currCollOpt: ViewElement alist) _attach =
        let updater = updateCollectionGeneric currCollOpt (fun _ -> failwith "updateMenuChildren - unexpected" (* target *) ) (fun _ _ _ -> ()) (fun _ _ -> true) updateChild
        fun token (target: Xamarin.Forms.Menu) ->
            updater token target
        
    let updateElementEffects (coll: ViewElement alist) _attach =
        let create token (viewElement: ViewElement) =
            match viewElement.Create(token) with
            | :? CustomEffect as customEffect -> Effect.Resolve(customEffect.Name)
            | effect -> effect :?> Xamarin.Forms.Effect
        let updater = updateCollectionGeneric coll create (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild
        fun token (target: Xamarin.Forms.Element) ->
            updater token target.Effects

    let updatePageToolbarItems (coll: ViewElement alist) _attach =
        let create token (viewElement: ViewElement) =
            viewElement.Create(token) :?> Xamarin.Forms.ToolbarItem
        
        let updater = updateCollectionGeneric coll create (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild
        fun token (target: Xamarin.Forms.Page) ->
            updater token target.ToolbarItems

    let updateElementMenu (value : ViewElement) =
        creationUpdater (fun token (target: Xamarin.Forms.Element) created ->
            match created with 
            | false -> Element.SetMenu(target, value.Create(token) :?> Menu)
            | true -> 
                let menu = Element.GetMenu(target)
                // TODO make this a ViewElementUpdater?
                value.Updater token (box menu))

