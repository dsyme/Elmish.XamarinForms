// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

[<AutoOpen>]
module MapsExtension = 

    open Fabulous
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
        static member inline Map(?pins: seq<ViewElement>, ?isShowingUser: bool, ?mapType: MapType, ?hasScrollEnabled: bool,
                                 ?hasZoomEnabled: bool, ?requestedRegion: MapSpan,
                                 // inherited attributes common to all views
                                 ?gestureRecognizers, ?horizontalOptions, ?margin, ?verticalOptions, ?anchorX, ?anchorY, ?backgroundColor,
                                 ?behaviors, ?flowDirection, ?height, ?inputTransparent, ?isEnabled, ?isTabStop, ?isVisible, ?minimumHeight,
                                 ?minimumWidth, ?opacity, ?resources, ?rotation, ?rotationX, ?rotationY, ?scale, ?scaleX, ?scaleY, ?styles,
                                 ?styleSheets, ?tabIndex, ?translationX, ?translationY, ?visual, ?width, ?style, ?styleClasses, ?shellBackButtonBehavior,
                                 ?shellBackgroundColor, ?shellDisabledColor, ?shellForegroundColor, ?shellFlyoutBehavior, ?shellNavBarIsVisible,
                                 ?shellSearchHandler, ?shellTabBarBackgroundColor, ?shellTabBarDisabledColor, ?shellTabBarForegroundColor,
                                 ?shellTabBarIsVisible, ?shellTabBarTitleColor, ?shellTabBarUnselectedColor, ?shellTitleColor, ?shellTitleView,
                                 ?shellUnselectedColor, ?automationId, ?classId, ?effects, ?menu, ?ref, ?styleId, ?tag, ?focused, ?unfocused, ?created) =

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
                ViewBuilders.BuildView(attribCount, ?gestureRecognizers=gestureRecognizers, ?horizontalOptions=horizontalOptions, ?margin=margin,
                                       ?verticalOptions=verticalOptions, ?anchorX=anchorX, ?anchorY=anchorY, ?backgroundColor=backgroundColor, ?behaviors=behaviors,
                                       ?flowDirection=flowDirection, ?height=height, ?inputTransparent=inputTransparent, ?isEnabled=isEnabled, ?isTabStop=isTabStop,
                                       ?isVisible=isVisible, ?minimumHeight=minimumHeight, ?minimumWidth=minimumWidth, ?opacity=opacity, ?resources=resources,
                                       ?rotation=rotation, ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?scaleX=scaleX, ?scaleY=scaleY, ?styles=styles,
                                       ?styleSheets=styleSheets, ?tabIndex=tabIndex, ?translationX=translationX, ?translationY=translationY, ?visual=visual, ?width=width,
                                       ?style=style, ?styleClasses=styleClasses, ?shellBackButtonBehavior=shellBackButtonBehavior, ?shellBackgroundColor=shellBackgroundColor,
                                       ?shellDisabledColor=shellDisabledColor, ?shellForegroundColor=shellForegroundColor, ?shellFlyoutBehavior=shellFlyoutBehavior,
                                       ?shellNavBarIsVisible=shellNavBarIsVisible, ?shellSearchHandler=shellSearchHandler, ?shellTabBarBackgroundColor=shellTabBarBackgroundColor,
                                       ?shellTabBarDisabledColor=shellTabBarDisabledColor, ?shellTabBarForegroundColor=shellTabBarForegroundColor,
                                       ?shellTabBarIsVisible=shellTabBarIsVisible, ?shellTabBarTitleColor=shellTabBarTitleColor, ?shellTabBarUnselectedColor=shellTabBarUnselectedColor,
                                       ?shellTitleColor=shellTitleColor, ?shellTitleView=shellTitleView, ?shellUnselectedColor=shellUnselectedColor, ?automationId=automationId,
                                       ?classId=classId, ?effects=effects, ?menu=menu, ?ref=ref, ?styleId=styleId, ?tag=tag, ?focused=focused, ?unfocused=unfocused, ?created=created)

            // Add our own attributes. They must have unique names which must match the names below.
            match pins with None -> () | Some v -> attribs.Add(MapPinsAttribKey, v) 
            match hasScrollEnabled with None -> () | Some v -> attribs.Add(MapHasScrollEnabledAttribKey, v) 
            match isShowingUser with None -> () | Some v -> attribs.Add(MapIsShowingUserAttribKey, v) 
            match mapType with None -> () | Some v -> attribs.Add(MapTypeAttribKey, v) 
            match hasZoomEnabled with None -> () | Some v -> attribs.Add(MapHasZoomEnabledAttribKey, v) 
            match requestedRegion with None -> () | Some v -> attribs.Add(MapRequestingRegionAttribKey, v) 

            // The update method
            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: Map) = 
                ViewBuilders.UpdateView (prevOpt, source, target)
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

#if DEBUG 
    let sample1 = View.Map(hasZoomEnabled = true, hasScrollEnabled = true)

    let sample2 = 
        let timbuktu = Position(16.7666, -3.0026)
        View.Map(hasZoomEnabled = true, hasScrollEnabled = true,
                 requestedRegion = MapSpan.FromCenterAndRadius(timbuktu, Distance.FromKilometers(1.0)))

    let sample3 = 
        let paris = Position(48.8566, 2.3522)
        let london = Position(51.5074, -0.1278)
        let calais = Position(50.9513, 1.8587)
        View.Map(hasZoomEnabled = true, hasScrollEnabled = true, 
                 pins = [ View.Pin(paris, label="Paris", pinType = PinType.Place)
                          View.Pin(london, label="London", pinType = PinType.Place) ] ,
                 requestedRegion = MapSpan.FromCenterAndRadius(calais, Distance.FromKilometers(300.0)))
#endif
