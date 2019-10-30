// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.Generator

module Helpers =
    type NullableBuilder() =
        member this.Return(x) = x
        member this.Bind(x, f) =
            match x with
            | null -> None
            | _ -> f x
       
    let nullable = NullableBuilder()
