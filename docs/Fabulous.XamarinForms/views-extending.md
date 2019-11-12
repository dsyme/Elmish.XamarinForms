Fabulous for Xamarin.Forms - Guide
=======

{% include_relative contents-views.md %}

View Extensions
------

Many open source and 3rd-party libraries of Xamarin.Forms controls exist. To use other controls, a small amount of wrapper code
is typically needed to define a corresponding view element using the incremental-update model used by Fabulous.

The following additional view elements are available as pre-built nuget libraries:

* [FFImageLoading](views-ffimageloading.md) for cached images, as opposed to the built-in Image view that wastes time and memory
* [Maps](views-maps.md) for platform maps
* [SkiaSharp](views-skiasharp.md) for drawing 2D graphics
* [OxyPlot](views-oxyplot.md) for charting
* [VideoManager](views-videomanager.md) for playing audio and video

To use other Xamarin.Forms controls, a small amount of wrapper code must
be written to convert the control to an Fabulous view element.

> Please consider contributing your extensions to [this repository](https://github.com/fsprojects/Fabulous/tree/master/Fabulous.XamarinForms/extensions).

The basic shape of an extension view component is shown below. Here we assume the Xamarin.Forms control defines one extra element
called ABC deriving from existing element kind BASE, and that ABC has one additional
collection property `Prop1` and one primitive property `Prop2`.
(A collection property is a one that may contain further sub-elements, e.g. `children` for StackLayout, `gestureRecognizers` for any `View`
and `pins` in the Maps example further below.)

An view element simply defines a static member that extends `View` and returns a `ViewElement`.
The view element inherits attributes and update functionality from BASE via prototype inheritance.

> **NOTE**: we are considering adding a code generator or type provider to automate this process, though the code is not complex to write.
>
> **NOTE**: The API used to write these extensions is subject to change.

```fsharp
[<AutoOpen>]
module MyViewExtensions =

    open Fabulous
    open Fabulous.XamarinForms

    // Define keys for the possible attributes
    let Prop1AttribKey = AttributeKey<seq<ViewElement>> "ABC_Prop1"
    let Prop2AttribKey = AttributeKey<bool> "ABC_Prop2"

    // Fully-qualified name to avoid extending by mistake
    // another View class (like Xamarin.Forms.View)
    type Fabulous.XamarinForms.View with
        /// Describes a ABC in the view
        /// The inline keyword is important for performance
        static member inline ABC(?prop1: seq<ViewElement>, ?prop2: bool, ... inherited attributes ... ) =

            // Count the number of additional attributes
            let attribCount = 0
            let attribCount = match prop1 with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match prop2 with Some _ -> attribCount + 1 | None -> attribCount

            // Populate the attributes of the base element
            let attribs = ViewBuilders.BuildBASE(attribCount, ... inherited attributes ... )

            // Add our own attributes.
            match prop1 with None -> () | Some v -> attribs.Add (Prop1AttribKey, v)
            match prop2 with None -> () | Some v -> attribs.Add (Prop2AttribKey, v)
            ...

            // The creation method
            let create () = new ABC()

            // The incremental update method
            let update (prev: ViewElement voption) (source: ViewElement) (target: ABC) =
                ViewBuilders.UpdateBASE (prev, source, target)
                source.UpdateElementCollection (prev, rop1AttribKey, target.Prop1)
                source.UpdatePrimitive (prev, target, Prop2AttribKey, (fun target -> target.Prop2), (fun target v -> target.Prop2 <- v))
                ...

            ViewElement.Create<ABC>(create, update, attribs)
```

The control is then used as follows:

```fsharp
    View.ABC(Prop1 = [ View.Label("hello") ], prop2 = true, property3 = "Yo!")
```

The `update` method of the extension is specified using:

* `source.UpdatePrimitive(prev, target, attribKey, setter, ?defaultValue)` - incrementally update a primitive
* `source.UpdateElement(prev, target, attribKey, getter, setter)` - incrementally update a nested element
* `source.UpdateElementCollection(prev, attribKey, targetCollection)` - incrementally update a collection of nested elements
* `source.UpdateEvent(prev, target, attribKey, setter, ?defaultValue)` - incrementally update a primitive event

Sometimes it makes sense to "massage" the input values before storing them in attibutes, e.g. to apply a conversion from an F#-friendly value
to a stored attribte value here:

```fsharp
match prop1 with None -> () | Some v -> attribs.Add(Prop1AttribKey, box (CONV v))
```

It is common to mark view extensions as `inline`. This allows the F# compiler to create more optimized
element-creation code for each particular instantiation based on the small set of properties specified at a particular usage point.
In particular the compiler can statically determine the count of attributes and remove all allocations related to
optional arguments.

### Example: Authoring the Xamarin.Forms.Maps Extension

The implementation of an extension for `Xamarin.Forms.Maps` is shown below - this is the same extension as that
available in `Fabulous.XamarinForms.Maps.dll`. The sample implements the extension for the types [Map](https://docs.microsoft.com/dotnet/api/xamarin.forms.maps.map?view=xamarin-forms]) and
[Pin](https://docs.microsoft.com/en-gb/dotnet/api/xamarin.forms.maps.pin?view=xamarin-forms).

```fsharp
[<AutoOpen>]
module MapsExtension =

    open Fabulous.XamarinForms

    open Xamarin.Forms
    open Xamarin.Forms.Maps

    let MapHasScrollEnabledAttribKey = AttributeKey "Map_HasScrollEnabled"
    let MapIsShowingUserAttribKey = AttributeKey "Map_IsShowingUser"
    let MapPinsAttribKey = AttributeKey "Map_Pins"
    let MapTypeAttribKey = AttributeKey "Map_MapType"
    let MapHasZoomEnabledAttribKey = AttributeKey "Map_HasZoomEnabled"
    let MapRequestingRegionAttribKey = AttributeKey "Map_RequestedRegion"

    let PinPositionAttribKey = AttributeKey "Pin_Position"
    let PinLabelAttribKey = AttributeKey "Pin_Label"
    let PinTypeAttribKey = AttributeKey "Pin_PinType"
    let PinAddressAttribKey = AttributeKey "Pin_Address"

    type Fabulous.XamarinForms.View with
        /// Describes a Map in the view
        static member inline Map(?pins: seq<ViewElement>, ?isShowingUser: bool, ?mapType: MapType,
                                 ?hasScrollEnabled: bool, ?hasZoomEnabled: bool, ?requestedRegion: MapSpan,
                                 // inherited attributes common to all views
                                 ?horizontalOptions, ?verticalOptions, ?margin, ?gestureRecognizers, ?anchorX, ?anchorY, ?backgroundColor, 
                                 ?heightRequest, ?inputTransparent, ?isEnabled, ?isVisible, ?minimumHeightRequest, ?minimumWidthRequest, ?opacity,
                                 ?rotation, ?rotationX, ?rotationY, ?scale, ?style, ?translationX, ?translationY, ?widthRequest,
                                 ?resources, ?styles, ?styleSheets, ?classId, ?styleId, ?automationId) =

            // Count the number of additional attributes
            let attribCount = 0
            let attribCount = match pins with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match hasScrollEnabled with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match isShowingUser with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match mapType with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match hasZoomEnabled with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match requestedRegion with Some _ -> attribCount + 1 | None -> attribCount

            // Count and populate the inherited attributes
            let attribs =
                ViewBuilders.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions,
                               ?margin=margin, ?gestureRecognizers=gestureRecognizers, ?anchorX=anchorX, ?anchorY=anchorY,
                               ?backgroundColor=backgroundColor, ?heightRequest=heightRequest, ?inputTransparent=inputTransparent,
                               ?isEnabled=isEnabled, ?isVisible=isVisible, ?minimumHeightRequest=minimumHeightRequest,
                               ?minimumWidthRequest=minimumWidthRequest, ?opacity=opacity, ?rotation=rotation,
                               ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?style=style,
                               ?translationX=translationX, ?translationY=translationY, ?widthRequest=widthRequest,
                               ?resources=resources, ?styles=styles, ?styleSheets=styleSheets, ?classId=classId, ?styleId=styleId, ?automationId=automationId)

            // Add our own attributes. They must have unique names which must match the names below.
            match pins with None -> () | Some v -> attribs.Add(MapPinsAttribKey, v)
            match hasScrollEnabled with None -> () | Some v -> attribs.Add(MapHasScrollEnabledAttribKey, v)
            match isShowingUser with None -> () | Some v -> attribs.Add(MapIsShowingUserAttribKey, v)
            match mapType with None -> () | Some v -> attribs.Add(MapTypeAttribKey, v)
            match hasZoomEnabled with None -> () | Some v -> attribs.Add(MapHasZoomEnabledAttribKey, v)
            match requestedRegion with None -> () | Some v -> attribs.Add(MapRequestingRegionAttribKey, v)

            // The update method
            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: Map) =
                ViewBuilders.UpdateView(prevOpt, source, target)
                source.UpdatePrimitive(prevOpt, target, MapHasScrollEnabledAttribKey, (fun target v -> target.HasScrollEnabled <- v))
                source.UpdatePrimitive(prevOpt, target, MapHasZoomEnabledAttribKey, (fun target v -> target.HasZoomEnabled <- v))
                source.UpdatePrimitive(prevOpt, target, MapIsShowingUserAttribKey, (fun target v -> target.IsShowingUser <- v))
                source.UpdatePrimitive(prevOpt, target, MapTypeAttribKey, (fun target v -> target.MapType <- v))
                source.UpdateElementCollection(prevOpt, MapPinsAttribKey, target.Pins)
                source.UpdatePrimitive(prevOpt, target, MapRequestingRegionAttribKey, (fun target v -> target.MoveToRegion(v)))

            // The element
            ViewElement.Create<Xamarin.Forms.Maps.Map>(Map, update, attribs)

        /// Describes a Pin in the view
        static member Pin(?position: Position, ?label: string, ?pinType: PinType, ?address: string) =

            // Count the number of additional attributes
            let attribCount = 0
            let attribCount = match position with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match label with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match pinType with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match address with Some _ -> attribCount + 1 | None -> attribCount

            let attribs = AttributesBuilder(attribCount)

            // Add our own attributes. They must have unique names which must match the names below.
            match position with None -> () | Some v -> attribs.Add(PinPositionAttribKey, v)
            match label with None -> () | Some v -> attribs.Add(PinLabelAttribKey, v)
            match pinType with None -> () | Some v -> attribs.Add(PinTypeAttribKey, v)
            match address with None -> () | Some v -> attribs.Add(PinAddressAttribKey, v)

            // The update method
            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: Pin) =
                source.UpdatePrimitive(prevOpt, target, PinPositionAttribKey, (fun target v -> target.Position <- v))
                source.UpdatePrimitive(prevOpt, target, PinLabelAttribKey, (fun target v -> target.Label <- v))
                source.UpdatePrimitive(prevOpt, target, PinTypeAttribKey, (fun target v -> target.Type <- v))
                source.UpdatePrimitive(prevOpt, target, PinAddressAttribKey, (fun target v -> target.Address <- v))

            // The element
            ViewElement.Create<Xamarin.Forms.Maps.Pin>(Pin, update, attribs)
```

In the above example, inherited properties from `View` (such as `margin` or `horizontalOptions`) have been included in the facade for `Map`.  These properties
need not be added, you can set them on elements using the helper `With`, usable for all `View` properties:

```fsharp
View.Map(hasZoomEnabled = true, hasScrollEnabled = true).With(horizontalOptions = LayoutOptions.FillAndExpand)
```

### Example: MasterDetailPage without a toolbar on UWP with custom ViewBuilders

Fabulous uses ViewBuilders to create the underlying Xamarin.Forms classes. Customizing ViewBuilders is not the recommended way for custom controls but it is a great solution for overridden controls like in the following example:

```fsharp
type MasterDetailPageWithoutToolbar() =
    inherit Xamarin.Forms.MasterDetailPage()
    override __.ShouldShowToolbarButton() = false

Fabulous.XamarinForms.ViewBuilders.CreateFuncMasterDetailPage <- fun () ->
    upcast(new MasterDetailPageWithoutToolbar())

View.MasterDetailPage() // this now uses MasterDetailPageWithoutToolbar
```

See also:

* [Core Elements](views-elements.md)
* [Maps](views-maps.md)
* [SkiaSharp](views-skiasharp.md)
* [Source for the Maps extension](https://github.com/fsprojects/Fabulous/blob/master/Fabulous.XamarinForms/extensions/Maps/Xamarin.Forms.Maps.fs)
* [Source for the SkiaSharp extension](https://github.com/fsprojects/Fabulous/blob/master/Fabulous.XamarinForms/extensions/SkiaSharp/SkiaSharp.fs)
