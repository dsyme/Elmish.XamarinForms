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

/// A description of a visual element
type AttributesBuilder (attribCount: int) = 

    let mutable count = 0
    let mutable attribs = Array.zeroCreate<KeyValuePair<int, obj>>(attribCount)    

    /// Get the attributes of the visual element
    [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
    member __.Attributes = 
        if isNull attribs then [| |] 
        else attribs |> Array.map (fun kvp -> KeyValuePair(AttributeKey<int>.GetName kvp.Key, kvp.Value))

    /// Get the attributes of the visual element
    member __.Close() : _[] = 
        let res = attribs 
        attribs <- null
        res

    /// Produce a new visual element with an adjusted attribute
    member __.Add(key: AttributeKey<'T>, value: 'T) = 
        if isNull attribs then failwithf "The attribute builder has already been closed"
        if count >= attribs.Length then failwithf "The attribute builder was not large enough for the added attributes, it was given size %d. Did you get the attribute count right?" attribs.Length
        attribs.[count] <- KeyValuePair(key.KeyValue, box value)
        count <- count + 1


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
type ViewElement (targetType: Type, create: (unit -> obj), update: (AdaptiveToken -> obj -> unit), attribs: KeyValuePair<int, obj>[]) = 
    
    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    let create = create

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    let targetType = targetType

    static member Create<'T> (create: (unit -> 'T), update: (AdaptiveToken -> 'T -> unit), attribs: KeyValuePair<int, obj>[]) =
        ViewElement(typeof<'T>, (create >> box), (fun token target -> update token (unbox target)), attribs)

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
        |> Array.tryPick (fun kvp -> if (kvp.Key = key.KeyValue) then Some (unbox kvp.Value) else None) 
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
    member x.Updater token target = update token target

    /// Create the UI element from the view description
    // TODO: remove all direct calls to this and use ViewElementUpdater instead
    member x.Create(token: AdaptiveToken) = create()

    /// Produce a new visual element with an adjusted attribute
    member __.WithAttribute(key: AttributeKey<'T>, value: 'T) =
        let duplicateViewElement newAttribsLength attribIndex =
            let attribs2 = Array.zeroCreate newAttribsLength
            Array.blit attribs 0 attribs2 0 attribs.Length
            attribs2.[attribIndex] <- KeyValuePair(key.KeyValue, box value)
            ViewElement(targetType, create, update, attribs2)
        
        let n = attribs.Length
        
        let existingAttrIndexOpt = attribs |> Array.tryFindIndex (fun attr -> attr.Key = key.KeyValue)
        match existingAttrIndexOpt with
        | Some i ->
            duplicateViewElement n i // duplicate and replace existing attribute
        | None ->
            duplicateViewElement (n + 1) n // duplicate and add new attribute

    override x.ToString() = sprintf "%s(...)" x.TargetType.Name //(x.GetHashCode())

and 
    [<AbstractClass>]
    ViewElementUpdater(node: ViewElement) = 
    inherit AdaptiveObject()
    let mutable targetOpt = None

    member x.Update(token: AdaptiveToken, scope: obj) =
        x.EvaluateIfNeeded token () (fun token ->
            match targetOpt with 
            | None -> 
                x.CreateAndUpdate(token, scope) |> ignore
            | Some target ->
                node.Updater token target
        )

    member x.CreateAndUpdate(token: AdaptiveToken, scope) =
        Debug.WriteLine (sprintf "Create %O" node.TargetType)
        let target = node.Create(token)
        targetOpt <- Some target
        x.OnCreated (scope, target)
        node.Updater token target
        match node.TryGetAttributeKeyed(ViewElement._CreatedAttribKey) with
        | ValueSome f -> (f.GetValue(token)) target
        | ValueNone -> ()
        match node.TryGetAttributeKeyed(ViewElement._RefAttribKey) with
        | ValueSome f -> (f.GetValue(token)).Set (box target)
        | ValueNone -> ()
        target
    
    abstract OnCreated : scope: obj * target: obj -> unit

    override x.ToString() = "updater for " + node.ToString()

