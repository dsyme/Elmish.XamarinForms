// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen

open System.IO
open System.Text.RegularExpressions
open Newtonsoft.Json
        
module Json =
    let jsonSettings =
        let settings = JsonSerializerSettings()
        settings.Formatting <- Formatting.Indented
        settings.Converters.Add(Microsoft.FSharpLu.Json.CompactUnionJsonConverter())
        settings
        
    let serialize obj = JsonConvert.SerializeObject(obj, jsonSettings)
    let deserialize<'a> str = JsonConvert.DeserializeObject<'a>(str, jsonSettings)
    
module Text =
    type TextWriter with
        member this.printf fmt = fprintf this fmt
        member this.printfn fmt = fprintfn this fmt
        
    let getValueOrDefault value defaultValue =
        match value with
        | None -> defaultValue
        | Some value when System.String.IsNullOrWhiteSpace value -> defaultValue
        | Some value -> value
        
    let eitherOrDefault v1 v2 defaultValue =
        let value1 = getValueOrDefault v1 ""
        let value2 = getValueOrDefault v2 ""
        
        match value1, value2 with
        | "", "" -> defaultValue
        | "", value2 -> value2
        | value1, _ -> value1

    let toLowerPascalCase (str : string) =
        match str with
        | null -> null
        | "" -> ""
        | x when x.Length = 1 -> x.ToLowerInvariant()
        | x -> string (System.Char.ToLowerInvariant(x.[0])) + x.Substring(1)
        
    let removeDotNetGenericNotation str =
        match str with
        | null -> null
        | x -> Regex.Replace(x, "`[0-9]*<",  "<")
        