// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Binder

open Fabulous.CodeGen
open Fabulous.CodeGen.Binder.Models

module Optimizer =
    /// Apply a function to all properties
    let propertyOptimizer (canBeOptimizedFunc: BoundType -> BoundProperty -> bool) (optimizeFunc: BoundType -> BoundProperty -> BoundProperty[]) =
        let optimizeBoundProperty (boundType: BoundType) (boundProperty: BoundProperty) =                
            if canBeOptimizedFunc boundType boundProperty then
                optimizeFunc boundType boundProperty
            else
                [| boundProperty |]
        
        let optimizeBoundType (boundType: BoundType) =
            { boundType with
                Properties = boundType.Properties |> Array.collect (optimizeBoundProperty boundType) }
        
        fun boundModel ->
            { boundModel with
                Types = boundModel.Types |> Array.map optimizeBoundType }
            
    /// Apply a function to all events
    let eventOptimizer (canBeOptimizedFunc: BoundType -> BoundEvent -> bool) (optimizeFunc: BoundType -> BoundEvent -> BoundEvent[]) =
        let optimizeBoundEvent (boundType: BoundType) (boundEvent: BoundEvent) =                
            if canBeOptimizedFunc boundType boundEvent then
                optimizeFunc boundType boundEvent
            else
                [| boundEvent |]
        
        let optimizeBoundType (boundType: BoundType) =
            { boundType with
                Events = boundType.Events |> Array.collect (optimizeBoundEvent boundType) }
        
        fun boundModel ->
            { boundModel with
                Types = boundModel.Types |> Array.map optimizeBoundType }
            
    
    /// Apply a function to all types
    let typeOptimizer (canBeOptimizedFunc: BoundType -> bool) (optimizeFunc: BoundType -> BoundType[]) =
        let optimizeType boundType =
            if canBeOptimizedFunc boundType then
                optimizeFunc boundType
            else
                [| boundType |]
        
        fun boundModel ->
            { boundModel with
                Types = boundModel.Types |> Array.collect optimizeType }
    
    /// When a type references another one available in the bound model, the referenced type is replaced with ViewElement
    /// This enables recursive incremental updates
    module OptimizeKnownTypes =
        let private optimizeBoundType (knownTypes: string array) (boundType: BoundType) =
            let replaceKnownTypeWithViewElement knownTypes typeName =
                if Array.contains typeName knownTypes then "ViewElement" else typeName
            
            let properties = boundType.Properties |> Array.map (fun p ->
                { p with
                    InputType = replaceKnownTypeWithViewElement knownTypes p.InputType
                    ModelType = replaceKnownTypeWithViewElement knownTypes p.ModelType
                    CollectionData =
                        p.CollectionData
                        |> Option.map (fun cd ->
                            { cd with
                               AttachedProperties =
                                   cd.AttachedProperties |> Array.map (fun a ->
                                       { a with
                                           InputType = replaceKnownTypeWithViewElement knownTypes a.InputType
                                           ModelType = replaceKnownTypeWithViewElement knownTypes a.ModelType }) }) }
            )
            
            { boundType with Properties = properties }
        
        let apply (boundModel: BoundModel) =
            let knownTypes = boundModel.Types |> Array.map (fun t -> t.FullName)
            typeOptimizer (fun _ -> true) (fun typ -> [| optimizeBoundType knownTypes typ |]) boundModel
            
    /// Optimizes storing list of data for efficiency
    /// When a property/attached property accepts a list as an input type (and no specific instructions were given in the Bindings file),
    /// the model type is changed to be an array with the corresponding ConvertInputToModel function
    module OptimizeLists =
        let private canBeOptimized (boundProperty: BoundProperty) =
            boundProperty.InputType.EndsWith(" list") && boundProperty.ModelType.EndsWith(" list") && boundProperty.ConvertInputToModel = ""
        
        let private optimizeBoundProperty (boundProperty: BoundProperty) =
            { boundProperty with
                ModelType = boundProperty.ModelType.Replace(" list",  " array")
                ConvertInputToModel = "Array.ofList"
                CollectionData =
                    boundProperty.CollectionData
                    |> Option.map (fun cd ->
                        { cd with
                            AttachedProperties =
                                cd.AttachedProperties |> Array.map (fun p ->
                                    if p.InputType.EndsWith(" list") && p.ModelType.EndsWith(" list") && p.ConvertInputToModel = "" then
                                        { p with
                                            ModelType =  p.ModelType.Replace(" list",  " array")
                                            ConvertInputToModel = "Array.ofList" }
                                    else
                                        p
                                ) }) }
        
        let apply = propertyOptimizer (fun _ prop -> canBeOptimized prop) (fun _ prop -> [| optimizeBoundProperty prop |])
            
    /// Converts .NET generic notation into F# generic notation
    module OptimizeGenerics =
        let private optimizeBoundType (boundType: BoundType) =
           { boundType with
                FullName = boundType.FullName.Replace("`1", "<'T>")
                TypeToInstantiate = boundType.TypeToInstantiate.Replace("`1", "<'T>") }
        
        let apply = typeOptimizer (fun _ -> true) (fun typ -> [| optimizeBoundType typ |])
        
    /// Optimize events by storing them as EventHandlers
    module OptimizeEvents =
        let private canBeOptimized (boundEvent: BoundEvent) =
            boundEvent.ConvertModelToValue = ""
        
        let private optimizeBoundEvent (boundEvent: BoundEvent) =
            { boundEvent with
                  ConvertModelToValue =
                      match boundEvent.ModelType with
                      | "unit -> unit" -> "makeEventHandlerNonGeneric"
                      | _ when not (System.String.IsNullOrWhiteSpace(boundEvent.EventArgsType)) -> "makeEventHandler"
                      | _ -> boundEvent.ConvertModelToValue }
            
        let apply = eventOptimizer (fun _ evt -> canBeOptimized evt) (fun _ evt -> [| optimizeBoundEvent evt |])
            
    /// Uses the member's name (property/event/attached property) as the unique name where possible
    /// If there's a name collision between two members with the same name but different model types, keep using the complete unique name
    /// This allows to write things like View.Entry().Text("XYZ") instead of View.Entry().EntryText("XYZ") 
    module OptimizeAttributeKeys =
        let inline canBeOptimized (keysToUpdate: string list) (item: ^T) =
            let name = (^T: (member Name: string) item)
            keysToUpdate |> List.contains name
            
        let inline applyFn keysToUpdate fn item =
            match canBeOptimized keysToUpdate item with
            | false -> item
            | true -> fn item
        
        let private optimizeBoundAttachedProperty (boundType: BoundType) (boundAttachedProperty: BoundAttachedProperty) =
            { boundAttachedProperty with
                UniqueName = sprintf "%s%s" boundType.Name boundAttachedProperty.Name }
        
        let private optimizeBoundProperty (keysToUpdate: string list) (boundType: BoundType) (boundProperty: BoundProperty) =
            let optimizeAttachedProperty =
                applyFn keysToUpdate (optimizeBoundAttachedProperty boundType)
            
            let collectionData =
                boundProperty.CollectionData
                |> Option.map (fun collection ->
                    { collection with
                        AttachedProperties = collection.AttachedProperties |> Array.map optimizeAttachedProperty })
                
            { boundProperty with
                UniqueName = sprintf "%s%s" boundType.Name boundProperty.Name
                CollectionData = collectionData }
        
        let private optimizeBoundEvent (boundType: BoundType) (boundEvent: BoundEvent) =
            { boundEvent with
                UniqueName = sprintf "%s%s" boundType.Name boundEvent.Name }
            
        let private optimizeType (keysToUpdate: string list) (boundType: BoundType) =
            let optimizeProperty =
                applyFn keysToUpdate (optimizeBoundProperty keysToUpdate boundType)
            let optimizeEvent =
                applyFn keysToUpdate (optimizeBoundEvent boundType)
                
            { boundType with
                Properties = boundType.Properties |> Array.map optimizeProperty
                Events = boundType.Events |> Array.map optimizeEvent }
        
        let apply (boundModel: BoundModel) =
            let allKeys =
                [ for t in boundModel.Types do
                      for p in t.Properties do
                          yield (p.Name, p.ModelType)
                          
                          match p.CollectionData with
                          | None -> ()
                          | Some collection ->
                              for ap in collection.AttachedProperties do
                                  yield (ap.Name, ap.ModelType)
                                  
                      for e in t.Events do
                          yield (e.Name, e.ModelType) ]
            
            let keysToUpdate =
                allKeys
                |> List.distinct
                |> List.groupBy fst
                |> List.filter (fun (_, items) -> items.Length > 1)
                |> List.map fst
            
            if keysToUpdate.Length = 0 then
                boundModel
            else
                { boundModel with
                    Types = boundModel.Types |> Array.map (optimizeType keysToUpdate) }

    /// Applies all optimizations to the bound model
    let optimize boundModel : WorkflowResult<BoundModel> =
        boundModel
        |> OptimizeKnownTypes.apply
        |> OptimizeLists.apply
        |> OptimizeGenerics.apply
        |> OptimizeEvents.apply
        |> OptimizeAttributeKeys.apply
        |> WorkflowResult.ok