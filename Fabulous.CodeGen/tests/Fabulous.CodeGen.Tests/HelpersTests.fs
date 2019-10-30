// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Tests.HelpersTests

open NUnit.Framework
open FsUnit
open Fabulous.CodeGen

module TextTests =
    [<Test>]
    let ``Text.getValueOrDefault should return the default value when no value is given``() =
        Text.getValueOrDefault None "DEFAULT VALUE" |> should equal "DEFAULT VALUE"

    [<TestCase(null)>]
    [<TestCase("")>]
    [<TestCase("     ")>]
    let ``Text.getValueOrDefault should return the default value when the given value is null, empty or only whitespace``(value) =
        Text.getValueOrDefault (Some value) "DEFAULT VALUE" |> should equal "DEFAULT VALUE"
        
    [<Test>]
    let ``Text.getValueOrDefault should return the value when a non-null/non-whitespace value is given``() =
        Text.getValueOrDefault (Some "VALUE") "DEFAULT VALUE" |> should equal "VALUE"
        
    [<Test>]
    let ``Text.eitherOrDefault should return the first value when a non-null/non-whitespace value is given, regardless of the second value``() =
        Text.eitherOrDefault (Some "FIRST VALUE") (Some "SECOND VALUE") "DEFAULT VALUE" |> should equal "FIRST VALUE"
        
    [<Test>]
    let ``Text.eitherOrDefault should return the second value when a non-null/non-whitespace value is given, and no first value is given``() =
        Text.eitherOrDefault None (Some "SECOND VALUE") "DEFAULT VALUE" |> should equal "SECOND VALUE"
        
    [<TestCase(null)>]
    [<TestCase("")>]
    [<TestCase("     ")>]
    let ``Text.eitherOrDefault should return the second value when a non-null/non-whitespace value is given, and the first value is null, empty or only whitespace``(firstValue) =
        Text.eitherOrDefault (Some firstValue) (Some "SECOND VALUE") "DEFAULT VALUE" |> should equal "SECOND VALUE"
        
    [<Test>]
    let ``Text.eitherOrDefault should return the default value when neither values are given``() =
        Text.eitherOrDefault None None "DEFAULT VALUE" |> should equal "DEFAULT VALUE"
        
    [<TestCase(null, null)>]
    [<TestCase(null, "")>]
    [<TestCase(null, "     ")>]
    [<TestCase("", null)>]
    [<TestCase("", "")>]
    [<TestCase("", "     ")>]
    [<TestCase("     ", null)>]
    [<TestCase("     ", "")>]
    [<TestCase("     ", "     ")>]
    let ``Text.eitherOrDefault should return the default value when both values are null, empty or only whitespace``(firstValue, secondValue) =
        Text.eitherOrDefault (Some firstValue) (Some secondValue) "DEFAULT VALUE" |> should equal "DEFAULT VALUE"
        
    [<TestCase(null, null)>]
    [<TestCase("", "")>]
    [<TestCase("A", "a")>]
    [<TestCase("a", "a")>]
    [<TestCase("1", "1")>]
    [<TestCase("TextProperty", "textProperty")>]
    let ``Text.toLowerPascalCase should return the string with the first character lowercase, if possible``(str, expected: string) =
        Text.toLowerPascalCase str |> should equal expected
        
    [<TestCase(null, null)>]
    [<TestCase("", "")>]
    [<TestCase("IList`1<System.Object>", "IList<System.Object>")>]
    [<TestCase("System.Tuple`2<System.String, System.EventHandler`1<Xamarin.Forms.TextChangedEventArgs>>", "System.Tuple<System.String, System.EventHandler<Xamarin.Forms.TextChangedEventArgs>>")>]
    let ``Text.removeDotNetGenericNotation should strip the .NET generic annotation``(str, expected: string) =
        Text.removeDotNetGenericNotation str |> should equal expected

