// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen

open System.IO
open System.Reflection
open Fabulous.CodeGen
open Fabulous.CodeGen.AssemblyReader
open Fabulous.CodeGen.AssemblyReader.Models
open Fabulous.CodeGen.Binder
open Fabulous.CodeGen.Binder.Models
open Fabulous.CodeGen.Generator
open Fabulous.CodeGen.Models
open Fabulous.CodeGen.Generator.Models

type Configuration =
    { /// The base type full name from which all UI controls inherit from (e.g. Xamarin.Forms.Element)
      baseTypeName: string
      
      /// The type full name of the object used for properties declared with a bindable/dependency property (e.g. Xamarin.Forms.BindableProperty)
      propertyBaseType: string }
    
type ReadAssembliesConfiguration =
    { loadAllAssembliesByReflection: seq<string> -> Assembly array
      tryGetAttachedPropertyByReflection: Assembly array -> string * string -> Models.ReflectionAttachedProperty option
      isTypeResolvable: string -> bool
      convertTypeName: string -> string
      tryGetStringRepresentationOfDefaultValue: obj -> string option }
    
type GeneratorConfiguration =
    { prepareData: BoundModel -> GeneratorData
      generate: GeneratorData -> string }
    
type WorkflowConfiguration =
    { loadBindings: string -> WorkflowResult<Bindings>
      readAssemblies: Configuration -> ReadAssembliesConfiguration -> string array -> WorkflowResult<AssemblyType array>
      bind: AssemblyType array -> Bindings -> WorkflowResult<BoundModel>
      optimize: BoundModel -> WorkflowResult<BoundModel>
      expand: AssemblyType array -> BoundModel -> WorkflowResult<BoundModel>
      generateCode: GeneratorConfiguration -> BoundModel -> WorkflowResult<string> }

type Program =
    { Debug: bool
      Configuration: Configuration
      Workflow: WorkflowConfiguration
      ReadAssembliesConfiguration: ReadAssembliesConfiguration
      GeneratorConfiguration: GeneratorConfiguration }
    
module private Functions =
    let readBindingsFile path =
        let bindings =
            path |> File.ReadAllText |> Json.deserialize<Bindings>
        WorkflowResult.ok bindings
    
    let readAssemblies configuration readAssembliesConfiguration assemblies =
        Reader.readAssemblies
            readAssembliesConfiguration.loadAllAssembliesByReflection
            readAssembliesConfiguration.tryGetAttachedPropertyByReflection
            readAssembliesConfiguration.isTypeResolvable
            readAssembliesConfiguration.convertTypeName
            readAssembliesConfiguration.tryGetStringRepresentationOfDefaultValue
            configuration.propertyBaseType
            configuration.baseTypeName
            assemblies
    
    let generateCode configuration bindings =
        CodeGenerator.generateCode
            configuration.prepareData
            configuration.generate
            bindings
    
    let runProgram program bindingsFile outputFile =        
        let readAssemblies (bindings: Bindings) =
            program.Workflow.readAssemblies program.Configuration program.ReadAssembliesConfiguration bindings.Assemblies
            |> WorkflowResult.debug program.Debug "1-assembly-types.json"
            |> WorkflowResult.map (fun x -> (x, bindings))
            
        let bind (readerData, bindings) =
            program.Workflow.bind readerData bindings
            |> WorkflowResult.debug program.Debug "2-bound-model.json"
            |> WorkflowResult.map (fun x -> (x, readerData))
            
        let optimize (boundModel, readerData) =
            program.Workflow.optimize boundModel
            |> WorkflowResult.debug program.Debug "3-optimized-bound-model.json"
            |> WorkflowResult.map (fun x -> (x, readerData))
            
        let expand (boundModel, readerData) =
            program.Workflow.expand readerData boundModel
            |> WorkflowResult.debug program.Debug "4-expanded-bound-model.json"
            
        let generateCode boundModel =
            program.Workflow.generateCode program.GeneratorConfiguration boundModel
            
        let write outputFile generatedCode =
            File.WriteAllText(outputFile, generatedCode)
        
        program.Workflow.loadBindings bindingsFile
        |> WorkflowResult.bind readAssemblies
        |> WorkflowResult.bind bind
        |> WorkflowResult.bind optimize
        |> WorkflowResult.bind expand
        |> WorkflowResult.bind generateCode
        |> WorkflowResult.tie (write outputFile)
        |> WorkflowResult.toProgramResult
    
module Program =
    let mkProgram loadAllAssembliesByReflection tryGetAttachedPropertyByReflection configuration =
        { Debug = false
          Configuration = configuration
          Workflow =
              { loadBindings = Functions.readBindingsFile
                readAssemblies = Functions.readAssemblies
                bind = Binder.bind
                optimize = Optimizer.optimize
                expand = Expander.expand
                generateCode = Functions.generateCode }
          ReadAssembliesConfiguration =
              { loadAllAssembliesByReflection = loadAllAssembliesByReflection
                tryGetAttachedPropertyByReflection = tryGetAttachedPropertyByReflection
                isTypeResolvable = (fun _ -> true)
                convertTypeName = Converters.convertTypeName
                tryGetStringRepresentationOfDefaultValue = Converters.tryGetStringRepresentationOfDefaultValue }
          GeneratorConfiguration =
              { prepareData = Preparer.prepareData
                generate = CodeGenerator.generate } }
        
    let withDebug debug program =
        { program with Debug = debug }
        
    let withWorkflow func program =
        { program with Workflow = func program.Workflow }
        
    let withReadAssembliesConfiguration func program =
        { program with ReadAssembliesConfiguration = func program.ReadAssembliesConfiguration }
        
    let withGeneratorConfiguration func program =
        { program with GeneratorConfiguration = func program.GeneratorConfiguration }
    
    let run bindingsFile outputFile program =
        Functions.runProgram program bindingsFile outputFile
