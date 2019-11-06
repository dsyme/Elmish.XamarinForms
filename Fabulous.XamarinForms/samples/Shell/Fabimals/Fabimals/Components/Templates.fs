// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabimals.Components

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Fabimals.Models

module Templates =
    let animalTemplate (animal: Animal) =
            View.Grid(
                tag = c (box animal),
                padding = c (Thickness 10.),
                coldefs = c [ Auto; Auto],
                rowdefs = c [ Auto; Auto ],
                children = cs [
                    View.Image(
                        source = c (Image.Path animal.ImageUrl),
                        aspect = c Aspect.AspectFill,
                        height = c 40.,
                        width = c 40.
                    ).RowSpan(c 2)
                    View.Label(
                        text = c animal.Name,
                        fontAttributes=c FontAttributes.Bold
                    ).Column(c 1)
                    View.Label(
                        text = c animal.Location,
                        fontAttributes = c FontAttributes.Italic,
                        verticalOptions = c LayoutOptions.End
                    ).Row(c 1).Column(c 1)
                ]
            )
