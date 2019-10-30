namespace Fabimals

open System
open Xamarin.Essentials

module Helper =
    let openUri (url) =
        Launcher.OpenAsync(new Uri(url))
        |> Async.AwaitTask
        |> Async.StartImmediate

