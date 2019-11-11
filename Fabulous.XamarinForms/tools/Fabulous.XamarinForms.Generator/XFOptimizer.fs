// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.Generator

open Fabulous.CodeGen
open Fabulous.CodeGen.Binder
open Fabulous.CodeGen.Binder.Models

module XFOptimizer =    
    /// Optimize command properties by asking for an F# function for the input type instead of ICommand
    module OptimizeCommands =
        let private canBeOptimized (boundProperty: BoundProperty) =
            boundProperty.ModelType = "System.Windows.Input.ICommand"
        
        let optimizeBoundProperty (boundType: BoundType) (boundProperty: BoundProperty) =
            [|
                // Accepts a function but don't apply it now
                { boundProperty with
                    InputType = "unit -> unit"
                    ModelType = "unit -> unit"
                    ConvertInputToModel = ""
                    ConvertModelToValue = ""
                    UpdateCode = sprintf "ViewUpdaters.updateCommand %sCanExecute (fun _target -> ()) (fun (target: %s) cmd -> target.%s <- cmd)" boundProperty.ShortName boundType.FullName boundProperty.Name }
                
                // Accepts a boolean to know when the function can be executed
                // Creates a Command for both CanExecute and the function
                { Name = sprintf "%sCanExecute" boundProperty.Name
                  ShortName = sprintf "%sCanExecute" boundProperty.ShortName
                  UniqueName = sprintf "%sCanExecute" boundProperty.UniqueName
                  CanBeUpdated = true
                  DefaultValue = "true"
                  OriginalType = "bool"
                  InputType = "bool"
                  ModelType = "bool"
                  ConvertInputToModel = ""
                  ConvertModelToValue = ""
                  UpdateCode = "(fun _ _ _ -> ())" 
                  //UpdateCode = sprintf "ViewUpdaters.updateCommand prev%sOpt curr%sOpt (fun _target -> ()) (fun (target: %s) cmd -> target.%s <- cmd)" boundProperty.UniqueName boundProperty.UniqueName boundType.FullName boundProperty.Name
                  CollectionData = None
                  IsInherited = false }
            |]
        
        let apply = Optimizer.propertyOptimizer (fun _ prop -> canBeOptimized prop) optimizeBoundProperty
    
    /// Optimize ImageSource properties by asking for InputTypes.Image instead of ImageSource  
    module OptimizeImageSource =
        let private canBeOptimized (boundProperty: BoundProperty) =
            boundProperty.InputType = boundProperty.ModelType
            && boundProperty.ConvertModelToValue = ""
            && boundProperty.UpdateCode = ""
            && boundProperty.ModelType = "Xamarin.Forms.ImageSource"
        
        let private optimizeBoundProperty (boundProperty: BoundProperty) =
            { boundProperty with
                InputType = "Fabulous.XamarinForms.InputTypes.Image"
                ModelType = "Fabulous.XamarinForms.InputTypes.Image"
                ConvertModelToValue = "ViewConverters.convertFabulousImageToXamarinFormsImageSource" }
        
        let apply = Optimizer.propertyOptimizer (fun _ prop -> canBeOptimized prop) (fun _ prop -> [| optimizeBoundProperty prop |])
    
    let optimize =
        let xfOptimize boundModel =
            boundModel
            |> OptimizeCommands.apply
            |> OptimizeImageSource.apply
        
        Optimizer.optimize
        >> WorkflowResult.map xfOptimize

