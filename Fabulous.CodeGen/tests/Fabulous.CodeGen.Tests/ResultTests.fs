// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Tests

open Fabulous.CodeGen
open FsUnit
open NUnit.Framework

module ResultTests =
    let testTie result =
        let mutable sideEffectRan = false
        let mutable sideEffectData = None
        
        let sideEffect x =
            sideEffectRan <- true
            sideEffectData <- Some x
            
        let newResult = result |> WorkflowResult.tie sideEffect
        newResult, sideEffectRan, sideEffectData
    
    [<Test>]
    let ``WorkflowResult.bind should return Error when WorkflowResult is Error``() =
        let result = WorkflowResult.Error [ "A"; "B"; "C" ]
        let newData = WorkflowResult.Ok (987.6, [ "1"; "2"; "3" ], [ "4"; "5"; "6" ])
        let expectedResult = WorkflowResult<float>.Error [ "A"; "B"; "C" ]
        result |> WorkflowResult.bind (fun r -> newData) |> should equal expectedResult
    
    [<Test>]
    let ``WorkflowResult.bind should apply function and return Ok with merged messages and warnings when WorkflowResult is Ok``() =
        let result = WorkflowResult.Ok (123, [ "A"; "B"; "C" ], [ "D"; "E"; "F" ])
        let newData = WorkflowResult.Ok (987.6, [ "1"; "2"; "3" ], [ "4"; "5"; "6" ])
        let expectedResult = WorkflowResult.Ok (987.6, [ "A"; "B"; "C"; "1"; "2"; "3" ], [ "D"; "E"; "F"; "4"; "5"; "6" ])
        result |> WorkflowResult.bind (fun r -> newData) |> should equal expectedResult
    
    [<Test>]
    let ``WorkflowResult.map should return Error when WorkflowResult is Error``() =
        let result = WorkflowResult.Error [ "A"; "B"; "C" ]
        let expectedResult = WorkflowResult<int>.Error [ "A"; "B"; "C" ]
        result |> WorkflowResult.map (fun d -> d * 2) |> should equal expectedResult
    
    [<Test>]
    let ``WorkflowResult.map should apply function and return Ok when WorkflowResult is Ok``() =
        let data = 123
        let messages = [ "A"; "B"; "C" ]
        let warnings = [  "D"; "E"; "F" ]
        let result = WorkflowResult.Ok (data, messages, warnings)
        let expectedResult = WorkflowResult.Ok (data * 2, messages, warnings)
        result |> WorkflowResult.map (fun d -> d * 2) |> should equal expectedResult
    
    [<Test>]
    let ``WorkflowResult.tie should not execute side effect when WorkflowResult is Error``() =
        let result = WorkflowResult.Error [ "A"; "B"; "C" ]
        let _, sideEffectRan, _ = testTie result
        sideEffectRan |> should equal false
        
    [<Test>]
    let ``WorkflowResult.tie should return same WorkflowResult when WorkflowResult is Error``() =
        let result = WorkflowResult.Error [ "A"; "B"; "C" ]
        let newResult, _, _ = testTie result
        newResult |> should equal result
        
    [<Test>]
    let ``WorkflowResult.tie should execute side effect when WorkflowResult is Ok``() =
        let result = WorkflowResult.Ok (123, [ "A"; "B"; "C" ], [ "D"; "E"; "F" ])
        let _, sideEffectRan, _ = testTie result
        sideEffectRan |> should equal true
        
    [<Test>]
    let ``WorkflowResult.tie should pass data to side effect function when WorkflowResult is Ok``() =
        let data = 123
        let result = WorkflowResult.Ok (data, [ "A"; "B"; "C" ], [ "D"; "E"; "F" ])
        let _, _, sideEffectData = testTie result
        sideEffectData |> should equal (Some data)
        
    [<Test>]
    let ``WorkflowResult.tie should return same WorkflowResult when WorkflowResult is Ok``() =
        let result = WorkflowResult.Ok (123, [ "A"; "B"; "C" ], [ "D"; "E"; "F" ])
        let newResult, _, _ = testTie result
        newResult |> should equal result
        
    [<Test>]
    let ``WorkflowResult.toProgramResult should return ProgramResult.Ok when WorkflowResult is Ok``() =
        let data = 123
        let messages = [ "A"; "B"; "C" ]
        let warnings = [  "D"; "E"; "F" ]
        let result = WorkflowResult.Ok (data, messages, warnings)
        WorkflowResult.toProgramResult result |> should equal (ProgramResult.Ok (messages, warnings))
        
    [<Test>]
    let ``WorkflowResult.toProgramResult should return ProgramResult.Error when WorkflowResult is Error``() =
        let messages = [ "A"; "B"; "C" ]
        let result = WorkflowResult.Error messages
        WorkflowResult.toProgramResult result |> should equal (ProgramResult.Error messages)
        
    [<Test>]
    let ``WorkflowResult.ok should return Ok with the given data``() =
        let data = 123
        WorkflowResult.ok data |> should equal (Ok (data, [], []))
        
    [<Test>]
    let ``WorkflowResult.okWarnings should return Ok with the given data``() =
        let data = 123
        let messages = [ "A"; "B"; "C" ]
        let warnings = [  "D"; "E"; "F" ]
        WorkflowResult.okWarnings data messages warnings |> should equal (Ok (data, messages, warnings))
        
    [<Test>]
    let ``WorkflowResult.error should return Error with the given messages``() =
        let messages = [ "A"; "B"; "C" ]
        WorkflowResult.error messages |> should equal (Error messages)
