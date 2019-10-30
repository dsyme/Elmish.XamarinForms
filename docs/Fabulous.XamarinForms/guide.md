Fabulous for Xamarin.Forms - Guide
=======

{% include_relative contents.md %}

This guide augments the primary [Xamarin.Forms documentation](https://docs.microsoft.com/xamarin/xamarin-forms/) with information
specific to the Fabulous programming model.

> NOTE: A comprehensive guide to Xamarin.Forms programming is available for free
in the [Creating Mobile Apps with Xamarin.Forms](https://docs.microsoft.com/xamarin/xamarin-forms/creating-mobile-apps-xamarin-forms/) ebook.
The book deals with C# MVVM programming but many details are relevant here.


Structure of an App
------

Here is the typical structure for the main logic of an app:
```fsharp
type Msg =
    | ...

/// The model from which the view is generated
type Model = 
    { ... }

/// Returns the initial state
let init() = { ... }
    
/// The funtion to update the view
let update (msg:Msg) (model:Model) = ...

/// The view function giving updated content for the page
let view (model: Model) dispatch = ...

type App () as app = 
    inherit Application ()

    let runner = 
        Program.mkSimple init update view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app
```



### The model

The model is the core data from which the whole state of the app can be resurrected.  When designing your model,
ask yourself  "what is the information I would need on restart to get the app back to the same essential state". The model is generally immutable but may also contain elements such as service connections.
It is common for the desgin of the model to grow "organically" as you prototype your app.

Some advantages of using an immutable model are:

* It is easy to unit test your `init`, `update` and `view` functions
* You can save/restore your model relatively easily
* It makes tracing causality usually very simple

The init function returns your initial model.  The update function updates the model as messages are received.

### The view function

The view function computes an immutable Xaml-like description.

### The update function

Each model gets an `update` function for message processing. The messages are either messages from the `view` or from external events.
If using `Program.mkProgram` your `update` function may also return new commands to trigger as a result of processing a message. (A command is simply a function that may dispatch one or more messages at some point, and is called by the Fabulous runtime.)

Structure of a Project
------

The majority of your app logic will be in your shared code project, normally a .NET Standard 2.0 project.

Your project will also have `iOS` and `Droid` projects for actually running the core logic on these different platforms.  

Running 
------

To run, set your target to `Any CPU` (Android) or `iPhone` or `iPhone Simulator`, then choose your device and launch.

You may need to install Android, iOS and/or other SDK tooling.  

If running on-device you may need to enable developer settings for your device,
or, in the case of iOS, enable [free provisioning](https://docs.microsoft.com/xamarin/ios/get-started/installation/device-provisioning/free-provisioning).

