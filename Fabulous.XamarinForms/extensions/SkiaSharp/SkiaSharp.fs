// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms 

[<AutoOpen>]
module SkiaSharpExtension = 

    open Fabulous
    open FSharp.Data.Adaptive
    open Xamarin.Forms
    open SkiaSharp
    open SkiaSharp.Views.Forms

    let CanvasEnableTouchEventsAttribKey = AttributeKey<_> "SKCanvas_EnableTouchEvents"
    let IgnorePixelScalingAttribKey = AttributeKey<_> "SKCanvas_IgnorePixelScaling"
    let PaintSurfaceAttribKey = AttributeKey<_> "SKCanvas_PaintSurface"
    let TouchAttribKey = AttributeKey<_> "SKCanvas_Touch"

    type Fabulous.XamarinForms.View with
        /// Describes a Map in the view
        static member SKCanvasView(?paintSurface: aval<(SKPaintSurfaceEventArgs -> unit)>, ?touch: aval<(SKTouchEventArgs -> unit)>, ?enableTouchEvents: aval<bool>, ?ignorePixelScaling: aval<bool>,
                                   ?invalidate: aval<bool>,
                                   // inherited attributes common to all views
                                   ?gestureRecognizers, ?horizontalOptions, ?margin, ?verticalOptions, ?anchorX, ?anchorY, ?backgroundColor,
                                   ?behaviors, ?flowDirection, ?height, ?inputTransparent, ?isEnabled, ?isTabStop, ?isVisible, ?minimumHeight,
                                   ?minimumWidth, ?opacity, ?resources, ?rotation, ?rotationX, ?rotationY, ?scale, ?scaleX, ?scaleY, ?styles,
                                   ?styleSheets, ?tabIndex, ?translationX, ?translationY, ?visual, ?width, ?style, ?styleClasses, ?shellBackButtonBehavior,
                                   ?shellBackgroundColor, ?shellDisabledColor, ?shellForegroundColor, ?shellFlyoutBehavior, ?shellNavBarIsVisible,
                                   ?shellSearchHandler, ?shellTabBarBackgroundColor, ?shellTabBarDisabledColor, ?shellTabBarForegroundColor,
                                   ?shellTabBarIsVisible, ?shellTabBarTitleColor, ?shellTabBarUnselectedColor, ?shellTitleColor, ?shellTitleView,
                                   ?shellUnselectedColor, ?automationId, ?classId, ?effects, ?menu, ?ref, ?styleId, ?tag, ?focused, ?unfocused, ?created) =
            let paintSurface = paintSurface |> Option.map (AVal.map (fun f -> System.EventHandler<_>(fun _ args -> f args)))
            let touch = touch |> Option.map (AVal.map (fun f -> System.EventHandler<_>(fun _ args -> f args)))
            // Count the number of additional attributes
            let attribCount = 0
            let attribCount = match enableTouchEvents with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match ignorePixelScaling with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match paintSurface with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match touch with Some _ -> attribCount + 1 | None -> attribCount

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

            // Add our own attributes. They must have unique names which must match the names below.
            match enableTouchEvents with None -> () | Some v -> attribs.Add(CanvasEnableTouchEventsAttribKey, v) 
            match ignorePixelScaling with None -> () | Some v -> attribs.Add(IgnorePixelScalingAttribKey, v) 
            match paintSurface with None -> () | Some v -> attribs.Add(PaintSurfaceAttribKey, v)
            match touch with None -> () | Some v -> attribs.Add(TouchAttribKey, v)

            let viewUpdater = ViewBuilders.UpdaterView (?gestureRecognizers=gestureRecognizers, ?horizontalOptions=horizontalOptions, ?margin=margin,
                                       ?verticalOptions=verticalOptions, ?anchorX=anchorX, ?anchorY=anchorY, ?backgroundColor=backgroundColor, ?behaviors=behaviors,
                                       ?flowDirection=flowDirection, ?height=height, ?inputTransparent=inputTransparent, ?isEnabled=isEnabled, ?isTabStop=isTabStop,
                                       ?isVisible=isVisible, ?minimumHeight=minimumHeight, ?minimumWidth=minimumWidth, ?opacity=opacity, (*?resources=resources,*)
                                       ?rotation=rotation, ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?scaleX=scaleX, ?scaleY=scaleY, (*?styles=styles,*)
                                       (*?styleSheets=styleSheets, *) 
                                       ?tabIndex=tabIndex, ?translationX=translationX, ?translationY=translationY, ?visual=visual, ?width=width,
                                       ?style=style, ?styleClasses=styleClasses, ?shellBackButtonBehavior=shellBackButtonBehavior, ?shellBackgroundColor=shellBackgroundColor,
                                       ?shellDisabledColor=shellDisabledColor, ?shellForegroundColor=shellForegroundColor, ?shellFlyoutBehavior=shellFlyoutBehavior,
                                       ?shellNavBarIsVisible=shellNavBarIsVisible, ?shellSearchHandler=shellSearchHandler, ?shellTabBarBackgroundColor=shellTabBarBackgroundColor,
                                       ?shellTabBarDisabledColor=shellTabBarDisabledColor, ?shellTabBarForegroundColor=shellTabBarForegroundColor,
                                       ?shellTabBarIsVisible=shellTabBarIsVisible, ?shellTabBarTitleColor=shellTabBarTitleColor, ?shellTabBarUnselectedColor=shellTabBarUnselectedColor,
                                       ?shellTitleColor=shellTitleColor, ?shellTitleView=shellTitleView, ?shellUnselectedColor=shellUnselectedColor, ?automationId=automationId,
                                       ?classId=classId, ?effects=effects, ?menu=menu, ?ref=ref, ?styleId=styleId, ?tag=tag, ?focused=focused, ?unfocused=unfocused, ?created=created)

            let updater1 = ViewExtensions.PrimitiveUpdater(enableTouchEvents, (fun (target: SKCanvasView) v -> target.EnableTouchEvents <- v))
            let updater2 = ViewExtensions.PrimitiveUpdater(ignorePixelScaling, (fun (target: SKCanvasView) v -> target.IgnorePixelScaling <- v))
            let updater3 = ViewExtensions.EventUpdater(paintSurface)
            let updater4 = ViewExtensions.EventUpdater(touch)

            // The create method
            let create () = new SkiaSharp.Views.Forms.SKCanvasView()

            // The update method
            let update token (target: SKCanvasView) = 
                viewUpdater token target
                updater1 token target
                updater2 token target
                updater3 token target.PaintSurface
                updater4 token target.Touch

            // The element
            ViewElement.Create(create, update, attribs.Close())

#if DEBUG 
    type State = 
        { mutable touches: int
          mutable paints: int }

    let sample1 = 
        View.Stateful(
            (fun () -> { touches = 0; paints = 0 }), 
            (fun state -> 
                View.SKCanvasView(enableTouchEvents = c true, 
                    paintSurface = c (fun args -> 
                        let info = args.Info
                        let surface = args.Surface
                        let canvas = surface.Canvas
                        state.paints <- state.paints + 1
                        printfn "paint event, total paints on this control = %d" state.paints

                        canvas.Clear() 
                        use paint = new SKPaint(Style = SKPaintStyle.Stroke, Color = Color.Red.ToSKColor(), StrokeWidth = 25.0f)
                        canvas.DrawCircle(float32 (info.Width / 2), float32 (info.Height / 2), 100.0f, paint)
                    ),
                    touch = c (fun args -> 
                        state.touches <- state.touches + 1
                        printfn "touch event at (%f, %f), total touches on this control = %d" args.Location.X args.Location.Y state.touches
                    )
            )))
#endif
