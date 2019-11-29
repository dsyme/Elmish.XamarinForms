Fabulous for Xamarin.Forms
=======

*Write cross-platform apps with Xamarin.Forms, using MVU architecture and dynamic UI*

[![Fabulous.XamarinForms NuGet version](https://badge.fury.io/nu/Fabulous.XamarinForms.svg)](https://badge.fury.io/nu/Fabulous.XamarinForms) [![Build Status](https://dev.azure.com/timothelariviere/Fabulous/_apis/build/status/Full%20Build?branchName=master)](https://dev.azure.com/timothelariviere/Fabulous/_build/latest?definitionId=7&branchName=master) [![Join the chat at https://gitter.im/fsprojects/Fabulous](https://badges.gitter.im/fsprojects/Fabulous.svg)](https://gitter.im/fsprojects/Fabulous?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This library allows you to use the ultra-simple Model-View-Update architecture to build applications for iOS, Android, Mac, WPF and more using Xamarin.Forms. It is built on Fabulous.

> The amount of code I'm *not* writing is great!  [@jimbobbennett](https://github.com/jimbobbennett/)

With Fabulous for Xamarin.Forms, you will be able to write complete applications fully in F#, like this:
```fsharp
type Model = { Text: string }
type Msg = ButtonClicked

let init () = { Text = "Hello Fabulous!" }

let update msg model =
    match msg with
    | ButtonClicked -> { model with Text = "Thanks for using Fabulous!" }

let view model dispatch =
    View.ContentPage(
        View.StackLayout(
            children=[
                View.Image(source = "fabulous.png")
                View.Label(text = model.Text, fontSize = 22.)
                View.Button(text = "Click me", command = (fun () -> dispatch ButtonClicked))
            ]
        )
    )
```
For more information, please take a look at the documentation.

* [Getting started](https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/index.html#getting=started)

* [Documentation Guide](https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/guide.html)

* [Release Notes](../RELEASE_NOTES.md)

## Roadmap

* Live Update
  * Use actual newly compiled DLLs on Android instead of F# interperter
  * Check Live Update on WPF and other same-machine

* App size
  * Remove F# resources in linker, see https://github.com/fsprojects/Fabulous/issues/94

## Contributing

Please contribute to this library through issue reports, pull requests, code reviews and discussion.
