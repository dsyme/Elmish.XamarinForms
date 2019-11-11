// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.Generator

open System.Diagnostics
open CommandLine
open Fabulous.CodeGen
    
module Program =
    type Options = {
        [<Option('m', "Mapping file", Required = true, HelpText = "Mapping file")>] MappingFile: string
        [<Option('o', "Output file", Required = true, HelpText = "Output file")>] OutputFile: string
        [<Option('d', "Debug", Required = false, HelpText = "Debug")>] Debug: bool
    }
    
    let tryReadOptions args =
        let options = CommandLine.Parser.Default.ParseArguments<Options>(args)
        match options with
        | :? Parsed<Options> as parsedOptions -> Some (parsedOptions.Value)
        | _ -> None
        
    let configuration =
        { baseTypeName = "Xamarin.Forms.BindableObject"
          propertyBaseType = "Xamarin.Forms.BindableProperty" }

    [<EntryPoint>]
    let main args =
        use consoleOutput = System.Console.OpenStandardOutput()
        use traceListener = new TextWriterTraceListener(consoleOutput)
        Trace.Listeners.Add(traceListener) |> ignore
        
        match tryReadOptions args with
        | None ->
            Trace.TraceError "Missing required arguments"
            1
        | Some options ->
            let flags = System.Collections.Generic.List<string>()
            flags.Add("CollectionView_Experimental")
            Xamarin.Forms.Device.SetFlags(flags)
            
            let result =
                Program.mkProgram
                    Reflection.loadAllAssemblies
                    Reflection.tryGetProperty
                    configuration
                |> Program.withDebug options.Debug
                |> Program.withWorkflow (fun workflow ->
                    { workflow with
                        optimize = XFOptimizer.optimize })
                |> Program.withReadAssembliesConfiguration (fun configuration ->
                    { configuration with
                        isTypeResolvable = XFConverters.isTypeResolvable
                        convertTypeName = XFConverters.convertTypeName
                        tryGetStringRepresentationOfDefaultValue = XFConverters.tryGetStringRepresentationOfDefaultValue })
                |> Program.run options.MappingFile options.OutputFile
                
            // Exit code
            match result with
            | Error errors ->
                errors |> List.iter Trace.TraceError
                1
                
            | Ok (messages, warnings) ->
                warnings |> List.iter Trace.TraceWarning
                messages |> List.iter Trace.TraceInformation
                0
