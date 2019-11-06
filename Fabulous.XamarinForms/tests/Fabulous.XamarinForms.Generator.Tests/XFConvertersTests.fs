// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.Generator.Tests

open System
open Fabulous.XamarinForms.Generator
open FsUnit
open NUnit.Framework
open Xamarin.Forms

module XFConvertersTests =
    [<TestCase("Xamarin.Forms.UriImageSource", false)>]
    [<TestCase("Xamarin.Forms.Button", true)>]
    let ``isTypeResolvable should return false for excluded types``(typeName, expectedValue) =
        XFConverters.isTypeResolvable typeName |> should equal expectedValue

    [<TestCase("Xamarin.Forms.Grid.IGridList<Xamarin.Forms.View>", "ViewElement alist")>]
    [<TestCase("System.Collections.Generic.IList<Xamarin.Forms.Effect>", "ViewElement alist")>]
    [<TestCase("System.Collections.Generic.IList<T>", "ViewElement alist")>]
    [<TestCase("System.Collections.Generic.IList<Xamarin.Forms.Behavior>", "ViewElement alist")>]
    [<TestCase("System.Collections.Generic.IList<Xamarin.Forms.Span>", "ViewElement alist")>]
    [<TestCase("System.Collections.Generic.IList<Xamarin.Forms.MenuItem>", "ViewElement alist")>]
    [<TestCase("Xamarin.Forms.Button+ButtonContentLayout", "Xamarin.Forms.Button.ButtonContentLayout")>]
    [<TestCase("System.EventHandler<Xamarin.Forms.VisualElement/FocusRequestArgs>", "System.EventHandler<Xamarin.Forms.VisualElement.FocusRequestArgs>")>]
    [<TestCase("System.EventHandler<System.Tuple<System.String,System.String>>", "System.EventHandler<string * string>")>]
    [<TestCase("System.Tuple<System.String,System.String>", "(string * string)")>]
    let ``convertTypeName should convert known Xamarin.Forms types``(typeName, expectedValue) =
        XFConverters.convertTypeName typeName |> should equal expectedValue
        
    let testTryGetStringRepresentationOfDefaultValue value expectedValue =
        XFConverters.tryGetStringRepresentationOfDefaultValue value |> should equal (Some expectedValue)
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Color.Default``() =
        testTryGetStringRepresentationOfDefaultValue Color.Default "Xamarin.Forms.Color.Default"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Unchecked.defaultof Color``() =
        testTryGetStringRepresentationOfDefaultValue Unchecked.defaultof<Color> "Xamarin.Forms.Color.Default"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Keyboard.Default``() =
        testTryGetStringRepresentationOfDefaultValue Keyboard.Default "Xamarin.Forms.Keyboard.Default"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Font.Default``() =
        testTryGetStringRepresentationOfDefaultValue Font.Default "Xamarin.Forms.Font.Default"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Unchecked.defaultof Thickness``() =
        testTryGetStringRepresentationOfDefaultValue Unchecked.defaultof<Thickness> "Xamarin.Forms.Thickness(0.)"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Unchecked.defaultof CornerRadius``() =
        testTryGetStringRepresentationOfDefaultValue Unchecked.defaultof<CornerRadius> "Xamarin.Forms.CornerRadius(0.)"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for LayoutOptions.Start``() =
        testTryGetStringRepresentationOfDefaultValue LayoutOptions.Start "Xamarin.Forms.LayoutOptions.Start"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for LayoutOptions.Center``() =
        testTryGetStringRepresentationOfDefaultValue LayoutOptions.Center "Xamarin.Forms.LayoutOptions.Center"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for LayoutOptions.End``() =
        testTryGetStringRepresentationOfDefaultValue LayoutOptions.End "Xamarin.Forms.LayoutOptions.End"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for LayoutOptions.Fill``() =
        testTryGetStringRepresentationOfDefaultValue LayoutOptions.Fill "Xamarin.Forms.LayoutOptions.Fill"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for LayoutOptions.StartAndExpand``() =
        testTryGetStringRepresentationOfDefaultValue LayoutOptions.StartAndExpand "Xamarin.Forms.LayoutOptions.StartAndExpand"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for LayoutOptions.CenterAndExpand``() =
        testTryGetStringRepresentationOfDefaultValue LayoutOptions.CenterAndExpand "Xamarin.Forms.LayoutOptions.CenterAndExpand"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for LayoutOptions.EndAndExpand``() =
        testTryGetStringRepresentationOfDefaultValue LayoutOptions.EndAndExpand "Xamarin.Forms.LayoutOptions.EndAndExpand"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for LayoutOptions.FillAndExpand``() =
        testTryGetStringRepresentationOfDefaultValue LayoutOptions.FillAndExpand "Xamarin.Forms.LayoutOptions.FillAndExpand"
        
    [<TestCase(Button.ButtonContentLayout.ImagePosition.Bottom, 1.5, "Bottom", "1.5")>]
    [<TestCase(Button.ButtonContentLayout.ImagePosition.Left, 2.5, "Left", "2.5")>]
    [<TestCase(Button.ButtonContentLayout.ImagePosition.Right, 3.5, "Right", "3.5")>]
    [<TestCase(Button.ButtonContentLayout.ImagePosition.Top, 4.5, "Top", "4.5")>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for ButtonContentLayout``(imagePosition, spacing, expectedImagePosition, expectedSpacing) =
        let buttonContentLayout = Button.ButtonContentLayout(imagePosition, spacing)
        testTryGetStringRepresentationOfDefaultValue buttonContentLayout (sprintf "Xamarin.Forms.Button.ButtonContentLayout(Xamarin.Forms.Button.ButtonContentLayout.ImagePosition.%s, %s)" expectedImagePosition expectedSpacing)
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Rectangle.Zero``() =
        testTryGetStringRepresentationOfDefaultValue Rectangle.Zero "Xamarin.Forms.Rectangle.Zero"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Rectangle``() =
        let rectangle = Rectangle(1.5, 2.5, 3.5, 4.5)
        testTryGetStringRepresentationOfDefaultValue rectangle "Xamarin.Forms.Rectangle(1.5, 2.5, 3.5, 4.5)"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Size.Zero``() =
        testTryGetStringRepresentationOfDefaultValue Size.Zero "Xamarin.Forms.Size.Zero"
        
    [<Test>]
    let ``tryGetStringRepresentationOfDefaultValue should return the default F# representation for Size``() =
        let size = Size(1.5, 2.5)
        testTryGetStringRepresentationOfDefaultValue size "Xamarin.Forms.Size(1.5, 2.5)"