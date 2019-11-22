Fabulous (Adaptive version)
=======

*F# Functional App Development, using declarative dynamic UI and adaptive data*

Extends the [Fabulous](https://fsprojects.github.io/Fabulous/) programming model to use [adaptive data](https://fsprojects.github.io/FSharp.Data.Adaptive/) for high-performance updates in data-rich UIs.

Experimental, wet-paint, no nuget packages or templates yet. Originated from [this discussion](https://github.com/fsprojects/Fabulous/issues/258).
See also corresponding work on [Fable.Elmish.Adaptive](https://github.com/krauthaufen/Fable.Elmish.Adaptive/).

With Fabulous Adaptive, you can write code like this:
```fsharp
type Model = { Text: string }

type Msg =
    | ButtonClicked

let init () =
    { Text = "Hello Fabulous!" }

let update msg model =
    match msg with
    | ButtonClicked -> { model with Text = "Thanks for using Fabulous!" }

// Write the view with resept to the adaptive vesion of the model.
let view (amodel: AdaptiveModel) dispatch =
    View.ContentPage(
        View.StackLayout(
            children = cs [
                View.Image(source = c (Path "fabulous.png"))
                View.Label(text = model.Text, fontSize = c (FontSize 22.0))
                View.Button(text = c "Click me", command = c (fun () -> dispatch ButtonClicked))
            ]
        )
    )
```
You must also add the following boiler-plate code - see also the 'Adaptify' tool.
```fsharp
/// An adaptive vesion of the model. 
type AdaptiveModel = { Text: cval<string> }

/// Initialize an adaptive vesion of the model.
let ainit (model: Model) = { Text = cval model.Text }

/// Update an adaptive vesion of the model. 
let adelta (model: Model) (amodel: AdaptiveModel) =
    transact (fun () -> 
        if model.Text <> amodel.Text.Value then 
            amodel.Text.Value <- model.Text
    )
```
