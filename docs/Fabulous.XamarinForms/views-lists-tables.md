Fabulous for Xamarin.Forms - Guide
=======

{% include_relative contents-views.md %}

Lists and Tables
-------------------

### ListView, ListGroupedView

A simple `ListView` is as follows:

```fsharp
View.ListView(
    items = [
        View.TextCell "Ionide"
        View.TextCell "Visual Studio"
        View.TextCell "Emacs"
        View.TextCell "Visual Studio Code"
        View.TextCell "JetBrains Rider"
    ],
    itemSelected = (fun idx -> dispatch (ListViewSelectedItemChanged idx))
)
```

The `itemSelected` callback uses integers indexes for keys to identify the elements.

There is also a `ListViewGrouped` for grouped items of data.  This uses the same Xamarin control under the hood but in a different mode of use.

<img src="https://user-images.githubusercontent.com/52166903/60180201-5dfc5b00-9817-11e9-9508-a0daa7b7a81d.png" width="400">

See also:

* [`Xamarin.Forms.Core.ListView`](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/listview/)
* [Xamarin.Forms Cells](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/controls/cells)

### TableView

An example `TableView` is as follows:

```fsharp
View.TableView(
    items = [
        (View.TextCell("Videos"), [
            View.SwitchCell(
                on = true,
                text = "Luca 2008",
                onChanged = (fun args -> ())
            )
            View.SwitchCell(
                on = true,
                text = "Don 2010",
                onChanged = (fun args -> ())
            )
        ])
        (View.TextCell("Books"), [
            View.SwitchCell(
                on = true,
                text = "Expert F#",
                onChanged = (fun args -> ())
            )
            View.SwitchCell(
                on = false,
                text = "Programming F#",
                onChanged = (fun args -> ())
            )
        ])
        (View.TextCell("Contact"), [
            View.EntryCell(
                label = "Email",
                placeholder = "foo@bar.com",
                completed = (fun args -> ())
            )
            View.EntryCell(
                label = "Phone",
                text = "+44 87654321",
                completed = (fun args -> ())
            )
        ])
    ]
)
```

<img src="https://user-images.githubusercontent.com/52166903/60177365-9d737900-9810-11e9-92d5-88487316bbf6.png" width="400">

See also:

* [`Xamarin.Forms.Core.TableView`](https://docs.microsoft.com/en-us/dotnet/api/Xamarin.Forms.TableView)

#### "Infinite" or "unbounded" ListViews

"Infinite" (really "unbounded") lists are created by using the `itemAppearing` event to prompt a message which nudges the
underlying model in a direction that will then supply new items to the view.

For example, consider this pattern:

```fsharp
type Model =
    { ...
      LatestItemAvailable: int
    }

type Message =
    ...
    | GetMoreItems of int

let update msg model =
    match msg with
    | ...
    | GetMoreItems n -> { model with LatestItemAvailable = n }

let view model dispatch =
    ...
    View.ListView(
        items = [
            for i in 1 .. model.LatestItemAvailable do
                yield View.TextCell("Item " + string i)
        ],
        itemAppearing = (fun idx -> if idx >= max - 2 then dispatch (GetMoreItems (idx + 10) ) )
    )
...
```

Note:

* The underlying data in the model is just an integer `LatestItemAvailable` (normally it would really be a list of actual entities drawn from a data source)
* On each update to the view we produce all the visual items from `Item 1` onwards
* The `itemAppearing` event is called for each item, e.g. when item `10` appears
* When the event triggers we grow the underlying data model by 10
* This will trigger an update of the view again, with more visual elements available (but not yet appearing)

Surprisingly even this naive technique  is fairly efficient. There are numerous ways to make this more efficient (we aim to document more of these over time too).  One simple one is to memoize each individual visual item using `dependsOn`:

```fsharp
items = [
    for i in 1 .. model.LatestItemAvailable do
        yield dependsOn i (fun model i -> View.Label("Item " + string i))
]
```

With that, this simple list views scale to > 10,000 items on a modern phone, though your mileage may vary.
There are many other techniques (e.g. save the latest collection of visual element descriptions in the model, or to use a `ConditionalWeakTable` to associate it with the latest model).  We will document further techniques in due course.

Thre is also an `itemDisappearing` event for `ListView` that can be used to discard data from the underlying model and restrict the
range of visual items that need to be generated.

### CollectionView
 
Please read the Xamarin.Forms documentation to check whether this control is available for the platforms you target.

Usage:
```fsharp
View.CollectionView(items = [
    View.Label(text = "Person1") 
    View.Label(text = "Person2")
    View.Label(text = "Person3")
    View.Label(text = "Person4")
    View.Label(text = "Person5")
])
```

<img src="https://user-images.githubusercontent.com/52166903/60262083-4683a780-98d5-11e9-8afc-cde4d594171b.png" width="400">

See also:
* [Xamarin guide to CollectionView](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/collectionview/)
