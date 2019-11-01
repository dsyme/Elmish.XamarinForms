// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.Generator

open System
open Fabulous.CodeGen.AssemblyReader
open Xamarin.Forms

module XFConverters =
    let isTypeResolvable typeName =
        match typeName with
        | "Xamarin.Forms.UriImageSource" -> false
        | _ -> true
        
    let convertTypeName (typeName: string) =
        match typeName with
        | "Xamarin.Forms.Grid.IGridList<Xamarin.Forms.View>"
        | "System.Collections.Generic.IList<Xamarin.Forms.Effect>"
        | "System.Collections.Generic.IList<T>"
        | "System.Collections.Generic.IList<Xamarin.Forms.Behavior>"
        | "System.Collections.Generic.IList<Xamarin.Forms.Span>"
        | "System.Collections.Generic.IList<Xamarin.Forms.MenuItem>" -> "ViewElement alist"
        | "Xamarin.Forms.Button+ButtonContentLayout" -> "Xamarin.Forms.Button.ButtonContentLayout"
        | "System.EventHandler<Xamarin.Forms.VisualElement/FocusRequestArgs>" -> "System.EventHandler<Xamarin.Forms.VisualElement.FocusRequestArgs>"
        | "System.EventHandler<System.Tuple<System.String,System.String>>" -> "System.EventHandler<string * string>"
        | "System.Tuple<System.String,System.String>" -> "(string * string)"
        | _ -> Converters.convertTypeName typeName
        
    let rec tryGetStringRepresentationOfDefaultValue (defaultValue: obj) : string option =
        let floatToString = Converters.numberWithDecimalsToString ""
        let float32ToString = Converters.numberWithDecimalsToString "f"
        
        match defaultValue with
        | :? Color as color when color = Color.Default || color = Unchecked.defaultof<Color> -> Some "Xamarin.Forms.Color.Default"
        | :? Keyboard as keyboard when keyboard = Keyboard.Default -> Some "Xamarin.Forms.Keyboard.Default"
        | :? Font as font when font.IsDefault -> Some "Xamarin.Forms.Font.Default"
        | :? Thickness as thickness when thickness = Unchecked.defaultof<Thickness> -> Some "Xamarin.Forms.Thickness(0.)"
        | :? CornerRadius as cornerRadius when cornerRadius = Unchecked.defaultof<CornerRadius> -> Some "Xamarin.Forms.CornerRadius(0.)"
        | :? LayoutOptions as layoutOptions ->
            match layoutOptions with
            | x when x = LayoutOptions.Start -> Some "Xamarin.Forms.LayoutOptions.Start"
            | x when x = LayoutOptions.Center -> Some "Xamarin.Forms.LayoutOptions.Center"
            | x when x = LayoutOptions.End -> Some "Xamarin.Forms.LayoutOptions.End"
            | x when x = LayoutOptions.Fill -> Some "Xamarin.Forms.LayoutOptions.Fill"
            | x when x = LayoutOptions.StartAndExpand -> Some "Xamarin.Forms.LayoutOptions.StartAndExpand"
            | x when x = LayoutOptions.CenterAndExpand -> Some "Xamarin.Forms.LayoutOptions.CenterAndExpand"
            | x when x = LayoutOptions.EndAndExpand -> Some "Xamarin.Forms.LayoutOptions.EndAndExpand"
            | x when x = LayoutOptions.FillAndExpand -> Some "Xamarin.Forms.LayoutOptions.FillAndExpand"
            | x -> Some (sprintf "Xamarin.Forms.LayoutOptions(Xamarin.Forms.LayoutAlignment.%s, %s)" (Enum.GetName(typeof<LayoutAlignment>, x.Alignment)) (if x.Expands then "true" else "false"))
        | :? Button.ButtonContentLayout as buttonContentLayout ->
            tryGetStringRepresentationOfDefaultValue buttonContentLayout.Position
            |> Option.map (fun positionName -> sprintf "Xamarin.Forms.Button.ButtonContentLayout(%s, %s)" positionName (floatToString buttonContentLayout.Spacing))
        | :? Rectangle as rectangle when rectangle = Rectangle.Zero -> Some "Xamarin.Forms.Rectangle.Zero"
        | :? Rectangle as rectangle -> Some (sprintf "Xamarin.Forms.Rectangle(%s, %s, %s, %s)" (floatToString rectangle.X) (floatToString rectangle.Y) (floatToString rectangle.Width) (floatToString rectangle.Height))
        | :? Size as size when size.IsZero -> Some "Xamarin.Forms.Size.Zero"
        | :? Size as size -> Some (sprintf "Xamarin.Forms.Size(%s, %s)" (floatToString size.Width) (floatToString size.Height))
        | :? IVisual as visual ->
            match visual.GetType().Name with
            | "MatchParentVisual" -> Some "Xamarin.Forms.VisualMarker.MatchParent"
            | "Default" -> Some "Xamarin.Forms.VisualMarker.Default"
            | "Material" -> Some "Xamarin.Forms.VisualMarker.Material"
            | _ -> None
        | :? FlexBasis as flexBasis when flexBasis = FlexBasis.Auto -> Some "Xamarin.Forms.FlexBasis.Auto"
        | :? FlexBasis as flexBasis -> Some (sprintf "Xamarin.Forms.FlexBasis(%s)" (float32ToString flexBasis.Length))
        | :? LinearItemsLayout as linearItemsLayout when linearItemsLayout = (LinearItemsLayout.Horizontal :?> LinearItemsLayout) -> Some (sprintf "Xamarin.Forms.LinearItemsLayout.Horizontal")
        | :? LinearItemsLayout as linearItemsLayout when linearItemsLayout = (LinearItemsLayout.Vertical :?> LinearItemsLayout) -> Some (sprintf "Xamarin.Forms.LinearItemsLayout.Vertical")
        | :? LinearItemsLayout as linearItemsLayout ->
            tryGetStringRepresentationOfDefaultValue linearItemsLayout.Orientation
            |> Option.map (fun orientation -> sprintf "Xamarin.Forms.ListItemsLayout(%s)" orientation)
        | _ -> Converters.tryGetStringRepresentationOfDefaultValue defaultValue

