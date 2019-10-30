// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open System.Collections.ObjectModel
open System.Collections.Generic
open Xamarin.Forms
open Xamarin.Forms.StyleSheets
open System.Windows.Input

[<AutoOpen>]
module ViewUpdaters =
    /// Update a control given the previous and new view elements
    let inline updateChild (prevChild:ViewElement) (newChild:ViewElement) targetChild = 
        newChild.UpdateIncremental(prevChild, targetChild)

    /// Incremental list maintenance: given a collection, and a previous version of that collection, perform
    /// a reduced number of clear/add/remove/insert operations
    let updateCollectionGeneric
           (prevCollOpt: 'T[] voption) 
           (collOpt: 'T[] voption) 
           (targetColl: IList<'TargetT>) 
           (create: 'T -> 'TargetT)
           (attach: 'T voption -> 'T -> 'TargetT -> unit) // adjust attached properties
           (canReuse : 'T -> 'T -> bool) // Used to check if reuse is possible
           (update: 'T -> 'T -> 'TargetT -> unit) // Incremental element-wise update, only if element reuse is allowed
        =
        match prevCollOpt, collOpt with 
        | ValueSome prevColl, ValueSome newColl when identical prevColl newColl -> ()
        | _, ValueNone -> targetColl.Clear()
        | _, ValueSome coll ->
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
                    
    /// Update the attached properties for each item in an already updated collection
    let updateAttachedPropertiesForCollection
           (prevCollOpt: 'T[] voption)
           (collOpt: 'T[] voption)
           (targetColl: IList<'TargetT>)
           (attach: 'T voption -> 'T -> 'TargetT -> unit) =
        match collOpt with
        | ValueNone -> ()
        | ValueSome coll when coll = null || coll.Length = 0 -> ()
        | ValueSome coll ->
            for i in 0 .. coll.Length-1 do
                let targetChild = targetColl.[i]
                let newChild = coll.[i]
                let prevChildOpt = match prevCollOpt with ValueNone -> ValueNone | ValueSome coll when i < coll.Length -> ValueSome coll.[i] | _ -> ValueNone
                attach prevChildOpt newChild targetChild
                
    /// Update the attached properties for each item in Layout<T>.Children
    let updateAttachedPropertiesForLayoutOfT prevCollOpt collOpt (target: Xamarin.Forms.Layout<'T>) attach =
        updateAttachedPropertiesForCollection prevCollOpt collOpt target.Children attach
                    
    /// Update the items in a ItemsView control, given previous and current view elements
    let updateItemsViewItems (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.ItemsView) = 
        let targetColl = 
            match target.ItemsSource with 
            | :? ObservableCollection<ViewElementHolder> as oc -> oc
            | _ -> 
                let oc = ObservableCollection<ViewElementHolder>()
                target.ItemsSource <- oc
                oc
        updateCollectionGeneric prevCollOpt collOpt targetColl ViewElementHolder (fun _ _ _ -> ()) ViewHelpers.canReuseView (fun _ curr holder -> holder.ViewElement <- curr)
                    
    /// Update the items in a ItemsView<'T> control, given previous and current view elements
    let updateItemsViewOfTItems<'T when 'T :> Xamarin.Forms.BindableObject> (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.ItemsView<'T>) = 
        let targetColl = 
            match target.ItemsSource with 
            | :? ObservableCollection<ViewElementHolder> as oc -> oc
            | _ -> 
                let oc = ObservableCollection<ViewElementHolder>()
                target.ItemsSource <- oc
                oc
        updateCollectionGeneric prevCollOpt collOpt targetColl ViewElementHolder (fun _ _ _ -> ()) ViewHelpers.canReuseView (fun _ curr holder -> holder.ViewElement <- curr)
        
    /// Update the items in a SearchHandler control, given previous and current view elements
    let updateSearchHandlerItems (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.SearchHandler) = 
        let targetColl = 
            match target.ItemsSource with 
            | :? ObservableCollection<ViewElementHolder> as oc -> oc
            | _ -> 
                let oc = ObservableCollection<ViewElementHolder>()
                target.ItemsSource <- oc
                oc
        updateCollectionGeneric prevCollOpt collOpt targetColl ViewElementHolder (fun _ _ _ -> ()) ViewHelpers.canReuseView (fun _ curr holder -> holder.ViewElement <- curr)
        
    let private updateViewElementHolderGroup (_prevShortName: string, _prevKey, prevColl: ViewElement[]) (currShortName: string, currKey, currColl: ViewElement[]) (target: ViewElementHolderGroup) =
        target.ShortName <- currShortName
        target.ViewElement <- currKey
        updateCollectionGeneric (ValueSome prevColl) (ValueSome currColl) target ViewElementHolder (fun _ _ _ -> ()) ViewHelpers.canReuseView (fun _ curr target -> target.ViewElement <- curr) 

    /// Update the items in a GroupedListView control, given previous and current view elements
    let updateListViewGroupedItems (prevCollOpt: (string * ViewElement * ViewElement[])[] voption) (collOpt: (string * ViewElement * ViewElement[])[] voption) (target: Xamarin.Forms.ListView) = 
        let targetColl = 
            match target.ItemsSource with 
            | :? ObservableCollection<ViewElementHolderGroup> as oc -> oc
            | _ -> 
                let oc = ObservableCollection<ViewElementHolderGroup>()
                target.ItemsSource <- oc
                oc
                
        updateCollectionGeneric prevCollOpt collOpt targetColl ViewElementHolderGroup (fun _ _ _ -> ()) (fun (_, prevKey, _) (_, currKey, _) -> ViewHelpers.canReuseView prevKey currKey) updateViewElementHolderGroup

    /// Update the ShowJumpList property of a GroupedListView control, given previous and current view elements
    let updateListViewGroupedShowJumpList (prevOpt: bool voption) (currOpt: bool voption) (target: Xamarin.Forms.ListView) =
        let updateTarget enableJumpList = target.GroupShortNameBinding <- (if enableJumpList then new Binding("ShortName") else null)

        match (prevOpt, currOpt) with
        | ValueNone, ValueSome curr -> updateTarget curr
        | ValueSome prev, ValueSome curr when prev <> curr -> updateTarget curr
        | ValueSome _, ValueNone -> target.GroupShortNameBinding <- null
        | _, _ -> ()

    /// Update the items of a TableSectionBase<'T> control, given previous and current view elements
    let updateTableSectionBaseOfTItems<'T when 'T :> Xamarin.Forms.BindableObject> (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.TableSectionBase<'T>) _ =
        let create (desc: ViewElement) =
            desc.Create() :?> 'T
        
        updateCollectionGeneric prevCollOpt collOpt target create (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild

    /// Update the resources of a control, given previous and current view elements describing the resources
    let updateResources (prevCollOpt: (string * obj) array voption) (collOpt: (string * obj) array voption) (target: Xamarin.Forms.VisualElement) = 
        match prevCollOpt, collOpt with 
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

    /// Update the style sheets of a control, given previous and current view elements describing them
    // Note, style sheets can't be removed
    // Note, style sheets are compared by object identity
    let updateStyleSheets (prevCollOpt: StyleSheet array voption) (collOpt: StyleSheet array voption) (target: Xamarin.Forms.VisualElement) = 
        match prevCollOpt, collOpt with 
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

    /// Update the styles of a control, given previous and current view elements describing them
    // Note, styles can't be removed
    // Note, styles are compared by object identity
    let updateStyles (prevCollOpt: Style array voption) (collOpt: Style array voption) (target: Xamarin.Forms.VisualElement) = 
        match prevCollOpt, collOpt with 
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
                    for prevStyle in prevColl do 
                        let childOpt = 
                            match prevCollOpt with 
                            | ValueNone -> None 
                            | ValueSome prevColl -> prevColl |> Array.tryFind(fun style-> identical style prevStyle)
                        match childOpt with 
                        | None -> 
                            eprintfn "**** WARNING: styles may not be removed, and are compared by object identity. They should be created independently of your update or view functions ****"
                        | Some _ -> ()

    /// Incremental NavigationPage maintenance: push/pop the right pages
    let updateNavigationPages (prevCollOpt: ViewElement[] voption)  (collOpt: ViewElement[] voption) (target: NavigationPage) attach =
        match prevCollOpt, collOpt with 
        | ValueSome prevColl, ValueSome newColl when identical prevColl newColl -> ()
        | _, ValueNone -> failwith "Error while updating NavigationPage pages: the pages collection should never be empty for a NavigationPage"
        | _, ValueSome coll ->
            let create (desc: ViewElement) = (desc.Create() :?> Page)
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
                                updateChild prevChildOpt.Value newChild targetChild
                                prevChildOpt, targetChild
                        else
                            //printfn "Skipping child %d" i
                            let targetChild = target.Pages |> Seq.item i
                            prevChildOpt, targetChild
                    attach prevChildOpt newChild targetChild

    /// Update the OnSizeAllocated callback of a control, given previous and current values
    let updateOnSizeAllocated prevValueOpt valueOpt (target: obj) = 
        let target = (target :?> CustomContentPage)
        match prevValueOpt with ValueNone -> () | ValueSome f -> target.SizeAllocated.RemoveHandler(f)
        match valueOpt with ValueNone -> () | ValueSome f -> target.SizeAllocated.AddHandler(f)
        
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
    let inline updateCommand prevCommandValueOpt commandValueOpt argTransform setter  prevCanExecuteValueOpt canExecuteValueOpt target = 
        match prevCommandValueOpt, prevCanExecuteValueOpt, commandValueOpt, canExecuteValueOpt with 
        | ValueNone, ValueNone, ValueNone, ValueNone -> ()
        | ValueSome prevf, ValueNone, ValueSome f, ValueNone when identical prevf f -> ()
        | ValueSome prevf, ValueSome prevx, ValueSome f, ValueSome x when identical prevf f && prevx = x -> ()
        | _, _, ValueNone, _ -> setter target null
        | _, _, ValueSome f, ValueNone -> setter target (makeCommand (fun () -> f (argTransform target)))
        | _, _, ValueSome f, ValueSome k -> setter target (makeCommandCanExecute (fun () -> f (argTransform target)) k)

    /// Update the CurrentPage of a control, given previous and current values
    let updateMultiPageOfTCurrentPage<'a when 'a :> Xamarin.Forms.Page> prevValueOpt valueOpt (target: Xamarin.Forms.MultiPage<'a>) =
        match prevValueOpt, valueOpt with
        | ValueNone, ValueNone -> ()
        | ValueSome prev, ValueSome curr when prev = curr -> ()
        | ValueSome _, ValueNone -> target.CurrentPage <- Unchecked.defaultof<'a>
        | _, ValueSome curr -> target.CurrentPage <- target.Children.[curr]

    /// Update the Minium and Maximum values of a slider, given previous and current values
    let updateSliderMinimumMaximum prevValueOpt valueOpt (target: obj) =
        let control = target :?> Xamarin.Forms.Slider
        let defaultValue = (0.0, 1.0)
        let updateFunc (_, prevMaximum) (newMinimum, newMaximum) =
            if newMinimum > prevMaximum then
                control.Maximum <- newMaximum
                control.Minimum <- newMinimum
            else
                control.Minimum <- newMinimum
                control.Maximum <- newMaximum

        match prevValueOpt, valueOpt with
        | ValueNone, ValueNone -> ()
        | ValueSome prev, ValueSome curr when prev = curr -> ()
        | ValueSome prev, ValueSome curr -> updateFunc prev curr
        | ValueSome prev, ValueNone -> updateFunc prev defaultValue
        | ValueNone, ValueSome curr -> updateFunc defaultValue curr

    /// Update the Minimum and Maximum values of a stepper, given previous and current values
    let updateStepperMinimumMaximum prevValueOpt valueOpt (target: obj) =
        let control = target :?> Xamarin.Forms.Stepper
        let defaultValue = (0.0, 1.0)
        let updateFunc (_, prevMaximum) (newMinimum, newMaximum) =
            if newMinimum > prevMaximum then
                control.Maximum <- newMaximum
                control.Minimum <- newMinimum
            else
                control.Minimum <- newMinimum
                control.Maximum <- newMaximum

        match prevValueOpt, valueOpt with
        | ValueNone, ValueNone -> ()
        | ValueSome prev, ValueSome curr when prev = curr -> ()
        | ValueSome prev, ValueSome curr -> updateFunc prev curr
        | ValueSome prev, ValueNone -> updateFunc prev defaultValue
        | ValueNone, ValueSome curr -> updateFunc defaultValue curr

    /// Update the AcceleratorProperty of a MenuItem, given previous and current Accelerator
    let updateMenuItemAccelerator prevValue currValue (target: Xamarin.Forms.MenuItem) =
        match prevValue, currValue with
        | ValueNone, ValueNone -> ()
        | ValueSome prevVal, ValueSome newVal when prevVal = newVal -> ()
        | _, ValueNone -> Xamarin.Forms.MenuItem.SetAccelerator(target, null)
        | _, ValueSome newVal -> Xamarin.Forms.MenuItem.SetAccelerator(target, Xamarin.Forms.Accelerator.FromString newVal)

    /// Update the items of a Shell, given previous and current view elements
    let updateShellItems (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.Shell) _ =
        let create (desc: ViewElement) =
            match desc.Create() with
            | :? ShellContent as shellContent -> ShellItem.op_Implicit shellContent
            | :? TemplatedPage as templatedPage -> ShellItem.op_Implicit templatedPage
            | :? ShellSection as shellSection -> ShellItem.op_Implicit shellSection
            | :? MenuItem as menuItem -> ShellItem.op_Implicit menuItem
            | :? ShellItem as shellItem -> shellItem
            | child -> failwithf "%s is not compatible with the type ShellItem" (child.GetType().Name)

        let update prevViewElement (currViewElement: ViewElement) (target: ShellItem) =
            let realTarget =
                match currViewElement.TargetType with
                | t when t = typeof<ShellContent> -> target.Items.[0].Items.[0] :> Element
                | t when t = typeof<TemplatedPage> -> target.Items.[0].Items.[0] :> Element
                | t when t = typeof<ShellSection> -> target.Items.[0] :> Element
                | t when t = typeof<MenuItem> -> target.GetType().GetProperty("MenuItem").GetValue(target) :?> Element // MenuShellItem is marked as internal
                | _ -> target :> Element
            updateChild prevViewElement currViewElement realTarget

        updateCollectionGeneric prevCollOpt collOpt target.Items create (fun _ _ _ -> ()) (fun _ _ -> true) update
        
    /// Update the menu items of a ShellContent, given previous and current view elements
    let updateShellContentMenuItems (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.ShellContent) =
        let create (desc: ViewElement) =
            desc.Create() :?> Xamarin.Forms.MenuItem

        updateCollectionGeneric prevCollOpt collOpt target.MenuItems create (fun _ _ _ -> ()) (fun _ _ -> true) updateChild

    /// Update the items of a ShellItem, given previous and current view elements
    let updateShellItemItems (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.ShellItem) _ =
        let create (desc: ViewElement) =
            match desc.Create() with
            | :? ShellContent as shellContent -> ShellSection.op_Implicit shellContent
            | :? TemplatedPage as templatedPage -> ShellSection.op_Implicit templatedPage
            | :? ShellSection as shellSection -> shellSection
            | child -> failwithf "%s is not compatible with the type ShellSection" (child.GetType().Name)

        let update prevViewElement (currViewElement: ViewElement) (target: ShellSection) =
            let realTarget =
                match currViewElement.TargetType with
                | t when t = typeof<ShellContent> -> target.Items.[0] :> BaseShellItem
                | t when t = typeof<TemplatedPage> -> target.Items.[0] :> BaseShellItem
                | _ -> target :> BaseShellItem
            updateChild prevViewElement currViewElement realTarget

        updateCollectionGeneric prevCollOpt collOpt target.Items create (fun _ _ _ -> ()) (fun _ _ -> true) update

    /// Update the items of a ShellSection, given previous and current view elements
    let updateShellSectionItems (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.ShellSection) _ =
        let create (desc: ViewElement) =
            desc.Create() :?> Xamarin.Forms.ShellContent

        updateCollectionGeneric prevCollOpt collOpt target.Items create (fun _ _ _ -> ()) (fun _ _ -> true) updateChild

    /// Trigger ScrollView.ScrollToAsync if needed, given the current values
    let triggerScrollToAsync _ (currValue: (float * float * AnimationKind) voption) (target: Xamarin.Forms.ScrollView) =
        match currValue with
        | ValueSome (x, y, animationKind) when x <> target.ScrollX || y <> target.ScrollY ->
            let animated =
                match animationKind with
                | Animated -> true
                | NotAnimated -> false
            target.ScrollToAsync(x, y, animated) |> ignore
        | _ -> ()

    /// Trigger ItemsView.ScrollTo if needed, given the current values
    let triggerScrollTo (currValue: (obj * obj * ScrollToPosition * AnimationKind) voption) (target: Xamarin.Forms.ItemsView) =
        match currValue with
        | ValueSome (x, y, scrollToPosition, animationKind) ->
            let animated =
                match animationKind with
                | Animated -> true
                | NotAnimated -> false
            target.ScrollTo(x,y, scrollToPosition, animated)
        | _ -> ()

    /// Trigger Shell.GoToAsync if needed, given the current values
    let triggerShellGoToAsync _ (currValue: (ShellNavigationState * AnimationKind) voption) (target: Xamarin.Forms.Shell) =
        match currValue with
        | ValueSome (navigationState, animationKind) ->
            let animated =
                match animationKind with
                | Animated -> true
                | NotAnimated -> false
            target.GoToAsync(navigationState, animated) |> ignore
        | _ -> ()

    let updatePageShellSearchHandler prevValueOpt (currValueOpt: ViewElement voption) target =
        match prevValueOpt, currValueOpt with
        | ValueNone, ValueNone -> ()
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueSome prevValue, ValueSome currValue ->
            let searchHandler = Shell.GetSearchHandler(target)
            currValue.UpdateIncremental(prevValue, searchHandler)
        | ValueNone, ValueSome currValue -> Shell.SetSearchHandler(target, currValue.Create() :?> Xamarin.Forms.SearchHandler)
        | ValueSome _, ValueNone -> Shell.SetSearchHandler(target, null)

    let updateShellBackgroundColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetBackgroundColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetBackgroundColor(target, Color.Default)

    let updateShellForegroundColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetForegroundColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetForegroundColor(target, Color.Default)

    let updateShellTitleColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetTitleColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetTitleColor(target, Color.Default)

    let updateShellDisabledColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetDisabledColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetDisabledColor(target, Color.Default)

    let updateShellUnselectedColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetUnselectedColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetUnselectedColor(target, Color.Default)

    let updateShellTabBarBackgroundColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetTabBarBackgroundColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetTabBarBackgroundColor(target, Color.Default)

    let updateShellTabBarForegroundColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetTabBarForegroundColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetTabBarForegroundColor(target, Color.Default)

    let updateShellBackButtonBehavior prevValueOpt (currValueOpt: ViewElement voption) target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetBackButtonBehavior(target, currValue.Create() :?> BackButtonBehavior)
        | ValueSome _, ValueNone -> Shell.SetBackButtonBehavior(target, null)

    let updateShellTitleView prevValueOpt (currValueOpt: ViewElement voption) target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetTitleView(target, currValue.Create() :?> View)
        | ValueSome _, ValueNone -> Shell.SetTitleView(target, null)

    let updateShellFlyoutBehavior prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetFlyoutBehavior(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetFlyoutBehavior(target, FlyoutBehavior.Flyout)

    let updateShellTabBarIsVisible prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetTabBarIsVisible(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetTabBarIsVisible(target, true)

    let updateShellNavBarIsVisible prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetNavBarIsVisible(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetNavBarIsVisible(target, true)

    let updateShellTabBarDisabledColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetTabBarDisabledColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetTabBarDisabledColor(target, Xamarin.Forms.Color.Default)

    let updateShellTabBarTitleColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetTabBarTitleColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetTabBarTitleColor(target, Xamarin.Forms.Color.Default)

    let updateShellTabBarUnselectedColor prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Shell.SetTabBarUnselectedColor(target, currValue)
        | ValueSome _, ValueNone -> Shell.SetTabBarUnselectedColor(target, Xamarin.Forms.Color.Default)

    let updateNavigationPageHasNavigationBar prevValueOpt currValueOpt target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> NavigationPage.SetHasNavigationBar(target, currValue)
        | ValueSome _, ValueNone -> NavigationPage.SetHasNavigationBar(target, true)

    let updateShellContentContentTemplate (prevValueOpt : ViewElement voption) (currValueOpt : ViewElement voption) (target : Xamarin.Forms.ShellContent) =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when identical prevValue currValue -> ()
        | ValueNone, ValueNone -> ()
        | ValueNone, ValueSome currValue ->
            target.ContentTemplate <- DirectViewElementDataTemplate(currValue)
        | ValueSome prevValue, ValueSome currValue ->
            target.ContentTemplate <- DirectViewElementDataTemplate(currValue)
            let realTarget = (target :> Xamarin.Forms.IShellContentController).Page
            if realTarget <> null then currValue.UpdateIncremental(prevValue, realTarget)            
        | ValueSome _, ValueNone -> target.ContentTemplate <- null
        
    let updatePageUseSafeArea (prevValueOpt: bool voption) (currValueOpt: bool voption) (target: Xamarin.Forms.Page) =
        let setUseSafeArea newValue =
                Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(
                    (target : Xamarin.Forms.Page).On<Xamarin.Forms.PlatformConfiguration.iOS>(),
                    newValue
                ) |> ignore
        
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> setUseSafeArea currValue
        | ValueSome _, ValueNone -> setUseSafeArea false

    let triggerWebViewReload _ curr (target: Xamarin.Forms.WebView) =
        if curr = ValueSome true then target.Reload()
    
    let updateEntryCursorPosition _ curr (target: Xamarin.Forms.Entry) =
        match curr with
        | ValueNone -> ()
        | ValueSome value -> target.CursorPosition <- value
    
    let updateEntrySelectionLength _ curr (target: Xamarin.Forms.Entry) =
        match curr with
        | ValueNone -> ()
        | ValueSome value -> target.SelectionLength <- value
        
    let updateMenuChildren (prevCollOpt: ViewElement array voption) (currCollOpt: ViewElement array voption) (target: Xamarin.Forms.Menu) _ =
        updateCollectionGeneric prevCollOpt currCollOpt target (fun _ -> target) (fun _ _ _ -> ()) (fun _ _ -> true) updateChild
        
    let updateElementEffects (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.Element) _ =
        let create (viewElement: ViewElement) =
            match viewElement.Create() with
            | :? CustomEffect as customEffect -> Effect.Resolve(customEffect.Name)
            | effect -> effect :?> Xamarin.Forms.Effect
        updateCollectionGeneric prevCollOpt collOpt target.Effects create (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild
        
    let updatePageToolbarItems (prevCollOpt: ViewElement array voption) (collOpt: ViewElement array voption) (target: Xamarin.Forms.Page) _ =
        let create (viewElement: ViewElement) =
            viewElement.Create() :?> Xamarin.Forms.ToolbarItem
        
        updateCollectionGeneric prevCollOpt collOpt target.ToolbarItems create (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild

    let updateElementMenu prevValueOpt (currValueOpt: ViewElement voption) target =
        match prevValueOpt, currValueOpt with
        | ValueSome prevValue, ValueSome currValue when prevValue = currValue -> ()
        | ValueNone, ValueNone -> ()
        | _, ValueSome currValue -> Element.SetMenu(target, currValue.Create() :?> Menu)
        | ValueSome _, ValueNone -> Element.SetMenu(target, null)