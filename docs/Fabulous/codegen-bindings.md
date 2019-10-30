Fabulous - Guide
=======

CodeGen - Bindings file format
------

### Root

```json
{
    "assemblies": [
        "path/to/A.dll",
        "path/to/B.dll"
    ],
    "outputNamespace": "My.Namespace",
    "types": [
        // See Type
    ]
}
```

| Field      | Type      | Required | Description |
| ---------- | --------- | ----------- | -- |
| Assemblies | string array | ✅ Yes | Paths to dlls containing controls (can be relative to working directory) |
| OutputNamespace | string | ✅ Yes | Namespace under which all the generated code will be added |
| Types | Type array | ✅ Yes | All types to bind |

### Type

```json
{
    "type": "Xamarin.Forms.ListView",
    "customType": "Fabulous.XamarinForms.CustomListView",
    "canBeInstantiated": true,
    "name": "ListView",
    "properties": [
        // See Property
    ],
    "events": [
        // See Event
    ],
    "attachedProperties": [
        // See Attached Property
    ]
}
```

| Field      | Type      | Required | Description |
| ---------- | --------- | ----------- | -- |
| Type | string | ✅ Yes | Full name of a control inside one of the dlls to bind |
| CustomType | string | ❌ No | If specified, this type will be used when instantiating this control instead of the binded type. Not necessary that the control exists at generation time |
| CanBeInstantiated | boolean | ❌ No | Indicates if the code generator should provide a public constructor for this type. If not specified, the value defaults to `true` |
| Name | boolean | ❌ No | The name that will be used when generating code (e.g. `View.MyCustomControl()`). If not specified, the name will be inferred from the `Type` field |
| Properties | Property array | ❌ No | All properties of this control to include in the code generation |
| Events | Event array | ❌ No | All events of this control to include in the code generation |
| AttachedProperties | AttachedProperty array | ❌ No | All attached properties of this control to include in the code generation |

### Property

```json
{
    "source": "Source",
    "name": "Name",
    "uniqueName": "UniqueName",
    "shortName": "shortName",
    "defaultValue": "DefaultValue",
    "elementType": "ElementType",
    "inputType": "InputType",
    "modelType": "ModelType",
    "convertInputToModel": "(fun input: InputType -> input :?> ModelType)",
    "convertModelToValue": "(fun model: ModelType-> model :?> ValueType)",
    "updateCode": "(fun prev curr target -> ())"
}
```

| Field      | Type      | Required | Description | Remarks |
| ---------- | --------- | ----------- | -- | -- |
| Source | string | ❓Depends | Name of the property to include (and overwrite) | |
| Name | string | ❓Depends | Name of the property used in the generated code (e.g. `buttonViewElement.MyProperty(value)`) | If not specified, `Source` will be used instead |
| UniqueName | string | ❌ No | Name to uniquely identify the property in the generated code | Use with caution. If not specified, `Type` and `Name` will be concatenated to create a unique name. (e.g. `ButtonText`). Subject to optimization |
| ShortName | string | ❌ No | Name (lower camel case) to use in ViewElement constructor (e.g. `View.Button(myProperty=value)`) | If not specified, `Name` will be used in lower camel case |
| DefaultValue | string | ❓Depends | Default value to apply to the property if none is given by the user (e.g. `0.0f`) | |
| ElementType | string | ❓Depends | (Apply only if property is a collection) Full name of the item type of the collection | If `null`, the property is not considered to be a collection |
| InputType | string | ❓Depends | Type expected in the constructor (e.g. `string` => `View.Button(text = "some string")`) | |
| ModelType | string | ❌ No | Type as which the value will be stored in the ViewElement attributes dictionary | Make sure this type is efficient |
| ConvertInputToModel | string | ❌ No | Function to convert the input value to the model type | Expects a signature of type `'Input -> 'Model`. Function can be written directly inside the json, or make a reference to a function. If not specified, no conversion will be applied. |
| ConvertModelToValue | string | ❌ No | Function to convert the model value to the expected type of the real property | Expects a signature of type `'Model -> 'Value`. Function can be written directly inside the json, or make a reference to a function. If not specified, no conversion will be applied. |
| UpdateCode | string | ❌ No | Function to use instead of the generated view diffing for this property | Expects a signature of type `prev: ViewElement -> curr: ViewElement -> target: ControlType -> unit`. Function can be written directly inside the json, or make a reference to a function. If not specified, the default view diffing code will be used. |

There is 3 different categories of property:
- Existing scalar property: A property present in one of the dlls to include with a simple data type
- Existing collection property: A property present in one of the dlls to include which is a collection (e.g. `Xamarin.Forms.View list`)
- Dummy property: A non-existent property to include which can be either of type scalar or collection

See examples for these categories below.

### Event

```json
{
    "source": "TextChanged",
    "name": "TextChanged",
    "uniqueName": "EntryTextChanged",
    "shortName": "textChanged",
    "inputType": "Xamarin.Forms.TextChangedEventArgs -> unit",
    "modelType": "System.EventHandler<Xamarin.Forms.TextChangedEventArgs>",
    "convertInputToModel": "(fun f -> System.EventHandler<Xamarin.Forms.TextChangedEventArgs>(fun _sender _args -> f args))"
}
```

| Field      | Type      | Required | Description | Remarks |
| ---------- | --------- | ----------- | -- | -- |
| Source | string | ❓Depends | Name of the event to include (and overwrite) | |
| Name | string | ❓Depends | Name of the event used in the generated code (e.g. `entryViewElement.TextChanged(value)`) | If not specified, `Source` will be used instead |
| UniqueName | string | ❌ No | Name to uniquely identify the event in the generated code | Use with caution. If not specified, `Type` and `Name` will be concatenated to create a unique name. (e.g. `EntryTextChanged`). Subject to optimization |
| ShortName | string | ❌ No | Name (lower camel case) to use in ViewElement constructor (e.g. `View.Button(myProperty=value)`) | If not specified, `Name` will be used in lower camel case |
| InputType | string | ❓Depends | Type expected in the constructor (e.g. `Xamarin.Forms.TextChangedEventArgs -> unit` => `View.Button(textChanged = (fun args -> ()))`) | Expects a function |
| ModelType | string | ❓Depends | Type as which the value will be stored in the ViewElement attributes dictionary | Expects an EventHandler |
| ConvertInputToModel | string | ❓Depends | Function to convert the input value to the model type | Expects a signature of type `'Input -> 'Model`. Function can be written directly inside the json, or make a reference to a function. If not specified, no conversion will be applied. |

### AttachedProperty

```json
{
    "source": "Source",
    "name": "Name",
    "uniqueName": "UniqueName",
    "defaultValue": "DefaultValue",
    "elementType": "ElementType",
    "inputType": "InputType",
    "modelType": "ModelType",
    "convertInputToModel": "(fun input -> input :?> Model)",
    "convertModelToValue": "(fun model -> model :?> Value)",
    "updateCode": "(fun prev curr target -> ())"
}
```

| Field      | Type      | Required | Description | Remarks |
| ---------- | --------- | ----------- | -- | -- |
| Source | string | ❓Depends | Name of the attached property to include (and overwrite) | |
| TargetType | string | ❌ No | Type to which this attached property will be applied to | If not specified, a default base target type will be applied |
| Name | string | ❓Depends | Name of the property used in the generated code (e.g. `buttonViewElement.MyProperty(value)`) | If not specified, `Source` will be used instead |
| UniqueName | string | ❌ No | Name to uniquely identify the property in the generated code | Use with caution. If not specified, `Type` and `Name` will be concatenated to create a unique name. (e.g. `ButtonText`). Subject to optimization |
| DefaultValue | string | ❓Depends | Default value to apply to the property if none is given by the user (e.g. `0.0f`) | If not specified, `Name` will be used in lower camel case |
| ElementType | string | ❓Depends | (Apply only if attached property is a collection) Full name of the item type of the collection | If `null`, the attached property is not considered to be a collection |
| InputType | string | ❓Depends | Type expected in the constructor (e.g. `string` => `View.Button(text = "some string")`) | |
| ModelType | string | ❌ No | Type as which the value will be stored in the ViewElement attributes dictionary | Make sure this type is efficient |
| ConvertInputToModel | string | ❌ No | Function to convert the input value to the model type | Expects a signature of type `'Input -> 'Model`. Function can be written directly inside the json, or make a reference to a function. If not specified, no conversion will be applied. |
| ConvertModelToValue | string | ❌ No | Function to convert the model value to the expected type of the real property | Expects a signature of type `'Model -> 'Value`. Function can be written directly inside the json, or make a reference to a function. If not specified, no conversion will be applied. |
| UpdateCode | string | ❌ No | Function to use instead of the generated view diffing for this property | Expects a signature of type `prev: ViewElement -> curr: ViewElement -> target: ControlType -> unit`. Function can be written directly inside the json, or make a reference to a function. If not specified, the default view diffing code will be used. |

## Examples

### Example properties

#### Existing scalar property

```json
{
    "source": "Margin",
    "name": "Margin",
    "uniqueName": "VisualElementMargin",
    "shortName": "margin",
    "inputType": "InputTypes.Thickness",
    "modelType": "Thickness",
    "convertInputToModel": "ViewConverters.convertThickness",
    "convertModelToValue": null
}
```

| Field               | Type   | Required |
| ------------------- | ------ | -------- |
| Source              | string | ✅ Yes   |
| Name                | string | ❌ No    |
| UniqueName          | string | ❌ No    |
| ShortName           | string | ❌ No    |
| InputType           | string | ❌ No    |
| ModelType           | string | ❌ No    |
| ConvertInputToModel | string | ❌ No    |
| ConvertModelToValue | string | ❌ No    |
| UpdateCode          | string | ❌ No    |

#### Existing collection property

```json
{
    "source": "ItemsSource",
    "name": "Items",
    "uniqueName": "GridItems",
    "shortName": "items",
    "elementType": "Xamarin.Forms.View",
    "inputType": "ViewElement list",
    "modelType": "ViewElement array",
    "convertInputToModel": "Array.ofList"
}
```

| Field               | Type   | Required  | Remarks |
| ------------------- | ------ | --------- | ------- |
| Source              | string | ✅ Yes    |         |
| Name                | string | ❌ No     |         |
| UniqueName          | string | ❌ No     |         |
| ShortName           | string | ❌ No     |         |
| ElementType         | string | ❓Depends | Usually automatically detected by the AssemblyReader step. If correct, no need to specify it. |
| InputType           | string | ❌ No     |         |
| ModelType           | string | ❌ No     |         |
| ConvertInputToModel | string | ❌ No     |         |
| ConvertModelToValue | string | ❌ No     |         |
| UpdateCode          | string | ❌ No     |         |

#### Dummy property

```json
{
    "source": null,
    "name": "Name",
    "inputType": "InputType"
}
```

| Field               | Type   | Required  | Remarks |
| ------------------- | ------ | --------- | ------- |
| Source              | string | ❌ No     | Must be `null` or not declared |
| Name                | string | ✅ Yes    |         |
| UniqueName          | string | ❌ No     |         |
| ShortName           | string | ❌ No     |         |
| ElementType         | string | ❓Depends | Must be declared if property is a collection |
| InputType           | string | ✅ Yes    |         |
| ModelType           | string | ❌ No     |         |
| ConvertInputToModel | string | ❌ No     |         |
| ConvertModelToValue | string | ❌ No     |         |
| UpdateCode          | string | ❌ No     |         |