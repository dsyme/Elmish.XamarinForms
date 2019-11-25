// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

[<AutoOpen>]
module InputTypes =
    /// Represents an image source
    type Image =
        /// A path to the image
        | Path of string
        /// A byte array representing the image
        | Bytes of byte[]
        /// An already defined ImageSource
        | Source of Xamarin.Forms.ImageSource
    
    /// Represents a dimension for either the row or column definition of a Grid    
    type Dimension =
        /// Use a size that fits the children of the row or column.
        | Auto
        /// Use a proportional size of 1
        | Star
        /// Use a proportional size defined by the associated value
        | Stars of float
        /// Use the associated value as the number of device-specific units.
        | Absolute of float
        
    /// Represents a font size
    type FontSize =
        /// Use a named size
        | Named of Xamarin.Forms.NamedSize
        /// Use a value as the size
        | FontSize of float

    /// Defines if the action should be animated or not
    type AnimationKind =
        | Animated
        | NotAnimated

    /// Defines a scroll request to an item
    type ScrollToItem =
        { /// Zero-based index of the item to scroll to 
          Index: int
          /// Position to use
          Position: Xamarin.Forms.ScrollToPosition
          /// Determines whether the scroll will be animated or not
          Animate: AnimationKind }
        
    /// Defines a scroll request to an item in a grouped list
    type ScrollToGroupedItem =
        { /// Zero-based index of the group containing the item to scroll to
          GroupIndex: int
          /// Zero-based index of the item to scroll to 
          ItemIndex: int
          /// Position to use
          Position: Xamarin.Forms.ScrollToPosition
          /// Determines whether the scroll will be animated or not
          Animate: AnimationKind }