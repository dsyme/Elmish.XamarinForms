Fabulous (Adaptive version)
=======

*F# Functional App Development, using declarative dynamic UI and adaptive data*

Extends the [Fabulous](https://fsprojects.github.io/Fabulous/) programming model to use [adaptive data](https://fsprojects.github.io/FSharp.Data.Adaptive/) for high-performance updates in data-rich UIs.

Experimental, wet-paint, no nuget packages or templates yet.

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

// Make an adaptive vesion of the model. This is boilerplate. See Adaptify.
type AdaptiveModel = { Text: aval<string> }

// Initialize an adaptive vesion of the model. This is boilerplate. See Adaptify.
let ainit (model: Model) = { Text = cval model.Text }

// Update an adaptive vesion of the model. This is boilerplate. See Adaptify.
let adelta (model: Model) (amodel: AdaptiveModel) =
    transact (fun () -> 
        if model.Text <> amodel.Text.Value then 
            amodel.Text.Set model.Text
    )

// Write the view with resept to the adaptive vesion of the model.
let view (amodel: AdaptiveModel) dispatch =
    View.ContentPage(
        View.StackLayout(
            children = cs [
                View.Image(source = c "fabulous.png")
                View.Label(text = model.Text, fontSize = c 22.0)
                View.Button(text = c "Click me", command = c (fun () -> dispatch ButtonClicked))
            ]
        )
    )
```
