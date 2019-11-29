namespace AllControls.MacOS

open System
open AppKit
open Foundation
open Xamarin.Forms
open Xamarin.Forms.Platform.MacOS

[<Register("AppDelegate")>]
type AppDelegate() =
    inherit FormsApplicationDelegate()
    let  style = NSWindowStyle.Closable ||| NSWindowStyle.Resizable ||| NSWindowStyle.Titled

    let  rect = new CoreGraphics.CGRect(nfloat 200.0, nfloat 1000.0, nfloat 1024.0, nfloat 768.0)

    let window = new NSWindow(rect, style, NSBackingStore.Buffered, false, Title = "Xamarin.Forms on Mac!", TitleVisibility = NSWindowTitleVisibility.Hidden)

    override __.MainWindow = window

    override this.DidFinishLaunching(notification: NSNotification) =
        Forms.Init()
        Xamarin.FormsMaps.Init() 
        //OxyPlot.Xamarin.Forms.Platform. .Android.PlotViewRenderer.Init()
        //FFImageLoading.Forms.Platform.CachedImageRenderer.Init()
        this.LoadApplication(new AllControls.App())

        base.DidFinishLaunching(notification)

    override __.WillTerminate(notification: NSNotification) =
        // Insert code here to tear down your application
        ()

    override __.ApplicationShouldTerminateAfterLastWindowClosed(_) = true

module EntryClass = 
    [<EntryPoint>]
    let Main(args: string[]) =
        NSApplication.Init() |> ignore
        NSApplication.SharedApplication.Delegate <- new AppDelegate()
        NSApplication.Main(args)
        0
