// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.

// F# Compiler Daemon sample
//
// Sample use during development, assumes app has a reference to Fabulous.LiveUpdate:
//
// cd Fabulous\Samples\CounterApp\CounterApp
// adb -d forward  tcp:9867 tcp:9867
// dotnet run --project ..\..\..\src\Fabulous.Cli\Fabulous.Cli.fsproj -- --eval @out.args
// dotnet run --project ..\..\..\src\Fabulous.Cli\Fabulous.Cli.fsproj -- --watch --send 

module FSharpDaemon.Driver

open FSharp.Compiler.PortaCode.ProcessCommandLine
open System
open System.Collections.Generic

#if TEST
module MockForms = 
    open Xamarin.Forms
    open Xamarin.Forms.Internals
    type MockPlatformServices() = 
        interface IPlatformServices with 
            member __. GetMD5Hash(input) = raise (NotImplementedException())
            member __.GetNamedSize(size, targetElement, useOldSizes) = 10.0
            member __.OpenUriAction(uri) = raise (NotImplementedException())
            member __.IsInvokeRequired = false
            member __.get_RuntimePlatform() = Unchecked.defaultof<_> 
            member __.BeginInvokeOnMainThread(action: Action) = action.Invoke()
            member __.CreateTicker() = raise (NotImplementedException())
            member __.StartTimer(interval, callback) =  raise (NotImplementedException())
            member __.GetStreamAsync(uri, cancellationToken) = raise (NotImplementedException())
            member __.GetAssemblies() = [| |] // raise (NotImplementedException())
            member __.GetUserStoreForApplication() = raise (NotImplementedException())
            member __.QuitApplication() = raise (NotImplementedException())
            member __.GetNativeSize(view, widthConstraint, heightConstraint) = raise (NotImplementedException())

    type MockDeserializer() = 
        interface IDeserializer with
            member __.DeserializePropertiesAsync() = raise (NotImplementedException())
            member __.SerializePropertiesAsync(properties: IDictionary<string, obj>)  = raise (NotImplementedException())

    type MockResourcesProvider() = 
        interface ISystemResourcesProvider with 
            member __.GetSystemResources() = (ResourceDictionary() :> IResourceDictionary) // raise (NotImplementedException())

    type MockDeviceInfo() = 
        inherit DeviceInfo()
        override __.PixelScreenSize = raise (NotImplementedException())
        override __.ScaledScreenSize = raise (NotImplementedException())
        override __.ScalingFactor = raise (NotImplementedException())

    let Init() = 
        Device.Info <- new MockDeviceInfo()
        Device.PlatformServices <- new MockPlatformServices()
        DependencyService.Register<MockResourcesProvider>()
        DependencyService.Register<MockDeserializer>()
#endif

#if !TEST
[<EntryPoint>]
#endif
let main (argv: string[]) =
    try 
#if TEST
        MockForms.Init()
#endif
        ProcessCommandLine argv

    with e -> 
        printfn "Error: %s" (e.ToString())
        1
