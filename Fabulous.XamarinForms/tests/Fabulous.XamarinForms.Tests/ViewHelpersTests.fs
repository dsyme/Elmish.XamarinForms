// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.Core.Tests

open System
open NUnit.Framework
open FsUnit
open FSharp.Data.Adaptive
open Fabulous.XamarinForms

module ViewHelpersTests =

    [<Test>]
    let ``Given no element with an automation id, tryFindViewElement should return no element``() =
        View.NavigationPage(pages = cs [
            View.ContentPage(content=
                View.Grid(children = cs [
                    View.Label(text = c "Text")
                    View.StackLayout(children = cs [
                        View.Button(text = c "Button text")
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
        View.NavigationPage(automationId = c "NavigationPage", pages = cs [
            View.ContentPage(automationId = c "ContentPageId", content=
                View.Grid(automationId = c "GridId", children = cs [
                    View.Label(automationId = c "LabelId", text = c "Text")
                    View.StackLayout(automationId = c "StackLayoutId", children = cs [
                        View.Button(automationId = c "ButtonId", text = c "Button text")
                        View.Slider(automationId = c "SliderId")
                    ])
                    View.Image(automationId = c "ImageId")
                ])
            )
        ])
        |> tryFindViewElement "AutomationIdTest"
        |> should equal None

    [<Test>]
    let ``Given an element with a matching automation id, tryFindViewElement should return the element`` () =
        let automationId = "ContentPageTest"
        let element = View.ContentPage(automationId = c automationId)

        element
        |> tryFindViewElement automationId
        |> should equal (Some element)

    [<Test>]
    let ``Given an element with a matching automation id inside a NavigationPage, tryFindViewElement should return the element`` () =
        let automationId = "ContentPageTest"
        let element = View.ContentPage(automationId = c automationId)

        View.NavigationPage(pages = cs [
            View.ContentPage(automationId = c "WrongPage1")
            element
            View.ContentPage(automationId = c "WrongPage2")
        ])
        |> tryFindViewElement automationId
        |> should equal (Some element)

    [<Test>]
    let ``Given an element with a matching automation id inside a ContentControl, tryFindViewElement should return the element`` () =
        let automationId = "LabelTest"
        let element = View.Label(automationId = c automationId)

        View.ContentPage(content=
            element
        )
        |> tryFindViewElement automationId
        |> should equal (Some element)

    [<Test>]
    let ``Given an element with a matching automation id inside a layout, tryFindViewElement should return the element`` () =
        let automationId = "LabelTest"
        let element = View.Label(automationId = c automationId)

        View.Grid(children = cs [
            View.Label(automationId = c "WrongLabel1")
            element
            View.Label(automationId = c "WrongLabel1")
        ])
        |> tryFindViewElement automationId
        |> should equal (Some element)

    [<Test>]
    let ``Given a complex view hierarchy, tryFindViewElement should return the first matching element`` () =
        let automationId = "ButtonId"
        let button = View.Button(automationId = c automationId, text = c "Button text")

        View.NavigationPage(automationId = c "NavigationPage", pages = cs [
            View.ContentPage(automationId = c "ContentPage1Id", content=
                View.Grid(automationId = c "Grid1Id", children = cs [
                    View.Label(automationId = c "Label1Id", text = c "Text")
                    View.StackLayout(automationId = c "StackLayout1Id", children = cs [
                        button
                        View.Slider(automationId = c "Slider1Id")
                    ])
                    View.Image(automationId = c "Image1Id")
                ])
            )
            View.ContentPage(automationId = c "ContentPage2Id", content=
                View.Grid(automationId = c "Grid2Id", children = cs [
                    View.Label(automationId = c "Label2Id", text = c "Text")
                    View.StackLayout(automationId = c "StackLayout2Id", children = cs [
                        View.Button(automationId = c automationId, text = c "Button text")
                        View.Slider(automationId = c "Slider2Id")
                    ])
                    View.Image(automationId = c "Image2Id")
                ])
            )
        ])
        |> tryFindViewElement automationId
        |> should equal (Some button)

    [<Test>]
    let ``Given an element with a matching automation id, findViewElement should return the element`` () =
        let automationId = "LabelTest"
        let element = View.Label(automationId = c automationId)

        View.ContentPage(content=
            element
        )
        |> findViewElement automationId
        |> should equal element

    [<Test>]
    let ``Given no element with a matching automation id, findViewElement should throw an exception`` () =
        (fun() ->
            View.ContentPage(content=
                View.Label(automationId = c "LabelId")
            )
            |> findViewElement "LabelTest"
            |> ignore)
        |> should throw typeof<System.Exception>
