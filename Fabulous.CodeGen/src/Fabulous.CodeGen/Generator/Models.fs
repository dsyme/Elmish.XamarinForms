// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Generator

module Models =
    type AttributeData =
        { UniqueName: string
          Name: string
          ModelType: string }
    
    type ConstructType =
        { Name: string
          InputType: string }

    type ConstructorType =
        { Name: string
          InputType: string }

    type BuildMember =
        { Name: string
          UniqueName: string
          InputType: string
          ConvertInputToModel: string
          IsInherited: bool }

    type UpdatePropertyRelatedEvent =
        { Name: string
          UniqueName: string
          ShortName: string }

    type UpdateEvent =
        { Name: string
          ShortName: string
          UniqueName: string
          ConvertModelToValue: string
          ModelType: string
          ConvertInputToModel: string
          CanBeUpdated: bool }
        
    type UpdateAttachedProperty =
        { Name: string
          UniqueName: string
          ShortName: string
          DefaultValue: string
          OriginalType: string
          ModelType: string
          ConvertModelToValue: string
          UpdateCode: string 
          ConvertInputToModel: string
          CanBeUpdated: bool   }
        
    type UpdatePropertyCollectionData =
        { ElementType: string
          AttachedProperties: UpdateAttachedProperty array }
        
    type UpdateProperty =
        { Name: string
          UniqueName: string
          ShortName: string
          DefaultValue: string
          OriginalType: string
          ModelType: string
          ConvertModelToValue: string
          UpdateCode: string
          CollectionData: UpdatePropertyCollectionData option
          RelatedEvents: UpdatePropertyRelatedEvent array 
          ConvertInputToModel: string 
          CanBeUpdated: bool } 

    type UpdateMember =
        | UpdateEvent of UpdateEvent
        | UpdateProperty of UpdateProperty
        | UpdateAttachedProperty of UpdateAttachedProperty
        member x.UniqueName = match x with UpdateEvent e -> e.UniqueName | UpdateProperty p -> p.UniqueName | UpdateAttachedProperty p -> p.UniqueName
        member x.ShortName = match x with UpdateEvent e -> e.ShortName | UpdateProperty p -> p.ShortName | UpdateAttachedProperty p -> p.ShortName
        member x.ConvertInputToModel = match x with UpdateEvent e -> e.ConvertInputToModel | UpdateProperty p -> p.ConvertInputToModel | UpdateAttachedProperty p -> p.ConvertInputToModel
        member x.CanBeUpdated = match x with UpdateEvent e -> e.CanBeUpdated | UpdateProperty p -> p.CanBeUpdated | UpdateAttachedProperty p -> p.CanBeUpdated
    
    type BuildData =
        { Name: string
          BaseName: string option
          Members: BuildMember array
          FullName: string
          BaseFullName: string option
          UpdateMembers : UpdateMember array }

    type CreateData =
        { Name: string
          FullName: string
          TypeToInstantiate: string }

    type ConstructData =
        { Name: string
          FullName: string
          Members: ConstructType array }

    type BuilderData =
        { Build: BuildData
          Create: CreateData option
          Construct: ConstructData option }

    type ViewerMember =
        { Name: string
          UniqueName: string }

    type ViewerData =
        { Name: string
          FullName: string
          ViewerName: string
          GenericConstraint: string option
          InheritedViewerName: string option
          InheritedGenericConstraint: string option
          Members: ViewerMember array }

    type ConstructorData =
        { Name: string
          FullName: string
          Members: ConstructorType array }

    type ViewExtensionsData =
        { LowerUniqueName: string
          UniqueName: string
          InputType: string
          ConvertInputToModel: string
          TargetFullName: string
          UpdateMember: UpdateMember }
        
    type GeneratorData =
        { Namespace: string
          Attributes: AttributeData array
          Builders: BuilderData array
          Viewers: ViewerData array
          Constructors: ConstructorData array
          ViewExtensions: ViewExtensionsData array }
