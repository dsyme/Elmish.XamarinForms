// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.XamarinForms.ViewHelpers
open Fabulous.XamarinForms.ViewUpdaters
open FSharp.Data.Adaptive

[<AutoOpen>]
module ViewExtensions =
    /// Update an event handler on a target control, given a previous and current view element description
    let inline EventUpdater(valueOpt: aval<'Delegate> option) = 
        let mutable prevOpt = ValueNone 
        match valueOpt with 
        | None -> (fun _ _ -> ())
        | Some v -> 
            fun token (targetEvent: IEvent<'Delegate,'Args>) -> 
                let newValue =  v.GetValue(token)
                match prevOpt with
                | ValueSome prevValue when identical prevValue newValue -> ()
                | ValueSome prevValue -> targetEvent.RemoveHandler(prevValue); targetEvent.AddHandler(newValue)
                | ValueNone -> targetEvent.AddHandler(newValue)
                prevOpt <- ValueSome newValue

    /// Update a primitive value on a target control, given a previous and current view element description
    let inline PrimitiveUpdater(valueOpt: aval<_> option, setter: 'Target -> 'T -> unit (* , ?defaultValue: 'T *) ) = 
        match valueOpt with 
        | None -> (fun _ _ -> ())
        | Some v -> 
            let mutable prevOpt = ValueNone 
            fun token target -> 
                let newValue = v.GetValue(token)
                match prevOpt with
                | ValueSome prevValue when prevValue = newValue -> ()
                | _ -> setter target newValue
                // TODO: disappearing attributes
                // setter target (defaultArg defaultValue Unchecked.defaultof<_>)
                prevOpt <- ValueSome newValue

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
    let inline ElementCollectionUpdater(collOpt: ViewElement alist option)  =
        match collOpt with 
        | None -> (fun _ _ -> ())
        | Some coll -> 
            updateCollectionGeneric coll (fun token x -> x.Create(token) :?> 'T) (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild

