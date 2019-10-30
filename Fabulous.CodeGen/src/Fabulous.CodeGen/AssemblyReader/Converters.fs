// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.AssemblyReader

open System

module Converters =
    /// Converts the type name to another type name (e.g. System.Boolean => bool)
    let convertTypeName typeName =
        match typeName with
        | "System.Boolean" -> "bool"
        | "System.SByte" -> "sbyte"
        | "System.Byte" -> "byte"
        | "System.Int16" -> "int16"
        | "System.UInt16" -> "uint16"
        | "System.Int32" -> "int"
        | "System.UInt32" -> "uint32"
        | "System.Int64" -> "int64"
        | "System.UInt64" -> "uint64"
        | "System.BigInteger" -> "bigint"
        | "System.Double" -> "float"
        | "System.Single" -> "single"
        | "System.Decimal" -> "decimal"
        | "System.String" -> "string"
        | "System.Object" -> "obj"
        | "System.Collections.Generic.IList<System.Object>" -> "obj list"
        | "System.Collections.IList" -> "obj list"
        | _ -> typeName
        
    let inline numberWithDecimalsToString literal v =
        let str = v.ToString()
        let separator = if not (str.Contains(".")) then "." else ""
        str + separator + literal
        
    /// Gets the string representation of the default value
    let tryGetStringRepresentationOfDefaultValue (defaultValue: obj) =
        match defaultValue with
        | null -> Some "null"
        | :? bool as b when b = true -> Some "true"
        | :? bool as b when b = false -> Some "false"
        | :? sbyte as sbyte -> Some (sbyte.ToString() + "y")
        | :? byte as byte -> Some (byte.ToString() + "uy")
        | :? int16 as int16 -> Some (int16.ToString() + "s")
        | :? uint16 as uint16 -> Some (uint16.ToString() + "us")
        | :? int as int -> Some (int.ToString())
        | :? uint32 as uint32 -> Some (uint32.ToString() + "u")
        | :? int64 as int64 -> Some (int64.ToString() + "L")
        | :? uint64 as uint64 -> Some (uint64.ToString() + "UL")
        | :? bigint as bigint -> Some (bigint.ToString() + "I")
        | :? float as float when Double.IsNaN(float) -> Some "System.Double.NaN"
        | :? double as double when Double.IsNaN(double) -> Some "System.Double.NaN"
        | :? float32 as float32 when Single.IsNaN(float32) -> Some "System.Single.NaN"
        | :? single as single when Single.IsNaN(single) -> Some "System.Single.NaN"
        | :? float as float -> Some (numberWithDecimalsToString "" float)
        | :? double as double -> Some (numberWithDecimalsToString "" double)
        | :? float32 as float32 -> Some (numberWithDecimalsToString "f" float32)
        | :? single as single -> Some (numberWithDecimalsToString "f" single)
        | :? decimal as decimal -> Some (numberWithDecimalsToString "m" decimal)
        | :? DateTime as dateTime when dateTime = DateTime.MinValue -> Some "System.DateTime.MinValue"
        | :? DateTime as dateTime when dateTime = DateTime.MaxValue -> Some "System.DateTime.MaxValue"
        | :? DateTime as dateTime -> Some (sprintf "System.DateTime(%i, %i, %i)" dateTime.Year dateTime.Month dateTime.Day)
        | :? string as string when string = "" -> Some "System.String.Empty"
        | :? string as string -> Some (sprintf "\"%s\"" string)
        | :? TimeSpan as timeSpan when timeSpan = TimeSpan.Zero -> Some "System.TimeSpan.Zero"
        | value when value.GetType().IsEnum ->
            let typ = defaultValue.GetType()
            let valueName = Enum.GetName(typ, defaultValue)
            match valueName with
            | null -> None
            | _ -> Some (sprintf "%s.%s" (typ.FullName.Replace("+", ".")) valueName)
        | _ -> Some (defaultValue.ToString())