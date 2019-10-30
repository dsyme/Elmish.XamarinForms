// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabimals.Components

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Fabimals.Models

module Templates =
    let animalTemplate animal =
        dependsOn animal (fun _ animal ->
            View.Grid(
                tag=animal,
                padding=Thickness 10.,
                coldefs=[ Auto; Auto],
                rowdefs=[ Auto; Auto ],
                children=[
                    View.Image(
                        source=Image.Path animal.ImageUrl,
                        aspect=Aspect.AspectFill,
                        height=40.,
                        width=40.
                    ).RowSpan(2)
                    View.Label(
                        text=animal.Name,
                        fontAttributes=FontAttributes.Bold
                    ).Column(1)
                    View.Label(
                        text=animal.Location,
                        fontAttributes=FontAttributes.Italic,
                        verticalOptions=LayoutOptions.End
                    ).Row(1).Column(1)
                ]
            )
        )