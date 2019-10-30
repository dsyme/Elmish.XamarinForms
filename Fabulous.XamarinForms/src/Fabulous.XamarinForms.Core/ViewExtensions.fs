// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.XamarinForms.ViewHelpers
open Fabulous.XamarinForms.ViewUpdaters
open System.Collections.Generic

[<AutoOpen>]
module ViewExtensions =
    // The public API for extensions to define their incremental update logic
    type ViewElement with

        /// Update an event handler on a target control, given a previous and current view element description
        member inline source.UpdateEvent(prevOpt: ViewElement voption, attribKey: AttributeKey<'T>, targetEvent: IEvent<'T,'Args>) = 
            let prevValueOpt = match prevOpt with ValueNone -> ValueNone | ValueSome prev -> prev.TryGetAttributeKeyed<'T>(attribKey)
            let valueOpt = source.TryGetAttributeKeyed<'T>(attribKey)
            match prevValueOpt, valueOpt with
            | ValueSome prevValue, ValueSome currValue when identical prevValue currValue -> ()
            | ValueSome prevValue, ValueSome currValue -> targetEvent.RemoveHandler(prevValue); targetEvent.AddHandler(currValue)
            | ValueNone, ValueSome currValue -> targetEvent.AddHandler(currValue)
            | ValueSome prevValue, ValueNone -> targetEvent.RemoveHandler(prevValue)
            | ValueNone, ValueNone -> ()

        /// Update a primitive value on a target control, given a previous and current view element description
        member inline source.UpdatePrimitive(prevOpt: ViewElement voption, target: 'Target, attribKey: AttributeKey<'T>, setter: 'Target -> 'T -> unit, ?defaultValue: 'T) = 
            let prevValueOpt = match prevOpt with ValueNone -> ValueNone | ValueSome prev -> prev.TryGetAttributeKeyed<'T>(attribKey)
            let valueOpt = source.TryGetAttributeKeyed<'T>(attribKey)
            match prevValueOpt, valueOpt with
            | ValueSome prevValue, ValueSome newValue when prevValue = newValue -> ()
            | _, ValueSome newValue -> setter target newValue
            | ValueSome _, ValueNone -> setter target (defaultArg defaultValue Unchecked.defaultof<_>)
            | ValueNone, ValueNone -> ()

        /// Recursively update a nested view element on a target control, given a previous and current view element description
        member inline source.UpdateElement(prevOpt: ViewElement voption, target: 'Target, attribKey: AttributeKey<ViewElement>, getter: 'Target -> 'T, setter: 'Target -> 'T -> unit) = 
            let prevValueOpt = match prevOpt with ValueNone -> ValueNone | ValueSome prev -> prev.TryGetAttributeKeyed<ViewElement>(attribKey)
            let valueOpt = source.TryGetAttributeKeyed<ViewElement>(attribKey)
            match prevValueOpt, valueOpt with
            | ValueSome prevChild, ValueSome newChild when identical prevChild newChild -> ()
            | ValueSome prevChild, ValueSome newChild when ViewHelpers.canReuseView prevChild newChild ->
                newChild.UpdateIncremental(prevChild, getter target)
            | _, ValueSome newChild -> setter target (newChild.Create() :?> 'T)
            | ValueSome _, ValueNone -> setter target null
            | ValueNone, ValueNone -> ()

        /// Recursively update a collection of nested view element on a target control, given a previous and current view element description
        member inline source.UpdateElementCollection(prevOpt: ViewElement voption, attribKey: AttributeKey<seq<ViewElement>>, targetCollection: IList<'T>)  =
            let prevCollOpt = match prevOpt with ValueNone -> ValueNone | ValueSome prev -> prev.TryGetAttributeKeyed<_>(attribKey)
            let collOpt = source.TryGetAttributeKeyed<_>(attribKey)
            updateCollectionGeneric (ValueOption.map Seq.toArray prevCollOpt) (ValueOption.map Seq.toArray collOpt) targetCollection (fun x -> x.Create() :?> 'T) (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild

