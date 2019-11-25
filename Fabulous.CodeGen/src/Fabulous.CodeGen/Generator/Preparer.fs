// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Generator

open Fabulous.CodeGen
open Fabulous.CodeGen.Binder.Models
open Fabulous.CodeGen.Generator.Models

module Preparer =
    let extractAttributes (boundTypes: BoundType array) : AttributeData array =
        (seq {
            for boundType in boundTypes do
               for e in boundType.Events do
                   yield { UniqueName = e.UniqueName; Name = e.Name; ModelType = e.ModelType }
               for p in boundType.Properties do
                   yield { UniqueName = p.UniqueName; Name = p.Name; ModelType = p.ModelType }
                   
                   match p.CollectionData with
                   | None -> ()
                   | Some cd ->
                       for ap in cd.AttachedProperties do
                           yield { UniqueName = ap.UniqueName; Name = ap.Name; ModelType = ap.ModelType } } : seq<AttributeData>)
        |> Seq.distinctBy (fun a -> a.UniqueName)
        |> Seq.toArray

    let toUpdateEvent (boundType: BoundType) (e: BoundEvent) =
        { Name = e.Name
          ShortName = e.ShortName
          UniqueName = e.UniqueName
          ConvertModelToValue = e.ConvertModelToValue
          ConvertInputToModel = e.ConvertInputToModel
          ModelType = e.ModelType
          CanBeUpdated = e.CanBeUpdated }

    //let toUpdateMember (boundType: BoundType) (m: BoundMember) = [| |]
    let toUpdateMember (boundType: BoundType) (m: BoundMember) =
        match m with 
        | BoundEvent e -> toUpdateEvent boundType e |> UpdateEvent
        
        | BoundProperty p -> 
            { Name = p.Name
              UniqueName = p.UniqueName
              ShortName = p.ShortName
              DefaultValue = p.DefaultValue
              OriginalType = p.OriginalType
              ModelType = p.ModelType
              ConvertModelToValue = p.ConvertModelToValue
              ConvertInputToModel = p.ConvertInputToModel
              UpdateCode = p.UpdateCode
              CanBeUpdated = p.CanBeUpdated
              RelatedEvents = [| for e in boundType.Events do for rp in e.RelatedProperties do if p.UniqueName = p.UniqueName then yield { Name = e.Name; ShortName = e.ShortName; UniqueName = e.UniqueName } |]
              CollectionData =
                  p.CollectionData
                  |> Option.map (fun cd ->
                      { ElementType = cd.ElementType
                        AttachedProperties =
                            cd.AttachedProperties
                            |> Array.map (fun ap ->
                                { Name = ap.Name
                                  ShortName = ap.ShortName
                                  UniqueName = ap.UniqueName
                                  DefaultValue = ap.DefaultValue
                                  OriginalType = ap.OriginalType
                                  ModelType = ap.ModelType
                                  ConvertModelToValue = ap.ConvertModelToValue
                                  ConvertInputToModel = ap.ConvertInputToModel
                                  UpdateCode = ap.UpdateCode
                                  CanBeUpdated = ap.CanBeUpdated }) }) }
            |> UpdateProperty
        | BoundAttachedProperty ap -> 
            UpdateAttachedProperty
                { Name = ap.Name
                  ShortName = ap.ShortName
                  UniqueName = ap.UniqueName
                  DefaultValue = ap.DefaultValue
                  OriginalType = ap.OriginalType
                  ModelType = ap.ModelType
                  CanBeUpdated = ap.CanBeUpdated
                  ConvertModelToValue = ap.ConvertModelToValue
                  ConvertInputToModel = ap.ConvertInputToModel
                  UpdateCode = ap.UpdateCode }
        
    let toBuildData (boundType: BoundType) =
        let toBuildMember (m: IBoundConstructorMember) : BuildMember =
            { Name = m.ShortName
              UniqueName = m.UniqueName
              InputType = m.InputType
              ConvertInputToModel = m.ConvertInputToModel
              IsInherited = m.IsInherited }
        
        let properties = boundType.Properties |> Array.map toBuildMember
        let events = boundType.Events |> Array.map toBuildMember
        let members = Array.concat [ properties; events ]
        
        let immediateEvents = boundType.Events |> Array.filter (fun e -> not e.IsInherited)
        let immediateProperties = boundType.Properties |> Array.filter (fun p -> not p.IsInherited)
        
        let updateEvents = immediateEvents |> Array.map (BoundEvent >> toUpdateMember boundType)
        let updateProperties = immediateProperties |> Array.map (BoundProperty >> toUpdateMember boundType)
        
        { Name = boundType.Name
          FullName = boundType.FullName
          BaseName = boundType.BaseTypeName
          BaseFullName = boundType.BaseTypeFullName
          UpdateMembers = Array.append updateEvents updateProperties
          Members = members }

    let toCreateData (boundType: BoundType) =
        { Name = boundType.Name
          FullName = boundType.FullName
          TypeToInstantiate = boundType.TypeToInstantiate }

    let toConstructData (boundType: BoundType) : ConstructData =
        let properties = boundType.Properties |> Array.map (fun p -> { Name = p.ShortName; InputType = p.InputType } : ConstructType)
        let events = boundType.Events |> Array.map (fun e -> { Name = e.ShortName; InputType = e.InputType } : ConstructType)
        let members = Array.concat [ properties; events ]
        
        { Name = boundType.Name
          FullName = boundType.FullName
          Members = members }
    
    let toBuilderData (boundType: BoundType) =
        { Build = toBuildData boundType
          Create = if boundType.CanBeInstantiated then Some (toCreateData boundType) else None
          Construct = if boundType.CanBeInstantiated then Some (toConstructData boundType) else None }

    let toViewerData (boundType: BoundType) : ViewerData =
        let properties = boundType.Properties |> Array.filter (fun p -> not p.IsInherited) |> Array.map (fun p -> { Name = p.Name; UniqueName = p.UniqueName })
        let events = boundType.Events |> Array.filter (fun e -> not e.IsInherited) |> Array.map (fun e -> { Name = e.Name; UniqueName = e.UniqueName })
        let members = Array.concat [ properties; events ]
            
        { Name = boundType.Name
          FullName = boundType.FullName
          ViewerName = sprintf "%sViewer" boundType.Name
          GenericConstraint = boundType.GenericConstraint
          InheritedViewerName = boundType.BaseTypeName |> Option.map (sprintf "%sViewer")
          InheritedGenericConstraint = boundType.BaseGenericConstraint
          Members = members }
    
    type TempConstructorMember = { Name: string; ShortName: string; InputType: string }
    
    let toConstructorData (boundType: BoundType) =
        let properties = boundType.Properties |> Array.map (fun p -> { Name = p.Name; ShortName = p.ShortName; InputType = p.InputType })
        let events = boundType.Events |> Array.map (fun e -> { Name = e.Name; ShortName = e.ShortName; InputType = e.InputType })
        let allMembers = Array.concat [ properties; events ]
        
        let primaryMembers =
            match boundType.PrimaryConstructorMembers with
            | None -> [||]
            | Some memberNames -> memberNames |> Array.choose (fun n -> allMembers |> Array.tryFind (fun m -> m.Name = n))
            
        let otherMembers =
            allMembers
            |> Array.except primaryMembers
            |> Array.sortBy (fun m -> m.ShortName)
        
        let members =
            [ primaryMembers; otherMembers ]
            |> Array.concat
            |> Array.map (fun p -> { Name = p.ShortName; InputType = p.InputType })
        
        { Name = boundType.Name
          FullName = boundType.FullName
          Members = members }

    let getViewExtensionsData (types: BoundType array) =
        let toViewExtensionsMember (boundType: BoundType) (m: BoundMember) =
            let um = toUpdateMember boundType m 
            { LowerUniqueName = Text.toLowerPascalCase m.UniqueName
              UniqueName = m.UniqueName
              InputType = m.InputType
              ConvertInputToModel = m.ConvertInputToModel 
              TargetFullName = boundType.FullName
              UpdateMember = um }
            
        [| for typ in types do
               for e in typ.Events do
                   yield toViewExtensionsMember typ (BoundEvent e)
               for p in typ.Properties do
                   yield toViewExtensionsMember typ (BoundProperty p)
                   
                   match p.CollectionData with
                   | None -> ()
                   | Some cd ->
                       for a in cd.AttachedProperties do
                           yield toViewExtensionsMember typ (BoundAttachedProperty a)  |]
        |> Array.groupBy (fun y -> y.UniqueName)
        |> Array.map (fun (_, members) -> members |> Array.head)
    
    let prepareData (boundModel: BoundModel) =
        { Namespace = boundModel.OutputNamespace
          Attributes = extractAttributes boundModel.Types
          Builders = boundModel.Types |> Array.map toBuilderData
          Viewers = boundModel.Types |> Array.map toViewerData
          Constructors = boundModel.Types |> Array.filter (fun t -> t.CanBeInstantiated) |> Array.map toConstructorData
          ViewExtensions = boundModel.Types |> getViewExtensionsData }

