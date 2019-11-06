// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.

namespace Fabulous.XamarinForms

open Fabulous
open Xamarin.Forms

type XamarinFormsHost(app: Application) =
    interface IHost with
        member __.GetRootView() =
            match app.MainPage with
            | null -> failwith "No root view"
            | rootView -> rootView :> obj 

        member __.SetRootView(rootView) =
            match rootView with
            | :? Page as page -> app.MainPage <- page
            | _ -> failwithf "Incorrect model type: expected a page but got a %O" (rootView.GetType())

/// Program module - functions to manipulate program instances
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module XamarinFormsProgram =    
    let private syncDispatch (dispatch: 'msg -> unit) =
        fun msg ->
            Device.BeginInvokeOnMainThread(fun () -> dispatch msg)
            
    let private syncAction (fn: unit -> unit) =
        fun () ->
            Device.BeginInvokeOnMainThread (fun () -> fn())

    let runWith app arg program =
        let host = XamarinFormsHost(app)

        program
        //|> Program.withCanReuseView ViewHelpers.canReuseView
        |> Program.withSyncDispatch syncDispatch
        |> Program.withSyncAction syncAction
        |> Program.runWithFabulous host arg
        
    let run app program =
        runWith app () program