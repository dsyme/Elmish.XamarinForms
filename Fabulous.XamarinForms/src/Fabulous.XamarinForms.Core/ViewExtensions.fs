// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.XamarinForms.ViewHelpers
open Fabulous.XamarinForms.ViewUpdaters
open FSharp.Data.Adaptive
open System.Collections.Generic

[<AutoOpen>]
module ViewExtensions =
    // The public API for extensions to define their incremental update logic
    type ViewElement with

        /// Update an event handler on a target control, given a previous and current view element description
        member inline source.EventUpdater(attribKey: AttributeKey<aval<'T>>) = 
            let mutable prevOpt = ValueNone 
            let valueOpt = source.TryGetAttributeKeyed<aval<'T>>(attribKey)
            match valueOpt with 
            | ValueNone -> (fun _ _ -> ())
            | ValueSome v -> 
                fun token (targetEvent: IEvent<'T,'Args>) -> 
                    let newValue = v.GetValue(token)
                    match prevOpt with
                    | ValueSome prevValue when identical prevValue newValue -> ()
                    | ValueSome prevValue -> targetEvent.RemoveHandler(prevValue); targetEvent.AddHandler(newValue)
                    | ValueNone -> targetEvent.AddHandler(newValue)
                    prevOpt <- ValueSome newValue

        /// Update a primitive value on a target control, given a previous and current view element description
        member inline source.PrimitiveUpdater(attribKey: AttributeKey<aval<'T>>, setter: 'Target -> 'T -> unit (* , ?defaultValue: 'T *) ) = 
            let mutable prevOpt = ValueNone 
            let valueOpt = source.TryGetAttributeKeyed<_>(attribKey)
            match valueOpt with 
            | ValueNone -> (fun _ _ -> ())
            | ValueSome v -> 
                fun token target -> 
                    let newValue = v.GetValue(token)
                    match prevOpt with
                    | ValueSome prevValue when prevValue = newValue -> ()
                    | _ -> setter target newValue
                    // TODO: disappearing attributes
                    // setter target (defaultArg defaultValue Unchecked.defaultof<_>)
                    prevOpt <- ValueSome newValue

        /// Recursively update a nested view element on a target control, given a previous and current view element description
        member inline source.ElementUpdater(target: 'Target, attribKey: AttributeKey<ViewElement>, getter: 'Target -> 'T, setter: 'Target -> 'T -> unit) = 
            let mutable created = false
            let valueOpt = source.TryGetAttributeKeyed<_>(attribKey)
            match valueOpt with 
            | ValueNone -> (fun _ _ -> ())
            | ValueSome v -> 
                fun token target ->
                    if created then 
                        v.Update(token, getter target)
                    else  
                        setter target (v.Create(token) :?> 'T)

        /// Recursively update a collection of nested view element on a target control, given a previous and current view element description
        member inline source.ElementCollectionUpdater(attribKey: AttributeKey<ViewElement alist>)  =
            let coll = source.GetAttributeKeyed<_>(attribKey)
            updateCollectionGeneric coll (fun token x -> x.Create(token) :?> 'T) (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild

