namespace FabulousWeather.Droid

open Android.App
open Android.Content.PM
open Android.OS
open Xamarin.Forms.Platform.Android

[<Activity (Label = "Droid", Icon = "@mipmap/icon", MainLauncher = true, ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation))>]
type MainActivity() =
    inherit FormsApplicationActivity()
    override this.OnCreate (bundle: Bundle) =
        base.OnCreate (bundle)
        Xamarin.Forms.Forms.Init (this, bundle)
        this.LoadApplication (new FabulousWeather.App())