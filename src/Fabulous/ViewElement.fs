// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

#nowarn "67" // cast always holds

open System
open System.Collections.Generic
open System.Diagnostics
open FSharp.Data.Adaptive

[<AutoOpen>]
module internal AttributeKeys = 
    let attribKeys = Dictionary<string,int>()
    let attribNames = Dictionary<int,string>()

[<Struct>]
type AttributeKey<'T> internal (keyv: int) = 

    static let getAttribKeyValue (attribName: string) : int = 
        match attribKeys.TryGetValue(attribName) with 
        | true, keyv -> keyv
        | false, _ -> 
            let keyv = attribKeys.Count + 1
            attribKeys.[attribName] <- keyv
            attribNames.[keyv] <- attribName
            keyv

    new (keyName: string) = AttributeKey<'T>(getAttribKeyValue keyName)

    member __.KeyValue = keyv

    member __.Name = AttributeKey<'T>.GetName(keyv)

    static member GetName(keyv: int) = 
        match attribNames.TryGetValue(keyv) with 
        | true, keyv -> keyv
        | false, _ -> failwithf "unregistered attribute key %d" keyv

    override x.ToString() = x.Name

type AttributeValue(key: int, value: obj, update: AdaptiveToken -> obj -> unit) =
    member x.KeyValue = key
    member x.Value = value
    member x.UpdateInternal(token, target) = update token target

type AttributeValue<'T>(key: AttributeKey<'T>, value: 'T, update: AdaptiveToken -> obj -> unit) =
    inherit AttributeValue(key.KeyValue, value, update)
    member x.Key = key
    member x.Value = value
    member x.UpdateInternal(token, target) = update token target

type AttributeValue<'T, 'Target>(key: AttributeKey<'T>, value: 'T, update: AdaptiveToken -> 'Target -> unit) =
    inherit AttributeValue<'T>(key, value, (fun token (target: obj) -> update token (unbox target)))

/// A description of a visual element
type AttributesBuilder (attribCount: int) = 

    let mutable count = 0
    let mutable attribs = Array.zeroCreate<AttributeValue>(attribCount)    

    /// Get the attributes of the visual element
    [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
    member __.Attributes = 
        if isNull attribs then [| |] 
        else attribs |> Array.map (fun kvp -> KeyValuePair(AttributeKey<int>.GetName kvp.KeyValue, kvp.Value))

    /// Get the attributes of the visual element
    member __.Close() : _[] = 
        let res = attribs 
        attribs <- null
        res

    /// Produce a new visual element with an adjusted attribute
    member __.Add(attrib: AttributeValue) = 
        if isNull attribs then failwithf "The attribute builder has already been closed"
        if count >= attribs.Length then failwithf "The attribute builder was not large enough for the added attributes, it was given size %d. Did you get the attribute count right?" attribs.Length
        attribs.[count] <- attrib
        count <- count + 1


type AttributesBuilder<'Target> (builder: AttributesBuilder) = 
    new (attribCount: int) = AttributesBuilder<'Target>(AttributesBuilder(attribCount))

    [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
    member this.Attributes = this.Attributes

    /// Produce a new visual element with an adjusted attribute
    member this.Add(attrib: AttributeValue<'T, 'Target>) = 
        builder.Add(attrib :> AttributeValue)

    /// Produce a new visual element with an adjusted attribute
    member this.Add(key: AttributeKey<'T>, value: 'T, update: AdaptiveToken -> 'Target -> unit) = 
        builder.Add(AttributeValue<'T, 'Target>(key, value, update))

    member __.Close() : _[] = builder.Close()

    /// Cast to a builder for a different kind of target, usually a sub-type to add further attributes
    member this.Retarget<'Target2>() : AttributesBuilder<'Target2> =
        AttributesBuilder<'Target2>(builder)

    member this.Sample = Unchecked.defaultof<'Target>

type ViewRef() = 
    let handle = System.WeakReference<obj>(null)

    member __.Set(target: obj) : unit = 
        handle.SetTarget(target)

    member __.TryValue = 
        match handle.TryGetTarget() with 
        | true, null -> None
        | true, res -> Some res 
        | _ -> None

type ViewRef<'T when 'T : not struct>() = 
    let handle = ViewRef()

    member __.Set(target: 'T) : unit =  handle.Set(box target)
    member __.Value : 'T = 
        match handle.TryValue with 
        | Some res -> unbox res
        | None -> failwith "view reference target has been collected or was not set"

    member __.Unbox = handle

    member __.TryValue : 'T option = 
        match handle.TryValue with 
        | Some res -> Some (unbox res)
        | _ -> None

/// An adaptive description of a visual element
type ViewElement (targetType: Type, create: (unit -> obj), attribs: AttributeValue[]) = 
    
    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    let create = create

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    let targetType = targetType

    static member Create<'T> (create: (unit -> 'T), attribs: AttributeValue[]) =
        ViewElement(typeof<'T>, (create >> box), attribs)

    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    static member val _CreatedAttribKey : AttributeKey<aval<obj -> unit>> = AttributeKey<_>("Created")

    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    static member val _RefAttribKey : AttributeKey<aval<ViewRef>> = AttributeKey<_>("Ref")

    /// Get the name of the type created by the visual element
    member x.TargetName = targetType.Name

    /// Get the type created by the visual element
    member x.TargetType = targetType

    /// Get the attributes of the visual element
    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member x.Attributes = attribs

    /// Get an attribute of the visual element
    member x.TryGetAttributeKeyed<'T>(key: AttributeKey<'T>) : 'T voption = 
        attribs 
        |> Array.tryPick (fun kvp -> if (kvp.KeyValue = key.KeyValue) then Some (unbox kvp.Value) else None) 
        |> function None -> ValueNone | Some x -> ValueSome x

    /// Get an attribute of the visual element
    member x.TryGetAttribute<'T>(name: string) : 'T voption = 
        x.TryGetAttributeKeyed<'T>(AttributeKey<'T> name)
 
    /// Get an attribute of the visual element
    member x.GetAttributeKeyed<'T>(key: AttributeKey<'T>) : 'T = 
        match x.TryGetAttributeKeyed(key) with
        | ValueSome v -> v
        | ValueNone -> failwithf "Property '%s' does not exist on %s" key.Name x.TargetType.Name

    /// Differentially update a visual element given the previous settings
    // TODO: remove all direct calls to this and use ViewElementUpdater instead
    member x.UpdateInternal token target = 
        for a in attribs do
           a.UpdateInternal(token, target)

    /// Create the UI element from the view description
    // TODO: remove all direct calls to this and use ViewElementUpdater instead
    member x.CreateInternal(token: AdaptiveToken) = create()

    /// Produce a new visual element with an adjusted attribute
    member __.WithAttribute(attrib: AttributeValue<'T>) =
        let duplicateViewElement newAttribsLength attribIndex =
            let attribs2 = Array.zeroCreate newAttribsLength
            Array.blit attribs 0 attribs2 0 attribs.Length
            attribs2.[attribIndex] <- (attrib :> AttributeValue)
            ViewElement(targetType, create, attribs2)
        
        let n = attribs.Length
        
        let existingAttrIndexOpt = attribs |> Array.tryFindIndex (fun attr -> attr.KeyValue = attrib.KeyValue)
        match existingAttrIndexOpt with
        | Some i ->
            duplicateViewElement n i // duplicate and replace existing attribute
        | None ->
            duplicateViewElement (n + 1) n // duplicate and add new attribute

    override x.ToString() = sprintf "%s(...)" x.TargetType.Name //(x.GetHashCode())

type ViewElementUpdater(anode: aval<ViewElement>, onCreated: obj -> obj -> unit, childTransform: obj -> obj) = 
    inherit AdaptiveObject()
    let mutable targetOpt = None

    let rec canReuseView token (prevChild: ViewElement) (newChild: ViewElement) =
        
        // TODO: this update logic is too pessimistic. We are only reusing elements if the
        // ViewElement is physical-identical, which means a needlessly-recomputing ViewElement
        // will tear down the target element and re-create it on every change.
        //
        // This is a hard problem - we would effectively have to apply non-adaptive Fabulous-style
        // diff'ing on the ViewElements to transfer the target element successfully...
        if FSharp.Core.LanguagePrimitives.PhysicalEquality prevChild newChild   
              // prevChild.TargetType = newChild.TargetType 
              //&& canReuseAutomationId token prevChild newChild 
        then
            //if newChild.TargetType.IsAssignableFrom(typeof<NavigationPage>) then
            //    canReuseNavigationPage prevChild newChild
            //elif newChild.TargetType.IsAssignableFrom(typeof<CustomEffect>) then
            //    canReuseCustomEffect prevChild newChild
            //else
                true
        else
            false

    // TODO: add this logic back in (it is specific to Xamarin.Forms...)
    //
    /// Checks whether an underlying NavigationPage control can be reused given the previous and new view elements
    //
    // NavigationPage can be reused only if the pages don't change their type (added/removed pages don't prevent reuse)
    // E.g. If the first page switch from ContentPage to TabbedPage, the NavigationPage can't be reused.
    //and canReuseNavigationPage (prevChild:ViewElement) (newChild:ViewElement) =
    //    let prevPages = prevChild.TryGetAttribute<ViewElement alist>("Pages")
    //    let newPages = newChild.TryGetAttribute<ViewElement alist>("Pages")
    //    match prevPages, newPages with
    //    | ValueSome prevPages, ValueSome newPages -> prevPages.Count = newPages.Count && (prevPages, newPages) ||> Seq.forall2 canReuseView
    //    | _, _ -> true

    /// Checks whether the control can be reused given the previous and the new AutomationId.
    /// Xamarin.Forms can't change an already set AutomationId
    //
    // TODO: make the AutomationId non-adapting
    //and canReuseAutomationId token (prevChild: ViewElement) (newChild: ViewElement) = true
        //let prevAutomationId = prevChild.TryGetAttribute<aval<string>>("AutomationId")
        //let newAutomationId = newChild.TryGetAttribute<aval<string>>("AutomationId")

        //match prevAutomationId, newAutomationId  with
        //| ValueSome _, ValueNone 
        //| ValueNone, ValueSome _ -> false
        //| ValueSome o, ValueSome n   when o.GetValue(token) <> n.GetValue(token) -> false
        //| _ -> true

    member x.SetTarget(target) = targetOpt <- Some (None, target)
    member x.Target = snd targetOpt.Value

    member x.Update(token: AdaptiveToken, scope: obj) =
        x.EvaluateIfNeeded token () (fun token ->
            let node = anode.GetValue(token)
            match targetOpt with 
            | Some (Some prevNode, target) when canReuseView token prevNode node ->
                node.UpdateInternal token target
            | Some (None, target) ->
                node.UpdateInternal token target
            | _ -> 
                Debug.WriteLine (sprintf "Create %O" node.TargetType)

                let target = node.CreateInternal(token) |> childTransform
                targetOpt <- Some (Some node, target)
                // Note, update before registering in scope, e.g. "Pin must have a label to be added to a map"
                node.UpdateInternal token target
                onCreated scope target

                match node.TryGetAttributeKeyed(ViewElement._CreatedAttribKey) with
                | ValueSome f -> (f.GetValue(token)) target
                | ValueNone -> ()

                match node.TryGetAttributeKeyed(ViewElement._RefAttribKey) with
                | ValueSome f -> (f.GetValue(token)).Set (box target)
                | ValueNone -> ()
        )

    override x.ToString() = "updater for " + anode.ToString()

    static member CreateAdaptive node  (childTransform: 'ChildTarget -> 'ActualChildTarget) (onCreated: 'Target -> 'ActualChildTarget -> unit)=
        let updater = 
            new ViewElementUpdater(node, 
                  (fun (scope: obj) (child: obj) -> onCreated (unbox<'Target> scope) (unbox<'ActualChildTarget> child)), 
                  (fun (childObj: obj) -> childTransform (unbox<'ChildTarget> childObj) |> box))
        fun token (target: 'Target) -> 
            updater.Update(token, target)

    static member Create node (childTransform: 'ChildTarget -> 'ActualChildTarget) (onCreated: 'Target -> 'ActualChildTarget -> unit)=
        ViewElementUpdater.CreateAdaptive (AVal.constant node) childTransform onCreated 

/// Represents a change in data that causes a trigger, e.g. for scrolling
[<RequireQualifiedAccess>]
type Trigger<'T when 'T : equality> internal (value: 'T option, stamp: int64) = 
    static let mutable triggerCount = 0L
    static let fresh() = 
        triggerCount <- triggerCount + 1L
        triggerCount
    static let none = Trigger<'T>(None, fresh())
    new (x: 'T) = new Trigger<'T>(Some x, fresh())
    member __.Stamp = stamp
    member __.Value = value.Value
    member __.TryValue = value
    member __.IsNone = value.IsNone
    static member None = none
    override __.Equals(obj: obj) = 
         match obj with 
         | :? Trigger<'T> as y -> stamp = y.Stamp && value = y.TryValue
         | _ -> false
    override __.GetHashCode() = int32 stamp + hash value.Value
 

// TODO - add combinators to allow building ViewElement that bind etc.
//module ViewElement =
//     let bind aval f =
//         ViewElement(AVal.bind aval )