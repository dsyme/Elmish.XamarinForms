// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous 

open FSharp.Data.Adaptive

[<AutoOpen>]
module SimplerHelpers = 
    let c x = AVal.constant x
    let cs xs = AList.ofList xs
