// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.AssemblyReader

module Models =
    /// An event extracted by the Assembly Reader
    type AssemblyTypeEvent =
        { /// The name of the event (e.g. TextChanged)
          Name: string
          
          /// The type of the event args (e.g. Xamarin.Forms.TextChangedEventArgs)
          EventArgsType: string
          
          /// The type of the event handler (e.g. System.EventHandler<Xamarin.Forms.TextChangedEventArgs>)
          EventHandlerType: string }
    
    /// An attached property extracted by the Assembly Reader 
    type AssemblyTypeAttachedProperty =
        { /// The name of the attached property (e.g. Row)
          Name: string
          
          /// The type of the attached property (e.g. int)
          Type: string
          
          /// The default value of the attached property (e.g. 0)
          DefaultValue: string }
    
    /// A property extracted by the Assembly Reader
    type AssemblyTypeProperty =
        { /// The name of the property (e.g. Text)
          Name: string
          
          /// The type of the property (e.g. string)
          Type: string
          
          /// The type of the items in case the property is a list (e.g. IGridList<View> => View)
          CollectionElementType: string option
          
          /// The default value of the property (e.g. null)
          DefaultValue: string }
    
    /// A type extracted by the Assembly Reader
    type AssemblyType =
        { /// The full name of the type
          FullName: string
          
          /// The name of the assembly containing the type
          AssemblyName: string
          
          /// Indicates if the type can be instantiated (non-abstract type with a public parameter-less constructor)
          CanBeInstantiated: bool
          
          /// The types inherited by this type
          /// From first to last, the closest inherited type to the base
          InheritanceHierarchy: string array
          
          /// The extracted events
          Events: AssemblyTypeEvent array
          
          /// The extracted properties
          Properties: AssemblyTypeProperty array
          
          /// The extracted attached properties
          AttachedProperties: AssemblyTypeAttachedProperty array }
        
    /// An attached property extracted by reflection
    type ReflectionAttachedProperty =
        { /// The name of the attached property
          Name: string
          
          /// The type of the attached property
          Type: string
          
          /// The default value of the attached property
          DefaultValue: obj }

