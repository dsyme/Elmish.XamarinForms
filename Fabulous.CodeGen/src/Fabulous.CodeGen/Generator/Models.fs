// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Generator

module Models =
    type AttributeData =
        { UniqueName: string
          Name: string }
    
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

    type BuildData =
        { Name: string
          BaseName: string option
          Members: BuildMember array }

    type CreateData =
        { Name: string
          FullName: string
          TypeToInstantiate: string }

    type UpdateMember =
        { UniqueName: string
          ModelType: string }
    
    type UpdateEvent =
        { Name: string
          ShortName: string
          UniqueName: string
          ConvertModelToValue: string
          RelatedProperties: string array }
        
    type UpdateAttachedProperty =
        { Name: string
          UniqueName: string
          DefaultValue: string
          OriginalType: string
          ModelType: string
          ConvertModelToValue: string
          UpdateCode: string }
        
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
          CollectionData: UpdatePropertyCollectionData option } 

    type UpdateData =
        { Name: string
          FullName: string
          BaseName: string option
          BaseFullName: string option
          ImmediateMembers : UpdateMember array
          Events: UpdateEvent array
          Properties: UpdateProperty array
          BuildData: BuildData }

    type ConstructData =
        { Name: string
          FullName: string
          Members: ConstructType array }

    type BuilderData =
        { Build: BuildData
          Create: CreateData option
          Update: UpdateData
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
          ConvertInputToModel: string }
        
    type GeneratorData =
        { Namespace: string
          Attributes: AttributeData array
          Builders: BuilderData array
          Viewers: ViewerData array
          Constructors: ConstructorData array
          ViewExtensions: ViewExtensionsData array }
