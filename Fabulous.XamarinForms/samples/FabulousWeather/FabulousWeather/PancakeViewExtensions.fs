namespace FabulousWeather

open Fabulous
open Fabulous.XamarinForms

[<AutoOpen>]
module PancakeViewExtensions =

    // Define keys for the possible attributes
    let backgroundGradientStartColorAttribKey = AttributeKey "BackgroundGradientStartColor"
    let backgroundGradientEndColorAttribKey = AttributeKey "BackgroundGradientEndColor"
    let pancakeContentAttribKey = AttributeKey "PancakeContent"
    let paddingAttribKey = AttributeKey "Padding"
    let cornerRadiusKey = AttributeKey "CornerRadius"
    let backgroundGradientAngleKey = AttributeKey "BackgroundGradientAngle"
    
    // Fully-qualified name to avoid extending by mistake
    // another View class (like Xamarin.Forms.View)
    type Fabulous.XamarinForms.View with
        /// Describes a ABC in the view
        /// The inline keyword is important for performance
        static member inline PancakeView(?content, ?backgroundGradientStartColor, ?backgroundGradientEndColor,
                                         ?cornerRadius, ?padding, ?backgroundGradientAngle,
                                         // inherited attributes common to all views
                                         ?gestureRecognizers, ?horizontalOptions, ?margin, ?verticalOptions, ?anchorX, ?anchorY, ?backgroundColor,
                                         ?behaviors, ?flowDirection, ?height, ?inputTransparent, ?isEnabled, ?isTabStop, ?isVisible, ?minimumHeight,
                                         ?minimumWidth, ?opacity, ?resources, ?rotation, ?rotationX, ?rotationY, ?scale, ?scaleX, ?scaleY, ?styles,
                                         ?styleSheets, ?tabIndex, ?translationX, ?translationY, ?visual, ?width, ?style, ?styleClasses, ?shellBackButtonBehavior,
                                         ?shellBackgroundColor, ?shellDisabledColor, ?shellForegroundColor, ?shellFlyoutBehavior, ?shellNavBarIsVisible,
                                         ?shellSearchHandler, ?shellTabBarBackgroundColor, ?shellTabBarDisabledColor, ?shellTabBarForegroundColor,
                                         ?shellTabBarIsVisible, ?shellTabBarTitleColor, ?shellTabBarUnselectedColor, ?shellTitleColor, ?shellTitleView,
                                         ?shellUnselectedColor, ?automationId, ?classId, ?effects, ?menu, ?ref, ?styleId, ?tag, ?focused, ?unfocused, ?created) =

            let attribCount = 0
            let attribCount = match backgroundGradientStartColor with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match backgroundGradientEndColor with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match content with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match cornerRadius with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match padding with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match backgroundGradientAngle with Some _ -> attribCount + 1 | None -> attribCount
            
            let attribs = ViewBuilders.BuildView(attribCount, ?gestureRecognizers=gestureRecognizers, ?horizontalOptions=horizontalOptions, ?margin=margin,
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

            match backgroundGradientStartColor with None -> () | Some v -> attribs.Add(backgroundGradientStartColorAttribKey, v)
            match backgroundGradientEndColor with None -> () | Some v -> attribs.Add(backgroundGradientEndColorAttribKey, v)
            match content with None -> () | Some v -> attribs.Add(pancakeContentAttribKey, v)
            match padding with None -> () | Some v -> attribs.Add(paddingAttribKey, v)
            match cornerRadius with None -> () | Some v -> attribs.Add(cornerRadiusKey, v)
            match backgroundGradientAngle with None -> () | Some v -> attribs.Add(backgroundGradientAngleKey, v)

            // The creation method
            let create () = Xamarin.Forms.PancakeView.PancakeView()

            // The incremental update method
            let update (prev: ViewElement voption) (source: ViewElement) (target: Xamarin.Forms.PancakeView.PancakeView) =
                ViewBuilders.UpdateView(prev,source,target)
                source.UpdateElement(prev,target, pancakeContentAttribKey,(fun target -> target.Content), (fun target v -> target.Content <- v))
                source.UpdatePrimitive(prev, target, backgroundGradientStartColorAttribKey, (fun target v -> target.BackgroundGradientStartColor <- v))
                source.UpdatePrimitive(prev, target, backgroundGradientEndColorAttribKey, (fun target v -> target.BackgroundGradientEndColor <- v))
                source.UpdatePrimitive(prev, target, paddingAttribKey, (fun target v -> target.Padding <- v))
                source.UpdatePrimitive(prev, target, cornerRadiusKey, (fun target v -> target.CornerRadius <- v))
                source.UpdatePrimitive(prev, target, backgroundGradientAngleKey, (fun target v -> target.BackgroundGradientAngle <- v))
                

            ViewElement.Create<Xamarin.Forms.PancakeView.PancakeView>(create, update, attribs)