// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

[<AutoOpen>]
module MapsExtension = 

    open Fabulous
    open FSharp.Data.Adaptive
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
        static member inline Map(?pins: ViewElement alist, ?isShowingUser: aval<bool>, ?mapType: aval<MapType>, ?hasScrollEnabled: aval<bool>,
                                 ?hasZoomEnabled: aval<bool>, ?requestedRegion: aval<MapSpan>,
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

            let attribs = attribs.Retarget<Map>()

            let updater1 = ViewExtensions.ValueUpdater(hasScrollEnabled, (fun (target: Map) v -> target.HasScrollEnabled <- v))
            let updater2 = ViewExtensions.ValueUpdater(isShowingUser, (fun (target: Map) v -> target.IsShowingUser <- v))
            let updater3 = ViewExtensions.ValueUpdater(mapType, (fun (target: Map) v -> target.MapType <- v))
            let updater4 = ViewExtensions.ValueUpdater(hasZoomEnabled, (fun (target: Map) v -> target.HasZoomEnabled <- v))
            let updater5 = ViewExtensions.ElementCollectionUpdater(pins, (fun (target: Map) -> target.Pins))
            let updater6 = ViewExtensions.ValueUpdater(requestedRegion, (fun (target: Map) v -> target.MoveToRegion(v)))

            // Add our own attributes. They must have unique names which must match the names below.
            match pins with None -> () | Some v -> attribs.Add(MapPinsAttribKey, v, updater1) 
            match hasScrollEnabled with None -> () | Some v -> attribs.Add(MapHasScrollEnabledAttribKey, v, updater2) 
            match isShowingUser with None -> () | Some v -> attribs.Add(MapIsShowingUserAttribKey, v, updater3) 
            match mapType with None -> () | Some v -> attribs.Add(MapTypeAttribKey, v, updater4) 
            match hasZoomEnabled with None -> () | Some v -> attribs.Add(MapHasZoomEnabledAttribKey, v, updater5) 
            match requestedRegion with None -> () | Some v -> attribs.Add(MapRequestingRegionAttribKey, v, updater6) 

            // The element
            ViewElement.Create<Xamarin.Forms.Maps.Map>(Map, attribs.Close())

        /// Describes a Pin in the view
        static member Pin(?position: aval<Position>, ?label: aval<string>, ?pinType: aval<PinType>, ?address: aval<string>) = 

            // Count the number of additional attributes
            let attribCount = 0
            let attribCount = match position with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match label with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match pinType with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match address with Some _ -> attribCount + 1 | None -> attribCount

            let attribs = AttributesBuilder<Pin>(attribCount)

            let updater1 = ViewExtensions.ValueUpdater(position, (fun (target: Pin) v -> target.Position <- v))
            let updater2 = ViewExtensions.ValueUpdater(label, (fun (target: Pin) v -> target.Label <- v))
            let updater3 = ViewExtensions.ValueUpdater(pinType, (fun (target: Pin) v -> target.Type <- v))
            let updater4 = ViewExtensions.ValueUpdater(address, (fun (target: Pin) v -> target.Address <- v))

            // Add our own attributes. They must have unique names which must match the names below.
            match position with None -> () | Some v -> attribs.Add(PinPositionAttribKey, v, updater1) 
            match label with None -> () | Some v -> attribs.Add(PinLabelAttribKey, v, updater2) 
            match pinType with None -> () | Some v -> attribs.Add(PinTypeAttribKey, v, updater3) 
            match address with None -> () | Some v -> attribs.Add(PinAddressAttribKey, v, updater4) 

            // The element
            ViewElement.Create<Xamarin.Forms.Maps.Pin>(Pin, attribs.Close())

#if DEBUG 
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
                             View.Pin(c london, label= c "London", pinType = c PinType.Place) ] ,
                 requestedRegion = c (MapSpan.FromCenterAndRadius(calais, Distance.FromKilometers(300.0))))
#endif
