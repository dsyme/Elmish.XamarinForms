// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open System
open System.Collections.ObjectModel
open System.Collections.Generic
open Xamarin.Forms
open Xamarin.Forms.StyleSheets
open FSharp.Data.Adaptive
open System.Windows.Input


[<AutoOpen>]
module ViewUpdaters =

    let updateCollectionPrim
           (coll: 'T alist) 
           childSetter
           childInsert
           childAdd
           childRemove
           =
      let mutable children : IndexList<'T>  = IndexList.empty
      let reader = coll.GetReader()
      (fun token (target: 'Target) -> 
        let changes = reader.GetChanges(token) 
        for (idx, op) in IndexListDelta.toSeq changes do
            match op with
            | Set child ->

                let (_, mid, right) = IndexList.neighbours idx children
                match mid with
                | Some(midIdx, _) ->
                    match IndexList.tryGetPosition midIdx children with
                    | Some i -> childSetter target child i
                    | None -> System.Diagnostics.Debug.Assert(false); failwith "inconsistent"
                | None ->
                    match right with
                    | Some (rightIdx, _) ->
                        match IndexList.tryGetPosition rightIdx children with
                        | Some i -> childInsert target child i 
                        | None -> System.Diagnostics.Debug.Assert(false); failwith "inconsistent"
                    | None -> childAdd target child
                children <- IndexList.set idx child children

            | Remove ->
                match IndexList.tryRemove idx children with
                | Some (child, rest) ->
                    let (_, mid, right) = IndexList.neighbours idx children
                    match mid, right with
                    | Some _, Some _ -> 
                        match IndexList.tryGetPosition idx children with
                        | Some i -> childRemove target child i false
                        | None -> ()
                    | Some _, None -> 
                        match IndexList.tryGetPosition idx children with
                        | Some i -> childRemove target child i true
                        | None -> ()
                    | None, _ ->
                        ()
                    children <- rest
                | None -> ()
        )

    let updateViewElementCollectionPrim
           (coll: ViewElement alist) 
           (createTransform: 'TargetT -> 'ActualTargetT) 
           (updateAttachedProps: AdaptiveToken -> 'ActualTargetT -> unit)
           childSetter
           childInsert
           childAdd
           childRemove  =
      let mutable children : IndexList<ViewElementUpdater * AttachedPropsUpdater>  = IndexList.empty
      let reader = coll.GetReader()
      (fun token (target: 'Target) -> 
        let changes = reader.GetChanges(token) 
        for (idx, op) in IndexListDelta.toSeq changes do
            match op with
            | Set childNode ->

                let childUpdater = 
                    { new ViewElementUpdater(AVal.constant childNode) with
                        // OnCreated may get called multiple times if the thing is re-created
                        member __.OnCreated (_scope: obj, element: obj) = 
                            let child = createTransform (element :?> 'TargetT)
                            let (_, mid, right) = IndexList.neighbours idx children
                            match mid with
                            | Some(midIdx, _) ->
                                match IndexList.tryGetPosition midIdx children with
                                | Some i -> childSetter target child i
                                | None -> System.Diagnostics.Debug.Assert(false); failwith "inconsistent"
                            | None ->
                                match right with
                                | Some (rightIdx, _) ->
                                    match IndexList.tryGetPosition rightIdx children with
                                    | Some i -> childInsert target child i 
                                    | None -> System.Diagnostics.Debug.Assert(false); failwith "inconsistent"
                                | None -> childAdd target child 
                    }
                let attachedPropsUpdater =
                    AttachedPropsUpdater((fun token child -> updateAttachedProps token (unbox<'ActualTargetT> child)), childNode)

                // Creates the child for the new element if necessary
                childUpdater.Update(token, ())
                attachedPropsUpdater.Update(token, childUpdater.Target)
                children <- IndexList.set idx (childUpdater, attachedPropsUpdater) children

            | Remove ->
                match IndexList.tryRemove idx children with
                | Some (_oldUpdater, rest) ->
                    let (_, mid, right) = IndexList.neighbours idx children
                    match mid with
                    | Some _ -> 
                        match IndexList.tryGetPosition idx children with
                        | Some i -> childRemove target i right.IsNone
                        | None -> ()
                    | None ->
                        ()
                    children <- rest
                | None -> ()

        // An update may have been requested because of a change in a child element
        for (childUpdater, attachedPropsUpdater) in children do
            childUpdater.Update(token, ())
            attachedPropsUpdater.Update(token, childUpdater.Target)
        )

    /// Incremental list maintenance: given a collection, and a previous version of that collection, perform
    /// a reduced number of clear/add/remove/insert operations
    let updateViewElementCollection
           (coll: ViewElement alist) 
           (createTransform: 'TargetT -> 'ActualTargetT) 
           (updateAttachedProps: AdaptiveToken -> 'ActualTargetT -> unit)
        =
     // TODO: actually use the attach function....
        updateViewElementCollectionPrim
           coll 
           createTransform 
           updateAttachedProps
           (fun (targetColl: IList<'ActualTargetT>) child i -> targetColl.[i] <- child)
           (fun targetColl child i -> targetColl.Insert(i, child))
           (fun targetColl child -> targetColl.Add(child))
           (fun targetColl i _isAtEnd -> targetColl.RemoveAt(i))

    /// Update the attached properties for each item in an already updated collection
    let updateAttachedPropertiesForCollection
           (coll: 'T alist)
           (updateAttachedProps: AdaptiveToken -> 'ActualTargetT -> unit) =
        let updater = 
            updateCollectionPrim coll
                (fun (target: ObservableCollection<_>) x i -> target.[i] <- x)
                (fun target x i -> target.Insert(i, x))
                (fun target x -> target.Add(x))
                (fun target x i _isAtEnd -> target.RemoveAt(i))

        fun token (target: IList<'TargetT>) ->
            () (* TODO - call updateAttachedProps *)

    /// Update the attached properties for each item in Layout<T>.Children
    let updateAttachedPropertiesForLayoutOfT coll updateAttachedProps =
        let updater = updateAttachedPropertiesForCollection coll updateAttachedProps
        fun token (target: Xamarin.Forms.Layout<'T>) ->
            updater token target.Children
                    
    /// Update the items in a ItemsView control, given previous and current view elements
    let updateItemsViewItems (coll: ViewElement alist) = 
        let updater = updateViewElementCollection coll id (fun _ _ -> ())
        fun token (target: Xamarin.Forms.ItemsView) ->
            let targetColl = 
                match target.ItemsSource with 
                | :? ObservableCollection<ViewElementUpdater> as oc -> oc
                | _ -> 
                    let oc = ObservableCollection<ViewElementUpdater>()
                    target.ItemsSource <- oc
                    oc
            updater token targetColl
                    
    /// Update the items in a ItemsView<'T> control, given previous and current view elements
    let updateItemsViewOfTItems<'T when 'T :> Xamarin.Forms.BindableObject> (coll: ViewElement alist) =
        let updater = updateViewElementCollection coll id (fun _ _ -> ())
        fun token (target: Xamarin.Forms.ItemsView<'T>) ->
            let targetColl = 
                match target.ItemsSource with 
                | :? ObservableCollection<ViewElementUpdater> as oc -> oc
                | _ -> 
                    let oc = ObservableCollection<ViewElementUpdater>()
                    target.ItemsSource <- oc
                    oc
            updater token targetColl
                    
    /// Update the selected items in a SelectableItemsView control, given previous and current indexes
    let updateSelectableItemsViewSelectedItems coll = 
        let updater = 
            updateCollectionPrim coll
                (fun (target: ObservableCollection<_>) x i -> target.[i] <- x)
                (fun target x i -> target.Insert(i, x))
                (fun target x -> target.Add(x))
                (fun target x i _isAtEnd -> target.RemoveAt(i))

        fun token (target: Xamarin.Forms.SelectableItemsView) ->
            let targetColl = 
                match target.SelectedItems with 
                | :? ObservableCollection<obj> as oc -> oc
                | _ -> 
                    let oc = ObservableCollection<obj>()
                    target.SelectedItems <- oc
                    oc
            updater token targetColl

    /// Update the items in a SearchHandler control, given previous and current view elements
    let updateSearchHandlerItems (coll: ViewElement alist) = 
        let updater = updateViewElementCollection coll id (fun _ _ -> ())
        fun token (target: Xamarin.Forms.SearchHandler) ->
            let targetColl = 
                match target.ItemsSource with 
                | :? ObservableCollection<ViewElementUpdater> as oc -> oc
                | _ -> 
                    let oc = ObservableCollection<ViewElementUpdater>()
                    target.ItemsSource <- oc
                    oc
            updater token targetColl
        
(*
    let private updateViewElementHolderGroup (currShortName: string, currKey, coll: ViewElement alist) =
        let updater = updateViewElementCollection coll id
        fun token (target: ViewElementHolderGroup) ->
            target.ShortName <- currShortName
            target.ViewElement <- currKey
            updater token target

    /// Update the items in a GroupedListView control, given previous and current view elements
    let updateListViewGroupedItems (coll: (string * ViewElement * ViewElement alist) alist) (target: Xamarin.Forms.ListView) = 
        let updater = updateViewElementCollection coll (fun token elem -> ViewElementHolderGroup(elem)) (fun _ _ _ -> ()) (fun (_, prevKey, _) (_, currKey, _) -> ViewHelpers.canReuseView prevKey currKey) updateViewElementHolderGroup
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
    let updateTableSectionBaseOfTItems<'T when 'T :> Xamarin.Forms.BindableObject> (coll: ViewElement alist) attach =
        let updater = updateViewElementCollection coll id attach
        fun token (target: Xamarin.Forms.TableSectionBase<'T>) ->
            updater token target

    let updateResources (coll: (string * obj) alist) =
        updateCollectionPrim coll 
           (fun (target: VisualElement) (nm, r) i -> target.Resources.[nm] <- box r)
           (fun target (nm, r) i -> target.Resources.[nm] <- box r)
           (fun target (nm, r) -> target.Resources.Add(nm, box r))
           (fun target (nm, r) i _isAtEnd -> target.Resources.Remove(nm) |> ignore)

    /// Update the style sheets of a control, given previous and current view elements describing them
    // Note, style sheets can't be removed
    let updateStyleSheets (coll: StyleSheet alist) = 
        updateCollectionPrim coll 
           (fun (target: VisualElement) ss i -> target.Resources.Add(ss))
           (fun target ss i -> target.Resources.Add(ss))
           (fun target ss -> target.Resources.Add(ss))
           (fun target ss i _isAtEnd -> ())

    /// Update the styles of a control, given previous and current view elements describing them
    // Note, styles can't be removed
    let updateStyles (coll: Style alist) = 
        updateCollectionPrim coll
           (fun (target: VisualElement) style i -> target.Resources.Add(style))
           (fun target style i -> target.Resources.Add(style))
           (fun target style -> target.Resources.Add(style))
           (fun target style i _isAtEnd -> ())

    /// Incremental NavigationPage maintenance: push/pop the right pages
    let updateNavigationPages (coll: ViewElement alist) attach =
        updateViewElementCollectionPrim
           coll 
           id
           attach
           (fun (target: NavigationPage) child i -> System.Diagnostics.Debug.Assert(false); (* TODO - Set of NavigationPage *))
           (fun target child i -> System.Diagnostics.Debug.Assert(false); (* "TODO - Insert of NavigationPage" *))
           (fun target child -> target.PushAsync(child) |> ignore)
           (fun target i isAtEnd -> 
               if isAtEnd then 
                   target.PopAsync(true) |> ignore
               else
                   System.Diagnostics.Debug.Assert(false); (* "TODO - Non-pop remove of NavigationPage" *))

    /// Update a value if it has changed
    // TODO: restore inline
    let (* inline *) valueUpdater (value: aval<_>) f = 
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
        
    let makeEventHandler f = EventHandler<_>(fun _sender args -> f args)
    let makeEventHandlerNonGeneric f = EventHandler(fun _sender _args -> f())

    let eventUpdater (value: aval<'T>) (makeDelegate: 'T -> 'Delegate) (getter: 'Target -> IEvent<'Delegate,'Args>) = 
        let mutable prevOpt = ValueNone 
        fun token (target: 'Target) -> 
            let newValue =  value.GetValue(token) 
            let newDelegate =  makeDelegate newValue
            match prevOpt with
            | ValueSome prevValue -> 
                let targetEvent = getter target
                targetEvent.RemoveHandler(prevValue); targetEvent.AddHandler(newDelegate)
            | ValueNone -> 
                let targetEvent = getter target
                targetEvent.AddHandler(newDelegate)
            prevOpt <- ValueSome newDelegate

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
    // TODO: restore inline
    let (* inline *) updateCommand (canExecute: aval<bool> option) argTransform setter (command: aval<_>) =
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
    let updateMultiPageOfTCurrentPage<'Target, 'PageKind when 'PageKind :> Xamarin.Forms.Page and 'Target :> Xamarin.Forms.MultiPage<'PageKind>> (value: aval<_>) = 
        valueUpdater value (fun (target: 'Target) prevOpt curr ->
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
    let updateShellItems (coll: ViewElement alist) attach =
        let transformRealChild (child: obj) =
            match child with
            | :? ShellContent as shellContent -> ShellItem.op_Implicit shellContent
            | :? TemplatedPage as templatedPage -> ShellItem.op_Implicit templatedPage
            | :? ShellSection as shellSection -> ShellItem.op_Implicit shellSection
            | :? MenuItem as menuItem -> ShellItem.op_Implicit menuItem
            | :? ShellItem as shellItem -> shellItem
            | child -> failwithf "%s is not compatible with the type ShellItem" (child.GetType().Name)

(*
       let updateRealChild (curr: ViewElement) =
            let updater = createChildUpdater curr
            fun token (target: ShellItem) ->
                let realTarget =
                    match curr.TargetType with
                    | t when t = typeof<ShellContent> -> target.Items.[0].Items.[0] :> Element
                    | t when t = typeof<TemplatedPage> -> target.Items.[0].Items.[0] :> Element
                    | t when t = typeof<ShellSection> -> target.Items.[0] :> Element
                    | t when t = typeof<MenuItem> -> target.GetType().GetProperty("MenuItem").GetValue(target) :?> Element // MenuShellItem is marked as internal
                    | _ -> target :> Element
                updater token realTarget
*)

        let updater = updateViewElementCollection coll transformRealChild attach
        fun token (target: Xamarin.Forms.Shell) ->
            updater token target.Items 
        
    /// Update the menu items of a ShellContent, given previous and current view elements
    let updateShellContentMenuItems (coll: ViewElement alist) =
        let updater = updateViewElementCollection coll id (fun _ _ -> ())
        fun token (target: Xamarin.Forms.ShellContent) ->
            updater token target.MenuItems 

    /// Update the items of a ShellItem, given previous and current view elements
    let updateShellItemItems (coll: ViewElement alist) attach =
        let transformRealChild (child: obj) =
            match child with
            | :? ShellContent as shellContent -> ShellSection.op_Implicit shellContent
            | :? TemplatedPage as templatedPage -> ShellSection.op_Implicit templatedPage
            | :? ShellSection as shellSection -> shellSection
            | child -> failwithf "%s is not compatible with the type ShellSection" (child.GetType().Name)

        //let updateRealChild (curr: ViewElement) =
        //    let updater = createChildUpdater curr
        //    fun token (target: ShellSection) ->
        //        let realTarget =
        //            match curr.TargetType with
        //            | t when t = typeof<ShellContent> -> target.Items.[0] :> BaseShellItem
        //            | t when t = typeof<TemplatedPage> -> target.Items.[0] :> BaseShellItem
        //            | _ -> target :> BaseShellItem
        //        updater token realTarget

        let updater = updateViewElementCollection coll transformRealChild attach
        fun token (target: Xamarin.Forms.ShellItem) ->
            updater token target.Items

    /// Update the items of a ShellSection, given previous and current view elements
    let updateShellSectionItems (coll: ViewElement alist) attach =
        let updater = updateViewElementCollection coll id attach
        fun token (target: Xamarin.Forms.ShellSection) ->
            updater token target.Items

    /// Trigger ScrollView.ScrollToAsync if needed, given the current values
    ///
    /// TODO: consider what trigger means for adaptive
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
    /// TODO: consider what trigger means for adaptive
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
    /// TODO: consider what trigger means for adaptive
    let triggerShellGoToAsync (curr: aval<(ShellNavigationState * AnimationKind)>) =
        fun token (target: Xamarin.Forms.Shell) ->
            let (navigationState, animationKind) = curr.GetValue(token)
            let animated =
                match animationKind with
                | Animated -> true
                | NotAnimated -> false
            target.GoToAsync(navigationState, animated) |> ignore

    let updatePageShellSearchHandler (element: ViewElement) =
        let updater = 
            { new ViewElementUpdater(AVal.constant element) with 
                  member __.OnCreated(scope: obj, element: obj) =
                      let scope = scope :?> NavigableElement
                      let handler = element :?> Xamarin.Forms.SearchHandler
                      Shell.SetSearchHandler(scope, handler) }
        fun token (target: NavigableElement) -> 
            updater.Update(token, target)

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
        
    let updateMenuChildren (coll: ViewElement alist) attach =
        let updater = updateViewElementCollection coll id attach
        fun token (target: Xamarin.Forms.Menu) ->
            updater token target
        
    let updateElementEffects (coll: ViewElement alist) attach =
        let createTransform (child: obj) =
            match child with
            | :? CustomEffect as customEffect -> Effect.Resolve(customEffect.Name)
            | effect -> effect :?> Xamarin.Forms.Effect
        let updater = updateViewElementCollection coll createTransform  attach
        fun token (target: Xamarin.Forms.Element) ->
            updater token target.Effects

    let updatePageToolbarItems (coll: ViewElement alist) attach =
        let updater = updateViewElementCollection coll id attach
        fun token (target: Xamarin.Forms.Page) ->
            updater token target.ToolbarItems

    let updateElementMenu (value : ViewElement) =
        let updater = 
            { new ViewElementUpdater(AVal.constant value) with 
                  member __.OnCreated(scope: obj, element: obj) =
                      let scope = scope :?> Element
                      let handler = element :?> Xamarin.Forms.Menu
                      Shell.SetMenu(scope, handler) }
        fun token (target: Element) -> 
            updater.Update(token, target)

