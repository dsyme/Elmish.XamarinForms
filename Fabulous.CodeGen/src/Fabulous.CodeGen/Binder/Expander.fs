// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Binder

open Fabulous.CodeGen
open Fabulous.CodeGen.AssemblyReader.Models
open Fabulous.CodeGen.Binder.Models

module Expander =
    let private tryFindType (boundTypes: BoundType array) typeFullName =
        boundTypes |> Array.tryFind (fun t2 -> t2.Id = typeFullName)
        
    let private getMembers (boundTypes: BoundType array) (getMemberBindings: BoundType -> 'a array) (setInherited: 'a -> 'a) (hierarchy: string array) =
        [ for typ in hierarchy do
              match tryFindType boundTypes typ with
              | None -> ()
              | Some t ->
                  let members = getMemberBindings t |> Array.map setInherited
                  for m in members do
                      yield m ]
        |> List.toArray
    
    let expandType (assemblyTypes: AssemblyType array) (boundTypes: BoundType array) (boundType: BoundType) =
        let hasNotOverridenEvent (boundEvent: BoundEvent) =
            boundType.Events
            |> Array.exists (fun e -> e.ShortName = boundEvent.ShortName)
            |> not
            
        let hasNotOverridenProperty (boundProperty: BoundProperty) =
            boundType.Properties
            |> Array.exists (fun e -> e.ShortName = boundProperty.ShortName)
            |> not
        
        let readerDataType = assemblyTypes |> Array.find (fun t -> t.FullName = boundType.Id)
        let hierarchy = readerDataType.InheritanceHierarchy
        let allBaseEvents = hierarchy |> getMembers boundTypes (fun t -> t.Events |> Array.filter hasNotOverridenEvent) (fun e -> { e with IsInherited = true })
        let allBaseProperties = hierarchy |> getMembers boundTypes (fun t -> t.Properties |> Array.filter hasNotOverridenProperty) (fun p -> { p with IsInherited = true })
        
        let firstBoundBaseType =
            hierarchy
            |> Array.tryPick (fun h ->
                boundTypes |> Array.tryFind (fun t -> t.Id = h)
            )
        
        { boundType with
            BaseTypeName = firstBoundBaseType |> Option.map (fun t -> t.Name)
            BaseTypeFullName = firstBoundBaseType  |> Option.map (fun t -> t.FullName)
            Events = Array.concat [ boundType.Events; allBaseEvents ]
            Properties = Array.concat [ boundType.Properties; allBaseProperties ] }
    
    /// Expands the bound model by adding all the inherited events and properties to all types
    /// This results in a verbose bound model that can be directly read by the generator
    let expand (assemblyTypes: AssemblyType array) (boundModel: BoundModel) : WorkflowResult<BoundModel> =
        let data =
            { boundModel with
                Types = boundModel.Types |> Array.map (expandType assemblyTypes boundModel.Types) }
        WorkflowResult.ok data
