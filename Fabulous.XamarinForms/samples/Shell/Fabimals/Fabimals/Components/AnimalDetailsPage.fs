// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabimals.Components

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Fabimals.Models

module AnimalDetails =
    type Msg = NoOp
    type CmdMsg = NoOp
    type ExternalMsg = NoOp
    
    type Model =
        { Animal: Animal }

    let init (animal: Animal) =
        { Animal = animal }
    
    let view model =
        dependsOn model.Animal (fun model animal ->
            // Currently Fabulous needs to handle its own ContentPage to support Routing inside Shell
            // This is a limitation of Xamarin.Forms. Might change in the future (https://github.com/xamarin/Xamarin.Forms/issues/5166)
            // So for now, we only declare the content of the page

            View.ScrollView(
                View.StackLayout(
                    margin=Thickness 20.,
                    children=[
                        View.Label(
                            text=animal.Name,
                            horizontalOptions=LayoutOptions.Center,
                            style=Device.Styles.TitleStyle
                        )
                        View.Label(
                            text=animal.Location,
                            fontAttributes=FontAttributes.Italic,
                            horizontalOptions=LayoutOptions.Center
                        )
                        View.Image(
                            source=Image.Path animal.ImageUrl,
                            width=200.,
                            height=200.,
                            horizontalOptions=LayoutOptions.CenterAndExpand
                        )
                        View.Label(
                            text=animal.Details,
                            style=Device.Styles.BodyStyle
                        )
                    ]
                )
            )
        )