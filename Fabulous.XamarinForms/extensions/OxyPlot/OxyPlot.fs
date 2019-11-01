// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms 

[<AutoOpen>]
module OxyPlotExtension = 

    open System
    open Xamarin.Forms
    open OxyPlot
    open OxyPlot.Axes
    open OxyPlot.Series
    open OxyPlot.Xamarin.Forms
    open Fabulous

    let ModelAttribKey = AttributeKey<_> "OxyPlot_Model"
    let ControllerAttribKey = AttributeKey<_> "OxyPlot_Controller"

    type Fabulous.XamarinForms.View with
        /// Describes a Map in the view
        static member inline PlotView
            (model: PlotModel, ?controller: PlotController,
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
            let attribCount = 1
            let attribCount = match controller with Some _ -> attribCount + 1 | None -> attribCount 

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
            attribs.Add(ModelAttribKey, model) 
            match controller with None -> () | Some v -> attribs.Add(ControllerAttribKey, v) 

            // The create method
            let create () = PlotView()

            // The update method
            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: PlotView) = 
                ViewBuilders.UpdateView (prevOpt, source, target)
                source.UpdatePrimitive(prevOpt, target, ModelAttribKey, (fun target v -> target.Model <- v))
                source.UpdatePrimitive(prevOpt, target, ControllerAttribKey, (fun target v -> target.Controller <- v))

            // The element
            ViewElement.Create(create, update, attribs.Close())

#if DEBUG 

    module Sample1 = 
        let plotModelCos =
            let model = PlotModel(Title = "Example 1")
            model.Series.Add(new OxyPlot.Series.FunctionSeries(Math.Cos, 0.0, 10.0, 0.1, "cos(x)"))
            model

        let plotModelHeatMap =
            let model = PlotModel (Title = "Heatmap")
            model.Axes.Add(LinearColorAxis (Palette = OxyPalettes.Rainbow(100)))
            let singleData = [ for x in 0 .. 99 -> Math.Exp((-1.0 / 2.0) * Math.Pow(((double)x - 50.0) / 20.0, 2.0)) ]
            let data = Array2D.init 100 100 (fun x y -> singleData.[x] * singleData.[(y + 30) % 100] * 100.0)
            let heatMapSeries =
                HeatMapSeries(X0 = 0.0, X1 = 99.0, Y0 = 0.0, Y1 = 99.0, Interpolate = true,
                                RenderMethod = HeatMapRenderMethod.Bitmap, Data = data)
            model.Series.Add(heatMapSeries)
            model

        let plotModels = [ plotModelCos; plotModelHeatMap ]

        let sample = 
            View.CarouselPage(
                [ for m in plotModels ->
                    View.ContentPage(
                        View.PlotView(m,
                                      horizontalOptions=LayoutOptions.FillAndExpand, 
                                      verticalOptions=LayoutOptions.FillAndExpand)) ])
#endif
