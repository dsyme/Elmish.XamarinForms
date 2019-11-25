Fabulous for Xamarin.Forms - Guide
=======

{% include_relative contents.md %}

Migrating from Fabulous v0.36 to Fabulous.XamarinForms v0.40
--------

With version 0.40.0, Fabulous has gone under a complete internal restructuring to enable future works (see [pull request #481](https://github.com/fsprojects/Fabulous/pull/481) for more information).

It has been splitted in 3 parts :
- Fabulous
- Fabulous.XamarinForms
- Fabulous.StaticView

Fabulous is now a framework enabling library authors to build their own declarative UI frameworks on top of Fabulous.  
It's not intended to be used directly to create applications.

Current users of Fabulous should now use Fabulous.XamarinForms instead.

Users writing apps with the StaticView constructs should target Fabulous.StaticView instead. Only the NuGet package has changed, code is still the same.  
This migration guide does not address StaticView.

An effort was made to minimize breaking changes to help users move from Fabulous v0.36 to Fabulous.XamarinForms.

### Updating the NuGet packages

To reflect this restructuring, the NuGet packages has been changed.

Here is the correspondance with the new ones:
- Fabulous.Core => Fabulous.XamarinForms
- Fabulous.LiveUpdate => Fabulous.XamarinForms.LiveUpdate

If you had a reference to the NuGet package `Fabulous.CustomControls`, you can remove it.

Extensions follow the same pattern:
- Fabulous.Maps => Fabulous.XamarinForms.Maps
- Fabulous.OxyPlot => Fabulous.XamarinForms.OxyPlot
- Fabulous.SkiaSharp => Fabulous.XamarinForms.SkiaSharp
- Fabulous.VideoManager => Fabulous.XamarinForms.VideoManager

### Changing the namespaces

One of the breaking changes is namespaces. They have been changed to match their NuGet packages.

Here is the correspondance:
- `open Fabulous.Core` => `open Fabulous`
- `open Fabulous.DynamicView` => `open Fabulous.XamarinForms`
- `open Fabulous.LiveUpdate` => `open Fabulous.XamarinForms.LiveUpdate`

### Changing the runner

Last thing to change is the line:
```
Program.runWithDynamicView
```

It now becomes
```
XamarinFormsProgram.run
```

E.g.
```
let runner =
    Program.mkProgram init update view
    |> Program.withConsoleTrace
    |> XamarinFormsProgram.run app
```

Your apps should now correctly build and run.

### Using the new templates

Templates have also been changed to reflect this new structure.

The quickest way to replace the old templates with the new ones is to execute the following commands:
```
dotnet new -u Fabulous.Templates
dotnet new -i Fabulous.XamarinForms.Templates
```

Then, you can create your app with the classic command (note: `fabulous-app` has been changed to `fabulous-xf-app`)
```
dotnet new fabulous-xf-app --macOS --GTK --WPF --UWP -n SqueakyApp
```
