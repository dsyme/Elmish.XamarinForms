// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen

open System.IO

type InformationMessage = string
type WarningMessage = string
type ErrorMessage = string
type WorkflowResult<'T> = Result<'T * InformationMessage list * WarningMessage list, ErrorMessage list>
type ProgramResult = Result<InformationMessage list * WarningMessage list, ErrorMessage list>

module WorkflowResult =
    let bind f result =
        match result with
        | Error messages -> Error messages
        | Ok (data, messages, warnings) ->
            f data
            |> Result.map (fun (d, i, w) -> (d, List.append messages i, List.append warnings w))
            
    let map f result =
        match result with
        | Error messages -> Error messages
        | Ok (data, messages, warnings) -> Ok ((f data), messages, warnings)
        
    let tie f result =
        match result with
        | Error messages -> Error messages
        | Ok (data, messages, warnings) ->
            f data
            Ok (data, messages, warnings) 
        
    let toProgramResult result : ProgramResult =
        match result with
        | Error messages -> Error messages
        | Ok (_, messages, warnings) ->
            Ok (messages, warnings)
            
    let debug isDebugMode fileName =
        tie (fun d ->
            if isDebugMode then
                let json = Json.serialize d
                File.WriteAllText(fileName, json)
        )
            
    let ok data =
        Ok (data, [], [])
        
    let okWarnings data messages warnings =
        Ok (data, messages, warnings)
        
    let error messages =
        Error messages

