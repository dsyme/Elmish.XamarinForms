// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

[<AutoOpen>]
module VideoManagerExtension =

    open Fabulous
    open MediaManager.Forms
    open MediaManager.Video
    open FSharp.Data.Adaptive

    //// Define keys for the possible attributes
    let SourceAttribKey = AttributeKey<_> "VideoManager_Source"
    let VideoAspectAttribKey = AttributeKey<_> "VideoManager_VideoAspect"
    let ShowControlsAttribKey = AttributeKey<_> "VideoManager_ShowControls"

    type Fabulous.XamarinForms.View with
        /// Describes a VideoView in the view
        static member inline VideoView
            (?source: aval<obj>, ?videoAspect: aval<VideoAspectMode>, ?showControls: aval<bool>,
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
            let attribCount = match source with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match videoAspect with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match showControls with Some _ -> attribCount + 1 | None -> attribCount

            // Populate the attributes of the base element
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

            let attribs = attribs.Retarget<VideoView>()

            let updater1 = ViewExtensions.ValueUpdater(source, (fun (target: VideoView) v -> target.Source <- v))
            let updater2 = ViewExtensions.ValueUpdater(videoAspect, (fun (target: VideoView) v -> target.VideoAspect <- v))
            let updater3 = ViewExtensions.ValueUpdater(showControls, (fun (target: VideoView) v -> target.ShowControls <- v))

            // Add our own attributes.
            match source with None -> () | Some v -> attribs.Add(SourceAttribKey, v, updater1)
            match videoAspect with None -> () | Some v -> attribs.Add(VideoAspectAttribKey, v, updater2)
            match showControls with None -> () | Some v -> attribs.Add(ShowControlsAttribKey, v, updater3)

            // The create method
            let create () = new VideoView()

            // The element
            ViewElement.Create(create, attribs.Close())


#if DEBUG
    let sample1 =
        View.VideoView(
            source = c (box "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"),
            showControls = c false,
            height = c 500.,
            width = c 200.)
#endif
