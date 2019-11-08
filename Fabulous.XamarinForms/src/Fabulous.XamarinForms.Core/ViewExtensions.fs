// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.XamarinForms.ViewHelpers
open Fabulous.XamarinForms.ViewUpdaters
open FSharp.Data.Adaptive
open System

type ViewExtensions() =
    /// Update an event handler on a target control, given a previous and current view element description
    static member inline EventUpdater(valueOpt: aval<'T> option, getter: 'Target -> IEvent<'Delegate,'Args>, makeDelegate) = 
        match valueOpt with 
        | None -> (fun _ _ -> ())
        | Some v -> eventUpdater v makeDelegate getter

    /// Update an event handler on a target control, given a previous and current view element description
    static member inline EventUpdater(valueOpt: aval<unit -> unit> option, getter: 'Target -> IEvent<EventHandler,EventArgs>) = 
        ViewExtensions.EventUpdater(valueOpt, getter, makeEventHandlerNonGeneric)

    static member inline EventUpdater(valueOpt: aval<'Args -> unit> option, getter: 'Target -> IEvent<EventHandler<'Args>,'Args>) = 
        ViewExtensions.EventUpdater(valueOpt, getter, makeEventHandler)

    /// Update a primitive value on a target control, given a previous and current view element description
    static member inline PrimitiveUpdater(valueOpt: aval<_> option, setter: 'Target -> 'T -> unit (* , ?defaultValue: 'T *) ) = 
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
    static member inline ElementUpdater(sourceOpt: ViewElement option, target: 'Target, getter: 'Target -> 'T, setter: 'Target -> 'T -> unit) = 
        match sourceOpt with 
        | None -> (fun _ -> ())
        | Some source -> 
            let updater = 
                { new ViewElementUpdater(source) with 
                        member __.OnCreated (target, element: obj) = setter (target :?> 'Target) (element :?> 'T) }
            updater.Update

    /// Recursively update a collection of nested view element on a target control, given a previous and current view element description
    static member inline ElementCollectionUpdater(collOpt: ViewElement alist option, getter: 'Target -> 'TCollection)  =
        match collOpt with 
        | None -> (fun _ _ -> ())
        | Some coll -> 
            let updater = updateCollectionGeneric coll (fun token x -> x.Create(token) :?> 'T) (fun _ _ _ -> ()) ViewHelpers.canReuseView updateChild
            fun token target -> 
               updater token (getter target)

