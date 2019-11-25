namespace AllControls.Effects

open Xamarin.Forms

type ShadowEffect() =
    inherit RoutingEffect("FabulousXamarinForms.ShadowEffect")

    member val Radius = 0. with get, set
    member val Color = Color.Default with get, set
    member val DistanceX = 0. with get, set
    member val DistanceY = 0. with get, set
    
[<AutoOpen>]
module ShadowEffectViewExtension =
    open Fabulous
    open Fabulous.XamarinForms
    
    let RadiusAttribKey = AttributeKey "ShadowEffectRadius"
    let ColorAttribKey = AttributeKey "ShadowEffectColor"
    let DistanceXAttribKey = AttributeKey "ShadowEffectDistanceX"
    let DistanceYAttribKey = AttributeKey "ShadowEffectDistanceY"
    
    type Fabulous.XamarinForms.View with
        static member inline ShadowEffect(?radius, ?color, ?distanceX, ?distanceY) =
            let attribCount = 0
            let attribCount = match radius with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match color with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match distanceX with Some _ -> attribCount + 1 | None -> attribCount
            let attribCount = match distanceY with Some _ -> attribCount + 1 | None -> attribCount
            
            let attribs = AttributesBuilder<ShadowEffect>(attribCount)
                
            let updater1 = ViewExtensions.ValueUpdater(radius, (fun (target: ShadowEffect) v -> target.Radius <- v))
            let updater2 = ViewExtensions.ValueUpdater(color, (fun (target: ShadowEffect) v -> target.Color <- v))
            let updater3 = ViewExtensions.ValueUpdater(distanceX, (fun (target: ShadowEffect) v -> target.DistanceX <- v))
            let updater4 = ViewExtensions.ValueUpdater(distanceY, (fun (target: ShadowEffect) v -> target.DistanceY <- v))

            match radius with None -> () | Some v -> attribs.Add(RadiusAttribKey, v, updater1)
            match color with None -> () | Some v -> attribs.Add(ColorAttribKey, v, updater2)
            match distanceX with None -> () | Some v -> attribs.Add(DistanceXAttribKey, v, updater3)
            match distanceY with None -> () | Some v -> attribs.Add(DistanceYAttribKey, v, updater4)
            
            let create () = ShadowEffect()

            ViewElement.Create(create, attribs.Close())
                