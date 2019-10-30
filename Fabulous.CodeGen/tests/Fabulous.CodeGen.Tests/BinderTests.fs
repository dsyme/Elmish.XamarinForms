// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Tests

open NUnit.Framework
open FsUnit
open Fabulous.CodeGen.Binder

//module BinderTests =
//    let private bindPropertyTest propertyOverwriteData =
//        let typeFullName = "Xamarin.Forms.View"
        
//        let propertyReaderData =
//            { Name = "HorizontalOptions"
//              Type = "Xamarin.Forms.LayoutOptions"
//              DefaultValue = "Xamarin.Forms.LayoutOptions.Start" }

//        Binder.bindProperty typeFullName propertyReaderData propertyOverwriteData
    
//    [<Test>]
//    let ``bindProperty should return PropertyBinding using reader values if no overwrite values are given``() =            
//        let propertyOverwriteData =
//            { Position = None
//              Source = None
//              Name = None
//              ShortName = None
//              UniqueName = None
//              DefaultValue = None
//              InputType = None
//              ModelType = None
//              ConvertInputToModel = None
//              ConvertModelToValue = None }

//        bindPropertyTest propertyOverwriteData
//        |> should equal
//            { Name = "HorizontalOptions"
//              ShortName = "horizontalOptions"
//              UniqueName = "ViewHorizontalOptions"
//              DefaultValue = "Xamarin.Forms.LayoutOptions.Start"
//              InputType = "Xamarin.Forms.LayoutOptions"
//              ModelType = "Xamarin.Forms.LayoutOptions"
//              ConvertInputToModel = ""
//              ConvertModelToValue = "" }

//    [<Test>]
//    let ``bindProperty should return PropertyBinding using the given overwrite name``() =            
//        let propertyOverwriteData =
//            { Position = None
//              Source = None
//              Name = Some "HorizontalAlignment"
//              ShortName = None
//              UniqueName = None
//              DefaultValue = None
//              InputType = None
//              ModelType = None
//              ConvertInputToModel = None
//              ConvertModelToValue = None }

//        bindPropertyTest propertyOverwriteData
//        |> should equal
//            { Name = "HorizontalAlignment"
//              ShortName = "horizontalAlignment"
//              UniqueName = "ViewHorizontalAlignment"
//              DefaultValue = "Xamarin.Forms.LayoutOptions.Start"
//              InputType = "Xamarin.Forms.LayoutOptions"
//              ModelType = "Xamarin.Forms.LayoutOptions"
//              ConvertInputToModel = ""
//              ConvertModelToValue = "" }

//    [<Test>]
//    let ``bindProperty should return PropertyBinding using the given overwrite values``() =
//        let propertyOverwriteData =
//            { Position = None
//              Source = None
//              Name = Some "Name"
//              ShortName = Some "ShortName"
//              UniqueName = Some "UniqueName"
//              DefaultValue = Some "DefaultValue"
//              InputType = Some "InputType"
//              ModelType = Some "ModelType"
//              ConvertInputToModel = Some "ConvertInputToModel"
//              ConvertModelToValue = Some "ConvertModelToValue" }

//        bindPropertyTest propertyOverwriteData
//        |> should equal
//            { Name = "Name"
//              ShortName = "ShortName"
//              UniqueName = "UniqueName"
//              DefaultValue = "DefaultValue"
//              InputType = "InputType"
//              ModelType = "ModelType"
//              ConvertInputToModel = "ConvertInputToModel"
//              ConvertModelToValue = "ConvertModelToValue" }
            
//    let private bindEventTest eventOverwriteData =
//        let typeFullName = "Xamarin.Forms.ScrollView"
        
//        let eventReaderData =
//            { Name = "Scrolled"
//              Type = "System.EventHandler<Xamarin.Forms.ScrolledEventArgs>"
//              EventArgsType = "Xamarin.Forms.ScrolledEventArgs" }

//        Binder.bindEvent typeFullName eventReaderData eventOverwriteData
            
//    [<Test>]
//    let ``bindEvent should return EventBinding using reader values if no overwrite values are given``() =            
//        let eventOverwriteData =
//            { Position = None
//              Source = None
//              Name = None
//              ShortName = None
//              UniqueName = None
//              Type = None
//              EventArgsType = None }

//        bindEventTest eventOverwriteData
//        |> should equal
//            { Name = "Scrolled"
//              ShortName = "scrolled"
//              UniqueName = "ScrollViewScrolled"
//              Type = "System.EventHandler<Xamarin.Forms.ScrolledEventArgs>"
//              EventArgsType = "Xamarin.Forms.ScrolledEventArgs" }

//    [<Test>]
//    let ``bindEvent should return EventBinding using the given overwrite name``() =            
//        let eventOverwriteData =
//            { Position = None
//              Source = None
//              Name = Some "ScrollFinished"
//              ShortName = None
//              UniqueName = None
//              Type = None
//              EventArgsType = None }

//        bindEventTest eventOverwriteData
//        |> should equal
//            { Name = "ScrollFinished"
//              ShortName = "scrollFinished"
//              UniqueName = "ScrollViewScrollFinished"
//              Type = "System.EventHandler<Xamarin.Forms.ScrolledEventArgs>"
//              EventArgsType = "Xamarin.Forms.ScrolledEventArgs" }

//    [<Test>]
//    let ``bindEvent should return EventBinding using the given overwrite values``() =
//        let eventOverwriteData =
//            { Position = None
//              Source = None
//              Name = Some "Name"
//              ShortName = Some "ShortName"
//              UniqueName = Some "UniqueName"
//              Type = Some "Type"
//              EventArgsType = Some "EventArgsType" }

//        bindEventTest eventOverwriteData
//        |> should equal
//            { Name = "Name"
//              ShortName = "ShortName"
//              UniqueName = "UniqueName"
//              Type = "Type"
//              EventArgsType = "EventArgsType" }
            
//    let private bindAttachedPropertyTest attachedPropertyOverwriteData =
//        let typeFullName = "Xamarin.Forms.Grid"
//        let baseTargetTypeFullName = "Xamarin.Forms.Element"
        
//        let attachedPropertyReaderData : AttachedPropertyReaderData =
//            { Name = "Row"
//              Type = "int"
//              DefaultValue = "0" }

//        Binder.bindAttachedProperty typeFullName baseTargetTypeFullName attachedPropertyReaderData attachedPropertyOverwriteData
            
//    [<Test>]
//    let ``bindAttachedProperty should return AttachedPropertyBinding using reader values if no overwrite values are given``() =            
//        let attachedPropertyOverwriteData =
//            { Position = None
//              Source = None
//              TargetType = None
//              Name = None
//              UniqueName = None
//              DefaultValue = None
//              InputType = None
//              ModelType = None
//              ConvertInputToModel = None
//              ConvertModelToValue = None }

//        bindAttachedPropertyTest attachedPropertyOverwriteData
//        |> should equal
//            { TargetType = "Xamarin.Forms.Element"
//              Name = "Row"
//              UniqueName = "GridRow"
//              DefaultValue = "0"
//              InputType = "int"
//              ModelType = "int"
//              ConvertInputToModel = ""
//              ConvertModelToValue = "" }

//    [<Test>]
//    let ``bindAttachedProperty should return AttachedPropertyBinding using the given overwrite name``() =
//        let attachedPropertyOverwriteData =
//            { Position = None
//              Source = None
//              TargetType = None
//              Name = Some "RowIndex"
//              UniqueName = None
//              DefaultValue = None
//              InputType = None
//              ModelType = None
//              ConvertInputToModel = None
//              ConvertModelToValue = None }

//        bindAttachedPropertyTest attachedPropertyOverwriteData
//        |> should equal
//            { TargetType = "Xamarin.Forms.Element"
//              Name = "RowIndex"
//              UniqueName = "GridRowIndex"
//              DefaultValue = "0"
//              InputType = "int"
//              ModelType = "int"
//              ConvertInputToModel = ""
//              ConvertModelToValue = "" }

//    [<Test>]
//    let ``bindAttachedProperty should return AttachedPropertyBinding using the given overwrite values``() =
//        let attachedPropertyOverwriteData =
//            { Position = None
//              Source = None
//              TargetType = Some "TargetType"
//              Name = Some "Name"
//              UniqueName = Some "UniqueName"
//              DefaultValue = Some "DefaultValue"
//              InputType = Some "InputType"
//              ModelType = Some "ModelType"
//              ConvertInputToModel = Some "ConvertInputToModel"
//              ConvertModelToValue = Some "ConvertModelToValue" }

//        bindAttachedPropertyTest attachedPropertyOverwriteData
//        |> should equal
//            { TargetType = "TargetType"
//              Name = "Name"
//              UniqueName = "UniqueName"
//              DefaultValue = "DefaultValue"
//              InputType = "InputType"
//              ModelType = "ModelType"
//              ConvertInputToModel = "ConvertInputToModel"
//              ConvertModelToValue = "ConvertModelToValue" }
            
//    let private tryCreateEventTest name ``type`` eventArgsType =
//        let typeFullName = "Type"
        
//        let eventOverwriteData =
//            { Position = None
//              Source = None
//              Name = name
//              ShortName = None
//              UniqueName = None
//              Type = ``type``
//              EventArgsType = eventArgsType }

//        Binder.tryCreateEvent typeFullName eventOverwriteData
            
//    [<Test>]
//    let ``tryCreateEvent should return None if name is not given``() =
//        tryCreateEventTest None (Some "Type") (Some "EventArgsType")
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateEvent should return None if type is not given``() =
//        tryCreateEventTest (Some "Name") None (Some "EventArgsType")
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateEvent should return None if eventArgsType is not given``() =
//        tryCreateEventTest (Some "Name") (Some "Type") None
//        |> should equal None
        
//    [<Test>]
//    let ``tryCreateEvent should return Some if all mandatory overwrite values are given``() =
//        tryCreateEventTest (Some "Name") (Some "Type") (Some "EventArgsType")
//        |> should equal
//            (Some { Name = "Name"
//                    ShortName = "name"
//                    UniqueName = "TypeName"
//                    Type = "Type"
//                    EventArgsType = "EventArgsType" })
        
//    [<Test>]
//    let ``tryCreateEvent should return Some if all overwrite values are given``() =
//        let typeFullName = "Type"
        
//        let eventOverwriteData =
//            { Position = None
//              Source = None
//              Name = Some "Name"
//              ShortName = Some "ShortName"
//              UniqueName = Some "UniqueName"
//              Type = Some "Type"
//              EventArgsType = Some "EventArgsType" }

//        Binder.tryCreateEvent typeFullName eventOverwriteData
//        |> should equal
//            (Some { Name = "Name"
//                    ShortName = "ShortName"
//                    UniqueName = "UniqueName"
//                    Type = "Type"
//                    EventArgsType = "EventArgsType" })
            
//    let private tryCreatePropertyTest name defaultValue inputType modelType =
//        let typeFullName = "Type"
        
//        let propertyOverwriteData =
//            { Position = None
//              Source = None
//              Name = name
//              ShortName = None
//              UniqueName = None
//              DefaultValue = defaultValue
//              InputType = inputType
//              ModelType = modelType
//              ConvertInputToModel = None
//              ConvertModelToValue = None }

//        Binder.tryCreateProperty typeFullName propertyOverwriteData
            
//    [<Test>]
//    let ``tryCreateProperty should return None if name is not given``() =
//        tryCreatePropertyTest None (Some "DefaultValue") (Some "InputType") (Some "ModelType")
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateProperty should return None if defaultValue is not given``() =
//        tryCreatePropertyTest (Some "Name") None (Some "InputType") (Some "ModelType")
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateProperty should return None if inputType is not given``() =
//        tryCreatePropertyTest (Some "Name") (Some "DefaultValue") None (Some "ModelType")
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateProperty should return None if modelType is not given``() =
//        tryCreatePropertyTest (Some "Name") (Some "DefaultValue") (Some "InputType") None
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateProperty should return Some if all mandatory overwrite values are given``() =
//        tryCreatePropertyTest (Some "Name") (Some "DefaultValue") (Some "InputType") (Some "ModelType")
//        |> should equal
//            (Some { Name = "Name"
//                    ShortName = "name"
//                    UniqueName = "TypeName"
//                    DefaultValue = "DefaultValue"
//                    InputType = "InputType"
//                    ModelType = "ModelType"
//                    ConvertInputToModel = ""
//                    ConvertModelToValue = "" })
        
//    [<Test>]
//    let ``tryCreateProperty should return Some if all overwrite values are given``() =
//        let typeFullName = "Type"
        
//        let propertyOverwriteData =
//            { Position = None
//              Source = None
//              Name = Some "Name"
//              ShortName = Some "ShortName"
//              UniqueName = Some "UniqueName"
//              DefaultValue = Some "DefaultValue"
//              InputType = Some "InputType"
//              ModelType = Some "ModelType"
//              ConvertInputToModel = Some "ConvertInputToModel"
//              ConvertModelToValue = Some "ConvertModelToValue" }

//        Binder.tryCreateProperty typeFullName propertyOverwriteData
//        |> should equal
//            (Some { Name = "Name"
//                    ShortName = "ShortName"
//                    UniqueName = "UniqueName"
//                    DefaultValue = "DefaultValue"
//                    InputType = "InputType"
//                    ModelType = "ModelType"
//                    ConvertInputToModel = "ConvertInputToModel"
//                    ConvertModelToValue = "ConvertModelToValue" })
            
//    let private tryCreateAttachedPropertyTest name defaultValue inputType modelType =
//        let typeFullName = "Type"
//        let baseTargetTypeFullName = "TargetType"
        
//        let attachedPropertyOverwriteData : AttachedPropertyOverwriteData =
//            { Position = None
//              Source = None
//              TargetType = None
//              Name = name
//              UniqueName = None
//              DefaultValue = defaultValue
//              InputType = inputType
//              ModelType = modelType
//              ConvertInputToModel = None
//              ConvertModelToValue = None }

//        Binder.tryCreateAttachedProperty typeFullName baseTargetTypeFullName attachedPropertyOverwriteData
            
//    [<Test>]
//    let ``tryCreateAttachedProperty should return None if name is not given``() =
//        tryCreateAttachedPropertyTest None (Some "DefaultValue") (Some "InputType") (Some "ModelType")
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateAttachedProperty should return None if defaultValue is not given``() =
//        tryCreateAttachedPropertyTest (Some "Name") None (Some "InputType") (Some "ModelType")
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateAttachedProperty should return None if inputType is not given``() =
//        tryCreateAttachedPropertyTest (Some "Name") (Some "DefaultValue") None (Some "ModelType")
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateAttachedProperty should return None if modelType is not given``() =
//        tryCreateAttachedPropertyTest (Some "Name") (Some "DefaultValue") (Some "InputType") None
//        |> should equal None
            
//    [<Test>]
//    let ``tryCreateAttachedProperty should return Some if all mandatory overwrite values are given``() =
//        tryCreateAttachedPropertyTest (Some "Name") (Some "DefaultValue") (Some "InputType") (Some "ModelType")
//        |> should equal
//            (Some { TargetType = "TargetType"
//                    Name = "Name"
//                    UniqueName = "TypeName"
//                    DefaultValue = "DefaultValue"
//                    InputType = "InputType"
//                    ModelType = "ModelType"
//                    ConvertInputToModel = ""
//                    ConvertModelToValue = "" })
        
//    [<Test>]
//    let ``tryCreateAttachedProperty should return Some if all overwrite values are given``() =
//        let typeFullName = "Type"
//        let baseTargetTypeFullName = "TargetType"
        
//        let attachedPropertyOverwriteData =
//            { Position = None
//              Source = None
//              TargetType = Some "OtherTargetType"
//              Name = Some "Name"
//              UniqueName = Some "UniqueName"
//              DefaultValue = Some "DefaultValue"
//              InputType = Some "InputType"
//              ModelType = Some "ModelType"
//              ConvertInputToModel = Some "ConvertInputToModel"
//              ConvertModelToValue = Some "ConvertModelToValue" }

//        Binder.tryCreateAttachedProperty typeFullName baseTargetTypeFullName attachedPropertyOverwriteData
//        |> should equal
//            (Some { TargetType = "OtherTargetType"
//                    Name = "Name"
//                    UniqueName = "UniqueName"
//                    DefaultValue = "DefaultValue"
//                    InputType = "InputType"
//                    ModelType = "ModelType"
//                    ConvertInputToModel = "ConvertInputToModel"
//                    ConvertModelToValue = "ConvertModelToValue" })