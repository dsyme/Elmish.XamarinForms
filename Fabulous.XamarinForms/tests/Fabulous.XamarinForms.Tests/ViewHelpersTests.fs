// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.Core.Tests

open System
open NUnit.Framework
open FsUnit
open Fabulous.XamarinForms

module ViewHelpersTests =

    [<Test>]
    let ``Given no element with an automation id, tryFindViewElement should return no element``() =
        View.NavigationPage(pages=[
            View.ContentPage(content=
                View.Grid(children=[
                    View.Label(text="Text")
                    View.StackLayout(children=[
                        View.Button(text="Button text")
                        View.Slider()
                    ])
                    View.Image()
                ])
            )
        ])
        |> tryFindViewElement "AutomationIdTest"
        |> should equal None

    [<Test>]
    let ``Given no element with a matching automation id, tryFindViewElement should return no element``() =
        View.NavigationPage(automationId="NavigationPage", pages=[
            View.ContentPage(automationId="ContentPageId", content=
                View.Grid(automationId="GridId", children=[
                    View.Label(automationId="LabelId", text="Text")
                    View.StackLayout(automationId="StackLayoutId", children=[
                        View.Button(automationId="ButtonId", text="Button text")
                        View.Slider(automationId="SliderId")
                    ])
                    View.Image(automationId="ImageId")
                ])
            )
        ])
        |> tryFindViewElement "AutomationIdTest"
        |> should equal None

    [<Test>]
    let ``Given an element with a matching automation id, tryFindViewElement should return the element`` () =
        let automationId = "ContentPageTest"
        let element = View.ContentPage(automationId=automationId)

        element
        |> tryFindViewElement automationId
        |> should equal (Some element)

    [<Test>]
    let ``Given an element with a matching automation id inside a NavigationPage, tryFindViewElement should return the element`` () =
        let automationId = "ContentPageTest"
        let element = View.ContentPage(automationId=automationId)

        View.NavigationPage(pages=[
            View.ContentPage(automationId="WrongPage1")
            element
            View.ContentPage(automationId="WrongPage2")
        ])
        |> tryFindViewElement automationId
        |> should equal (Some element)

    [<Test>]
    let ``Given an element with a matching automation id inside a ContentControl, tryFindViewElement should return the element`` () =
        let automationId = "LabelTest"
        let element = View.Label(automationId=automationId)

        View.ContentPage(content=
            element
        )
        |> tryFindViewElement automationId
        |> should equal (Some element)

    [<Test>]
    let ``Given an element with a matching automation id inside a layout, tryFindViewElement should return the element`` () =
        let automationId = "LabelTest"
        let element = View.Label(automationId=automationId)

        View.Grid(children=[
            View.Label(automationId="WrongLabel1")
            element
            View.Label(automationId="WrongLabel1")
        ])
        |> tryFindViewElement automationId
        |> should equal (Some element)

    [<Test>]
    let ``Given a complex view hierarchy, tryFindViewElement should return the first matching element`` () =
        let automationId = "ButtonId"
        let button = View.Button(automationId=automationId, text="Button text")

        View.NavigationPage(automationId="NavigationPage", pages=[
            View.ContentPage(automationId="ContentPage1Id", content=
                View.Grid(automationId="Grid1Id", children=[
                    View.Label(automationId="Label1Id", text="Text")
                    View.StackLayout(automationId="StackLayout1Id", children=[
                        button
                        View.Slider(automationId="Slider1Id")
                    ])
                    View.Image(automationId="Image1Id")
                ])
            )
            View.ContentPage(automationId="ContentPage2Id", content=
                View.Grid(automationId="Grid2Id", children=[
                    View.Label(automationId="Label2Id", text="Text")
                    View.StackLayout(automationId="StackLayout2Id", children=[
                        View.Button(automationId=automationId, text="Button text")
                        View.Slider(automationId="Slider2Id")
                    ])
                    View.Image(automationId="Image2Id")
                ])
            )
        ])
        |> tryFindViewElement automationId
        |> should equal (Some button)

    [<Test>]
    let ``Given an element with a matching automation id, findViewElement should return the element`` () =
        let automationId = "LabelTest"
        let element = View.Label(automationId=automationId)

        View.ContentPage(content=
            element
        )
        |> findViewElement automationId
        |> should equal element

    [<Test>]
    let ``Given no element with a matching automation id, findViewElement should throw an exception`` () =
        (fun() ->
            View.ContentPage(content=
                View.Label(automationId="LabelId")
            )
            |> findViewElement "LabelTest"
            |> ignore)
        |> should throw typeof<System.Exception>
