// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

[<AutoOpen>]
module VideoManagerExtension =

    open Fabulous
    open MediaManager.Forms
    open MediaManager.Video

    //// Define keys for the possible attributes
    let SourceAttribKey = AttributeKey<_> "VideoManager_Source"
    let VideoAspectAttribKey = AttributeKey<_> "VideoManager_VideoAspect"
    let ShowControlsAttribKey = AttributeKey<_> "VideoManager_ShowControls"

    type Fabulous.XamarinForms.View with
        /// Describes a VideoView in the view
        static member inline VideoView
            (?source: obj, ?videoAspect: VideoAspectMode , ?showControls: bool,
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

            // Add our own attributes.
            match source with None -> () | Some v -> attribs.Add(SourceAttribKey, v)
            match videoAspect with None -> () | Some v -> attribs.Add(VideoAspectAttribKey, v)
            match showControls with None -> () | Some v -> attribs.Add(ShowControlsAttribKey, v)

            // The create method
            let create () = new VideoView()

            // The update method
            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: VideoView) =
                ViewBuilders.UpdateView(prevOpt, source, target)
                source.UpdatePrimitive(prevOpt, target, SourceAttribKey, (fun target v -> target.Source <- v))
                source.UpdatePrimitive(prevOpt, target, VideoAspectAttribKey, (fun target v -> target.VideoAspect <- v))
                source.UpdatePrimitive(prevOpt, target, ShowControlsAttribKey, (fun target v -> target.ShowControls <- v))

            // The element
            ViewElement.Create(create, update, attribs)

#if DEBUG
    let sample1 =
        View.VideoView(
            source = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
            showControls = false,
            height = 500.,
            width = 200.)
#endif

