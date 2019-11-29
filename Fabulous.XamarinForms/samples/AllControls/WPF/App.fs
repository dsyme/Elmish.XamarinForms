namespace AllControls.WPF

open System

open Xamarin.Forms
open Xamarin.Forms.Platform.WPF

type MainWindow() = 
    inherit FormsApplicationPage()

module Main = 
    [<EntryPoint>]
    [<STAThread>]
    let main(_args) =

        let app = new System.Windows.Application()
        let assembliesToInclude = [| typeof<Xamarin.Forms.Maps.WPF.MapRenderer>.Assembly |]
        Xamarin.Forms.Forms.Init(assembliesToInclude);
        Xamarin.FormsMaps.Init("ENTER-YOUR-KEY-HERE");
        //OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init()
        //FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer=Nullable true)

        let window = MainWindow() 
        window.LoadApplication(new AllControls.App())

        app.Run(window)
