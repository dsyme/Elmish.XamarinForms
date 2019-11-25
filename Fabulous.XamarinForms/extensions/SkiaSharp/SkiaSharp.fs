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
        /// Describes a SkiaSharp canvas in the view
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

            let attribs = attribs.Retarget<SKCanvasView>()

            let updater1 = ViewExtensions.ValueUpdater(enableTouchEvents, (fun (target: SKCanvasView) v -> target.EnableTouchEvents <- v))
            let updater2 = ViewExtensions.ValueUpdater(ignorePixelScaling, (fun (target: SKCanvasView) v -> target.IgnorePixelScaling <- v))
            let updater3 = ViewExtensions.EventUpdater(paintSurface, (fun (target: SKCanvasView) -> target.PaintSurface))
            let updater4 = ViewExtensions.EventUpdater(touch, (fun (target: SKCanvasView) -> target.Touch))

            // Add our own attributes. They must have unique names which must match the names below.
            match enableTouchEvents with None -> () | Some v -> attribs.Add(CanvasEnableTouchEventsAttribKey, v, updater1) 
            match ignorePixelScaling with None -> () | Some v -> attribs.Add(IgnorePixelScalingAttribKey, v, updater2) 
            match paintSurface with None -> () | Some v -> attribs.Add(PaintSurfaceAttribKey, v, updater3)
            match touch with None -> () | Some v -> attribs.Add(TouchAttribKey, v, updater4)

            // The create method
            let create () = new SkiaSharp.Views.Forms.SKCanvasView()

            // The element
            ViewElement.Create(create, attribs.Close())


// Mucking about with a data-driven canvas view just to get a feel for what's possible
//
// In this demo the redraw is skipped for elements off-screen, e.g. if you resize the screen to make the canvas 
// viewport smaller.
//
// If you want to go further in speeding up rendering see this:
//    https://forums.xamarin.com/discussion/91436/how-to-achieve-advanced-performance
//
// ... I am quickly approaching the point where I need to speed up my rendering. Right now, I draw
// ... everything from scratch on every PaintCanvas call. I am pretty sure I need to stop doing that
// ... and instead break things up into layers where each layer is its own bitmap. A layer would only 
// ... redraw itself from primitives when it changes. All other times, it just blasts its saved bitmap
// ... onto the main canvas.
//
// This doesn't do anything like this

[<AutoOpen>]
module SkiaSharpExtension2 = 

    open Fabulous
    open FSharp.Data.Adaptive
    open Xamarin.Forms
    open SkiaSharp
    open SkiaSharp.Views.Forms
    open System.Collections.Generic

    // This is an experiment to show data-driven Canvas drawing
    type SKShape =
        | Draw of SKRect * (SKPaintSurfaceEventArgs -> unit)
        member this.Paint = (match this with Draw(_, paint) -> paint) 
        member this.BB = (match this with Draw(bb, _) -> bb) 
        static member Circle (centre: SKPoint, radius: single, width: single) =
            let bb = SKRect(centre.X - radius - width/2.0f, centre.Y - radius - width/2.0f, centre.X + radius + width/2.0f, centre.Y + radius + width/2.0f )
            Draw (bb, fun args -> 
                use paint = new SKPaint(Style = SKPaintStyle.Stroke, Color = Color.Red.ToSKColor(), StrokeWidth = width)
                args.Surface.Canvas.DrawCircle(centre, radius, paint))

    type CustomSKCanvasView() = 
        inherit SKCanvasView()
        member val Shapes : IList<SKShape> = null with get, set

    let CanvasEnableTouchEventsAttribKey = AttributeKey<_> "SKCanvasView2_EnableTouchEvents"
    let IgnorePixelScalingAttribKey = AttributeKey<_> "SKCanvasView2_IgnorePixelScaling"
    let ShapesAttribKey = AttributeKey<_> "SKCanvasView2_Shapes"
    let TouchAttribKey = AttributeKey<_> "SKCanvasView2_Touch"

    type Fabulous.XamarinForms.View with
        /// Describes a SkiaSharp canvas in the view
        static member SKCanvasView2(?shapes: alist<SKShape>, ?touch: aval<(SKTouchEventArgs -> unit)>, ?enableTouchEvents: aval<bool>, ?ignorePixelScaling: aval<bool>,
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
            let attribCount = match enableTouchEvents with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match ignorePixelScaling with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match shapes with Some _ -> attribCount + 1 | None -> attribCount
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

            let attribs = attribs.Retarget<CustomSKCanvasView>()

            let updater1 = ViewExtensions.ValueUpdater(enableTouchEvents, (fun (target: CustomSKCanvasView) v -> target.EnableTouchEvents <- v))
            let updater2 = ViewExtensions.ValueUpdater(ignorePixelScaling, (fun (target: CustomSKCanvasView) v -> target.IgnorePixelScaling <- v))
            let updater3 = 
                ViewExtensions.CollectionUpdater(shapes, 
                    (fun (target: CustomSKCanvasView) -> 
                        target.Shapes), 
                    (fun target v -> 
                        target.Shapes <- v),
                    add = (fun target p -> 
                        let viewport = SKRect(0.0f, 0.0f, float32 target.Width, float32 target.Height)
                        // If a point is added outside the visible region do not invalidate
                        if viewport.IntersectsWith(p.BB) then
                            target.InvalidateSurface()),
                    remove = (fun target p -> 
                        // If a point is removed outside the visible region do not invalidate
                        let viewport = SKRect(0.0f, 0.0f, float32 target.Width, float32 target.Height)
                        if viewport.IntersectsWith(p.BB) then
                            target.InvalidateSurface()))
            let updater4 = ViewExtensions.EventUpdater(touch, (fun (target: CustomSKCanvasView) -> target.Touch))

            // Add our own attributes. They must have unique names which must match the names below.
            match enableTouchEvents with None -> () | Some v -> attribs.Add(CanvasEnableTouchEventsAttribKey, v, updater1) 
            match ignorePixelScaling with None -> () | Some v -> attribs.Add(IgnorePixelScalingAttribKey, v, updater2) 
            match shapes with None -> () | Some v -> attribs.Add(ShapesAttribKey, v, updater3)
            match touch with None -> () | Some v -> attribs.Add(TouchAttribKey, v, updater4)

            // The create method
            let create () = 
                let t = new CustomSKCanvasView()
                t.PaintSurface.Add(fun args -> 
                    match t.Shapes with 
                    | null -> ()
                    | shapes -> 
                        // In this demo the redraw is skipped for elements off-screen, e.g. if you resize the screen to make the canvas 
                        // viewport smaller
                        let clipBounds = args.Surface.Canvas.LocalClipBounds
                        for shape in shapes do
                            if shape.BB.IntersectsWith(clipBounds) then 
                                System.Diagnostics.Debug.WriteLine(sprintf "repaint one shape, bb = %A" shape.BB)
                                shape.Paint args)
                t

            // The element
            ViewElement.Create(create, attribs.Close())

#if DEBUG 
    type State = 
        { mutable touches: int
          mutable paints: int }

(*
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
*)
#endif
