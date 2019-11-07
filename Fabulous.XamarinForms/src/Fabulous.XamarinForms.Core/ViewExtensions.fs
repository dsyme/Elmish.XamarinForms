// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.XamarinForms.ViewHelpers
open Fabulous.XamarinForms.ViewUpdaters
open FSharp.Data.Adaptive

[<AutoOpen>]
module ViewExtensions =
    /// Update an event handler on a target control, given a previous and current view element description
    let inline EventUpdater(valueOpt: aval<'Delegate> option, getter: 'Target -> IEvent<'Delegate,'Args>) = 
        match valueOpt with 
        | None -> (fun _ _ -> ())
        | Some v -> eventUpdater v getter

    /// Update a primitive value on a target control, given a previous and current view element description
    let inline PrimitiveUpdater(valueOpt: aval<_> option, setter: 'Target -> 'T -> unit (* , ?defaultValue: 'T *) ) = 
        match valueOpt with 
        | None -> (fun _ _ -> ())
        | Some v -> 
            valueUpdater v (fun (target: 'Target) prevOpt curr ->
                match prevOpt with
                | ValueSome prev when prev = curr -> ()
                | _ -> setter target curr)
                // TODO: disappearing attributes
                // setter target (defaultArg defaultValue Unchecked.defaultof<_>)

    /// Recursively update a nested view element on a target control, given a previous and current view element description
    let inline ElementUpdater(sourceOpt: ViewElement option, target: 'Target, getter: 'Target -> 'T, setter: 'Target -> 'T -> unit) = 
        match sourceOpt with 
        | None -> (fun _ -> ())
        | Some source -> 
            let updater = 
                { new ViewElementUpdater(source) with 
                        member __.OnCreated (target, element: obj) = setter (target :?> 'Target) (element :?> 'T) }
            updater.Update

    /// Recursively update a collection of nested view element on a target control, given a previous and current view element description
    let inline ElementCollectionUpdater(collOpt: ViewElement alist option, getter: 'Target -> 'TCollection)  =
        match collOpt with 
        | None -> (fun _ _ -> ())
        | Some coll -> 
            let updater = updateCollectionGeneric coll (fun token x -> x.Create(token) :?> 'T) (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild
            fun token target -> 
               updater token (getter target)

