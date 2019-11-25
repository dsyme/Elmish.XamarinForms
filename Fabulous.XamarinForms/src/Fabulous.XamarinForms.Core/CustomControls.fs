// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open FSharp.Data.Adaptive
open Xamarin.Forms
open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.ComponentModel

/////////////////
/// DataTemplate
/////////////////

type ViewElementHolder(node: ViewElement) =
    let ev = new Event<_,_>()
    let mutable node = node
    let mutable updater = new ViewElementUpdater(AVal.constant node, (fun (scope: obj) (child: obj) -> ()), id)

    interface INotifyPropertyChanged with
        [<CLIEvent>] 
        member x.PropertyChanged = ev.Publish

    member x.SetTarget(targetToUse)  =
        updater.SetTarget(targetToUse)
        
    member x.Update(token)  =
        updater.Update(token, scope=null)
        
    member x.ViewElement = node

    member x.SetViewElement value =
        node <- value
        updater <- new ViewElementUpdater(AVal.constant node, (fun (scope: obj) (child: obj) -> ()), id)
        ev.Trigger(x, PropertyChangedEventArgs "ViewElement") 

#if GROUPLIST

[<AllowNullLiteral>]
type ViewElementHolderGroup(shortName: string, viewElement: ViewElement, items: ViewElement[]) =
    inherit ObservableCollection<ViewElementHolder>(Seq.map ViewElementHolder items)
        
    let ev = new Event<_,_>()
    let mutable shortNameData = shortName
    let mutable data = viewElement
    
    interface IViewElementHolder with
        member x.ViewElement = data
        [<CLIEvent>] member x.PropertyChanged = ev.Publish
        
    member x.ViewElement
        with get() = data
        and set(value) =
            data <- value
            ev.Trigger(x, PropertyChangedEventArgs "ViewElement")
            
    member x.ShortName
        with get() = shortNameData
        and set(value) =
            shortNameData <- value
            ev.Trigger(x, PropertyChangedEventArgs "ShortName")
            
    member __.Items = items
#endif

module BindableHelpers =

    // The target is created by the call to Activator.CreateInstance
    let createOnBindingContextChanged (target: BindableObject) =
        let mutable holderOpt : ViewElementHolder voption = ValueNone

        let onDataPropertyChanged = PropertyChangedEventHandler(fun _ args ->
            match args.PropertyName, holderOpt with
            | "ViewElement", ValueSome holder ->
                // We really need to recreate the target here, but the data template binding process doesn't let us recreate???
                // TODO: this Update would need to reset all the properties to continue to use the original target
                holder.Update(AdaptiveToken.Top)
            | _ -> ()
        )
        
        let onBindingContextChanged () =
            match holderOpt with
            | ValueNone -> ()
            | ValueSome prevHolder -> 
                (prevHolder :> INotifyPropertyChanged).PropertyChanged.RemoveHandler onDataPropertyChanged
            
            match target.BindingContext with
            | :? ViewElementHolder as holder ->
                (holder :> INotifyPropertyChanged).PropertyChanged.AddHandler onDataPropertyChanged
                holder.SetTarget target
                holder.Update AdaptiveToken.Top
                holderOpt <- ValueSome holder
            | _ ->
                holderOpt <- ValueNone
            
        onBindingContextChanged
        
type ViewElementDataTemplate(``type``) =
    inherit DataTemplate(fun () ->
        let target = (Activator.CreateInstance ``type``) :?> BindableObject
        let onBindingContextChanged = BindableHelpers.createOnBindingContextChanged target
        target.BindingContextChanged.Add (fun _ -> onBindingContextChanged ())
        target :> obj
    )

type ViewElementDataTemplateSelector() =
    inherit DataTemplateSelector()
    let cache = Dictionary<Type, ViewElementDataTemplate>()
    override this.OnSelectTemplate(item, _) =
        let holder = item :?> ViewElementHolder
        let targetType = holder.ViewElement.TargetType
        
        match cache.TryGetValue targetType with
        | false, _ ->
            let template = ViewElementDataTemplate(targetType)
            cache.[targetType] <- template
            template :> DataTemplate
        | true, template ->
            template :> DataTemplate
            
type DirectViewElementDataTemplate(viewElement: ViewElement) =
    inherit DataTemplate(Func<obj>((fun () ->
        // TODO: remove this call to CreateInternal
        viewElement.CreateInternal(AdaptiveToken.Top))))
        
/////////////////
/// Cells
/////////////////

type CustomEntryCell() as self = 
    inherit EntryCell()
    let textChanged = Event<EventHandler<TextChangedEventArgs>, TextChangedEventArgs>()
    let mutable oldTextValue = ""

    do self.PropertyChanging.Add(
        fun args ->
            if args.PropertyName = "Text" then
                oldTextValue <- self.Text)

    do self.PropertyChanged.Add(
        fun args ->
            if args.PropertyName = "Text" then
                textChanged.Trigger(self, TextChangedEventArgs(oldTextValue, self.Text)))

    [<CLIEvent>] member __.TextChanged = textChanged.Publish

/////////////////
/// Collections
/////////////////

type CustomListView() =
    inherit ListView(ItemTemplate = ViewElementDataTemplateSelector())
    
type CustomGroupListView() = 
    inherit ListView(IsGroupingEnabled = true, ItemTemplate = ViewElementDataTemplateSelector(), GroupHeaderTemplate = ViewElementDataTemplateSelector())

type CustomCollectionView() = 
    inherit CollectionView(ItemTemplate = ViewElementDataTemplateSelector())

type CustomCarouselView() =
    inherit CarouselView(ItemTemplate = ViewElementDataTemplateSelector())

/////////////////
/// Controls
/////////////////
    
/// A name holder for effects that don't require to create a cross-platform type to use them
type CustomEffect() =
    inherit BindableObject()
    member val Name = "" with get, set

/// A custom SearchHandler which exposes the overridable methods OnQueryChanged, OnQueryConfirmed and OnItemSelected as events
type CustomSearchHandler() =
    inherit SearchHandler(ItemTemplate = ViewElementDataTemplateSelector())
    
    let queryChanged = Event<EventHandler<string * string>, _>()
    let queryConfirmed = Event<EventHandler, _>()
    let itemSelected = Event<EventHandler<obj>, _>()
    
    [<CLIEvent>] member __.QueryChanged = queryChanged.Publish
    [<CLIEvent>] member __.QueryConfirmed = queryConfirmed.Publish
    [<CLIEvent>] member __.ItemSelected = itemSelected.Publish

    override this.OnQueryChanged(oldValue, newValue) = queryChanged.Trigger(this, (oldValue, newValue))
    override this.OnQueryConfirmed() = queryConfirmed.Trigger(this, null)
    override this.OnItemSelected(item) = itemSelected.Trigger(this, item)
    
/// A custom TimePicker which exposes a TimeChanged event to notify when the user has selected a new time from the picker
type CustomTimePicker() =
    inherit TimePicker()
    
    let timeChanged = Event<EventHandler<TimeSpan>, _>()
    
    [<CLIEvent>] member __.TimeChanged = timeChanged.Publish

    override this.OnPropertyChanged(propertyName) =
        base.OnPropertyChanged(propertyName)
        if propertyName = "Time" then
            timeChanged.Trigger(this, this.Time)

/////////////////
/// Pages
/////////////////

/// The underlying page type for the ContentPage view element
type CustomContentPage() as self = 
    inherit ContentPage()
    do Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(self, true)
    
    let sizeAllocated = Event<EventHandler<double * double>, _>()

    [<CLIEvent>] member __.SizeAllocated = sizeAllocated.Publish

    override this.OnSizeAllocated(width, height) =
        base.OnSizeAllocated(width, height)
        sizeAllocated.Trigger(this, (width, height))