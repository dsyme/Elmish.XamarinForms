// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.Generator

open System
open System.IO
open Fabulous.CodeGen
open Fabulous.CodeGen.Binder.Models
open Fabulous.CodeGen.Text
open Fabulous.CodeGen.Generator.Models

module CodeGenerator =
//#if DEBUG
    let inlineFlag = ""
//#else
//    let inlineFlag = "inline "
//#endif

    let generateNamespace (namespaceOfGeneratedCode: string) (w: StringWriter) = 
        w.printfn "// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license."
        w.printfn "namespace %s" namespaceOfGeneratedCode
        w.printfn ""
        w.printfn "#nowarn \"59\" // cast always holds"
        w.printfn "#nowarn \"66\" // cast always holds"
        w.printfn "#nowarn \"67\" // cast always holds"
        w.printfn ""
        w.printfn "open FSharp.Data.Adaptive"
        w.printfn "open Fabulous"
        w.printfn ""
        w

    let adaptType (s: string) =
        if s = "ViewElement" || s.EndsWith("alist") then s
        else sprintf "aval<%s>" s

    let generateAttributes (members: AttributeData array) (w: StringWriter) =
        w.printfn "module ViewAttributes ="
        for m in members do
            let memberType =
                match m.Name with
                | "Created" -> adaptType "(obj -> unit)"
                | _ when String.IsNullOrWhiteSpace m.ModelType -> "_"
                | _ -> adaptType m.ModelType
                
            w.printfn "    let %sAttribKey : AttributeKey<%s> = AttributeKey<%s>(\"%s\")" m.UniqueName memberType memberType m.UniqueName
        w.printfn ""
        w

    let generateBuildMemberArgs (data: BuildData) =
        let memberNewLine = "\n                          "
        let members =
            data.Members
            |> Array.mapi (fun index m -> sprintf "%s%s?%s: %s" (if index = 0 then "" else ",") (if index = 0 then "" else memberNewLine) m.Name (adaptType m.InputType))
            |> Array.fold (+) ""

        let immediateMembers =
            data.Members
            |> Array.filter (fun m -> not m.IsInherited)

        let baseMembers =
            match data.BaseName with 
            | None -> ""
            | Some nameOfBaseCreator ->
                data.Members
                |> Array.filter (fun m -> m.IsInherited)
                |> Array.mapi (fun index m -> sprintf "%s%s?%s=%s" (if index = 0 then "" else ", ") (if index > 0 && index % 5 = 0 then memberNewLine else "") m.Name m.Name)
                |> Array.fold (+) ""
        members, baseMembers, immediateMembers

    let generateMemberUpdater (targetFullName: string) (m: UpdateMember) shortName (w: StringWriter) =
        match m with 
        | UpdateProperty p -> 
            let hasApply = not (System.String.IsNullOrWhiteSpace(p.ConvertModelToValue)) || not (System.String.IsNullOrWhiteSpace(p.UpdateCode))
            let generateAttachedProperties (collectionData: UpdatePropertyCollectionData) =
(*
                if collectionData.AttachedProperties.Length > 0 then
                    w.printfn "                        (let updater = (fun _token _childTarget  -> ())"
                    for ap in collectionData.AttachedProperties do
                        let hasApply = not (System.String.IsNullOrWhiteSpace(ap.ConvertModelToValue)) || not (System.String.IsNullOrWhiteSpace(ap.UpdateCode))
                        w.printfn "                         match %s with" ap.Nam
                        w.printfn "                         | None -> updater"
                        w.printfn "                         | Some %s ->" shortName
                        w.printfn "                        (fun _token _childTarget  -> "
Ok this needs more thought.  Attached properties are really weird, going back to the property bag
to get current/previous values.  This ould be kind of slow, also each is deal with in turn.

What's the ideal behaviour here? FOr examples
   Grid([ element.Row(c 3) ] 

We have one AP updater for each child element in the collection. It sits in the collection
but doesn't know staticlly what APs may be present.... Its job is to mediate
changes in all the various AP values for that child element w.r.t. this collection.

It feels like we can assume the presence/absence of APs for the child remains constant. So
we iterate looking for all possible APs, apply these. If any changes the AP updater becomes
dirty and will re-iterate all possible APs for that child and re-apply them and/or update them.

That sounds expensive but it's similar to the fact that when one property changes value in a ViewElement
we run property update on every property statically present in that ViewElement

Note we could create an intermediate AP updater for each AP for each child.



                    w.printfn "                let prev%sOpt = match prevChildOpt with ValueNone -> ValueNone | ValueSome prevChild -> prevChild.TryGetAttributeKeyed<%s>(ViewAttributes.%sAttribKey)" ap.UniqueName ap.ModelType ap.UniqueName
                    w.printfn "                let curr%sOpt = newChild.TryGetAttributeKeyed<%s>(ViewAttributes.%sAttribKey)" ap.UniqueName ap.ModelType ap.UniqueName
                        if ap.ModelType = "ViewElement" && not hasApply then
                            w.printfn "                match prev%sOpt, curr%sOpt with" ap.UniqueName ap.UniqueName
                            w.printfn "                // For structured objects, dependsOn on reference equality"
                            w.printfn "                | ValueSome prevValue, ValueSome newValue when identical prevValue newValue -> ()"
                            w.printfn "                | ValueSome prevValue, ValueSome newValue when canReuseView prevValue newValue ->"
                            w.printfn "                    newValue.UpdateIncremental(prevValue, (%s.Get%s(targetChild)))" targetFullName ap.Name
                            w.printfn "                | _, ValueSome newValue ->"
                            w.printfn "                    %s.Set%s(targetChild, (newValue.Create() :?> %s))" targetFullName ap.Name ap.OriginalType
                            w.printfn "                | ValueSome _, ValueNone ->"
                            w.printfn "                    %s.Set%s(targetChild, null)" targetFullName ap.Name
                            w.printfn "                | ValueNone, ValueNone -> ()"
                        
                        elif not (System.String.IsNullOrWhiteSpace(ap.UpdateCode)) then
                            w.printfn "                %s prev%sOpt curr%sOpt targetChild" ap.UniqueName ap.UniqueName ap.UpdateCode
                        
                        else
                            w.printfn "                match prev%sOpt, curr%sOpt with" ap.UniqueName ap.UniqueName
                            w.printfn "                | ValueSome prevChildValue, ValueSome currChildValue when prevChildValue = currChildValue -> ()"
                            w.printfn "                | _, ValueSome currChildValue -> %s.Set%s(targetChild, %s currChildValue)" targetFullName ap.Name ap.ConvertModelToValue
                            w.printfn "                | ValueSome _, ValueNone -> %s.Set%s(targetChild, %s)" targetFullName ap.Name ap.DefaultValue
                            w.printfn "                | _ -> ()"
                
                        w.printfn "                )"
                else
*)                    
                    w.printfn "                        (fun _token _childTarget  -> ())"
            match p.CollectionData with 
            | Some collectionData when not hasApply ->
                w.printfn "            let updater ="
                w.printfn "                ViewUpdaters.updateViewElementCollection %s FSharp.Core.Operators.id" shortName
                generateAttachedProperties collectionData
                w.printfn "                |> (fun f token (target: %s) -> f token target.%s)" (targetFullName.Replace("'T", "_")) p.Name 
            | Some collectionData ->
                w.printfn "            let updater ="
                w.printfn "                %s %s" p.UpdateCode shortName
                generateAttachedProperties collectionData
            | None when p.ModelType = "ViewElement" && not hasApply -> 
                w.printfn "            let updater ="
                w.printfn "                { new ViewElementUpdater(AVal.constant %s) with" shortName
                w.printfn "                         member __.OnCreated (scope: obj, element: obj) ="
                w.printfn "                             (scope :?> %s).%s <- (element :?> _) }" targetFullName p.Name
                w.printfn "                |> (fun f token target -> f.Update(token, target))"
            | None when not (System.String.IsNullOrWhiteSpace(p.UpdateCode)) ->
                if not (String.IsNullOrWhiteSpace(p.ConvertModelToValue)) then 
                    w.printfn "            let %s = AVal.map %s %s" shortName p.ConvertModelToValue shortName
                w.printfn "            let updater = %s %s" p.UpdateCode shortName
            | None -> 
                w.printfn "            let updater ="
                w.printfn "                ViewUpdaters.valueUpdater %s (fun (target: %s) prevOpt curr ->" shortName targetFullName
                w.printfn "                    match prevOpt with"
                w.printfn "                    | ValueSome prev when prev = curr -> ()"
                w.printfn "                    | _ -> target.%s <- %s curr)" p.Name p.ConvertModelToValue

        | UpdateEvent e -> 
            // TODO: restore this
            //let relatedProperties =
            //    e.RelatedProperties
            //    |> Array.map (fun p -> sprintf "(identical prev%sOpt curr)" p p)
            //    |> Array.fold (fun a b -> a + " && " + b) ""
            if not (String.IsNullOrWhiteSpace(e.ConvertModelToValue)) then
                w.printfn "            let updater = eventUpdater %s %s (* ModelType = %s *) (fun (target: %s) -> target.%s)" shortName e.ConvertModelToValue e.ModelType targetFullName e.Name
            else 
                w.printfn "            let updater = eventUpdater %s makeEventHandler (* ModelType = %s *) (fun (target: %s) -> target.%s)" shortName e.ModelType targetFullName e.Name
        | UpdateAttachedProperty ap -> 
            let hasApply = not (System.String.IsNullOrWhiteSpace(ap.ConvertModelToValue)) || not (System.String.IsNullOrWhiteSpace(ap.UpdateCode))
            if ap.ModelType = "ViewElement" && not hasApply then
                w.printfn "            let updater ="
                w.printfn "                { new ViewElementUpdater(AVal.constant %s) with" shortName
                w.printfn "                         member __.OnCreated (scope: obj, element: obj) ="
                w.printfn "                             %s.Set%s(unbox scope, unbox element) }" targetFullName ap.Name
                w.printfn "                |> (fun f token target -> f.Update(token, target))"
            elif not (System.String.IsNullOrWhiteSpace(ap.UpdateCode)) then
                w.printfn "            let updater = TODO" // (fun _ _ -> ())
                //w.printfn "                %s prev%sOpt curr%sOpt targetChild" ap.UniqueName ap.UniqueName ap.UpdateCode
            else
                w.printfn "            let updater ="
                w.printfn "                ViewUpdaters.valueUpdater %s (fun target prevOpt curr ->" shortName 
                w.printfn "                    match prevOpt with"
                w.printfn "                    | ValueSome prev when prev = curr -> ()"
                w.printfn "                    | _ -> %s.Set%s(target, %s curr))" targetFullName ap.Name ap.ConvertModelToValue

    let generateBuildFunction (data: BuildData) (w: StringWriter) =
        let members, baseMembers, immediateMembers = generateBuildMemberArgs data

        w.printfn "    /// Builds the attributes for a %s in the view" data.Name
        w.printfn "    static member %sBuild%s(attribCount: int, %s) : AttributesBuilder<%s> = " inlineFlag data.Name members data.FullName

        if immediateMembers.Length > 0 then
            w.printfn ""
            for m in immediateMembers do
                w.printfn "        let attribCount = match %s with Some _ -> attribCount + 1 | None -> attribCount" m.Name
            w.printfn ""

        w.printfn "        let attribBuilder ="
        match data.BaseName with 
        | None ->
            w.printfn "            new AttributesBuilder<%s>(attribCount)" data.FullName
        | Some nameOfBaseCreator ->
            w.printfn "            ViewBuilders.Build%s(attribCount, %s).Retarget<%s>()" nameOfBaseCreator baseMembers data.FullName

        for m in data.UpdateMembers do
            w.printfn "        match %s with" m.ShortName
            w.printfn "        | None -> ()"
            w.printfn "        | Some %s ->" m.ShortName
            generateMemberUpdater data.FullName m m.ShortName w
            w.printfn "            attribBuilder.Add(AttributeValue<_, _>(ViewAttributes.%sAttribKey, %s%s %s, updater)) " 
                m.UniqueName 
                (if not (String.IsNullOrWhiteSpace m.ConvertInputToModel) then "AVal.map " else "")
                m.ConvertInputToModel
                m.ShortName

        w.printfn "        attribBuilder"
        w.printfn ""
        w

    let generateCreateFunction (data: CreateData option) (w: StringWriter) =
        match data with
        | None -> w
        | Some data ->
            w.printfn "    static member Create%s () : %s =" data.Name data.FullName
            
            if data.TypeToInstantiate = data.FullName then
                w.printfn "        new %s()" data.TypeToInstantiate
            else
                w.printfn "        upcast (new %s())" data.TypeToInstantiate
            
            w.printfn ""
            w
        
    let memberArgumentType name inputType fullName =
        match name with
        | "created" -> sprintf "(%s -> unit)" fullName
        | "ref" ->     sprintf "ViewRef<%s>" fullName
        | _ -> inputType
        |> adaptType


    let generateConstruct (data: ConstructData option) (w: StringWriter) =
        match data with
        | None -> ()
        | Some data ->
            let memberNewLine = "\n                                  " + String.replicate data.Name.Length " " + " "
            let space = "\n                               "
            let membersForConstructor =
                data.Members
                |> Array.mapi (fun i m ->
                    let commaSpace = if i = 0 then "" else "," + memberNewLine
                    sprintf "%s?%s: %s" commaSpace m.Name (memberArgumentType m.Name m.InputType data.FullName))
                |> Array.fold (+) ""

            let membersForBuild =
                data.Members
                |> Array.map (fun m ->
                    let value = 
                        match m.Name with
                        | "created" -> sprintf "(%s |> Option.map (AVal.map (fun createdFunc -> (unbox<%s> >> createdFunc))))" m.Name data.FullName
                        | "ref" ->     sprintf "(%s |> Option.map (AVal.map (fun (ref: ViewRef<%s>) -> ref.Unbox)))" m.Name data.FullName
                        | _ ->         m.Name
                    sprintf ",%s?%s=%s" space m.Name value)
                |> Array.fold (+) ""

            w.printfn "    static member %sConstruct%s(%s) = " inlineFlag data.Name membersForConstructor
            w.printfn ""
            w.printfn "        let attribBuilder = ViewBuilders.Build%s(0%s)" data.Name membersForBuild
            w.printfn ""
            w.printfn "        ViewElement.Create<%s>(ViewBuilders.Create%s, attribBuilder.Close())" data.FullName data.Name
            w.printfn ""

    let generateBuilders (data: BuilderData array) (w: StringWriter) =
        w.printfn "type ViewBuilders() ="
        for typ in data do
            w
            |> generateBuildFunction typ.Build
            |> generateCreateFunction typ.Create
            |> generateConstruct typ.Construct
        w

    let generateViewers (data: ViewerData array) (w: StringWriter) =
        for typ in data do
            let genericConstraint =
                match typ.GenericConstraint with
                | None -> ""
                | Some constr -> sprintf "<%s>" constr
            
            w.printfn "/// Viewer that allows to read the properties of a ViewElement representing a %s" typ.Name
            w.printfn "type %s%s(element: ViewElement) =" typ.ViewerName genericConstraint

            match typ.InheritedViewerName with
            | None -> ()
            | Some inheritedViewerName ->
                let inheritedGenericConstraint =
                    match typ.InheritedGenericConstraint with
                    | None -> ""
                    | Some constr -> sprintf "<%s>" constr
                
                w.printfn "    inherit %s%s(element)" inheritedViewerName inheritedGenericConstraint

            w.printfn "    do if not ((typeof<%s>).IsAssignableFrom(element.TargetType)) then failwithf \"A ViewElement assignable to type '%s' is expected, but '%%s' was provided.\" element.TargetType.FullName" typ.FullName typ.FullName
            for m in typ.Members do
                match m.Name with
                | "Created" | "Ref" -> ()
                | _ ->
                    w.printfn "    /// Get the value of the %s member" m.Name
                    w.printfn "    member this.%s = element.GetAttributeKeyed(ViewAttributes.%sAttribKey)" m.Name m.UniqueName
            w.printfn ""
        w

    let generateConstructors (data: ConstructorData array) (w: StringWriter) =
        w.printfn "[<AbstractClass; Sealed>]"
        w.printfn "type View private () ="

        for d in data do
            let memberNewLine = "\n                         " + String.replicate d.Name.Length " " + " "
            let space = "\n                               "
            let membersForConstructor =
                d.Members
                |> Array.mapi (fun i m ->
                    let commaSpace = if i = 0 then "" else "," + memberNewLine
                    sprintf "%s?%s: %s" commaSpace m.Name (memberArgumentType m.Name m.InputType d.FullName))
                |> Array.fold (+) ""
            let membersForConstruct =
                d.Members
                |> Array.mapi (fun i m ->
                    let commaSpace = if i = 0 then "" else "," + space
                    sprintf "%s?%s=%s" commaSpace m.Name m.Name)
                |> Array.fold (+) ""

            w.printfn "    /// Describes a %s in the view" d.Name
            w.printfn "    static member %s%s(%s) =" inlineFlag d.Name membersForConstructor
            w.printfn ""
            w.printfn "        ViewBuilders.Construct%s(%s)" d.Name membersForConstruct
            w.printfn ""
        w.printfn ""
        w

    let memberSupportsWith (nm: string) = 
        match nm with 
        | "Created" | "Ref" | "Tag" -> false
        // MINOR TODO: Command and CommandCanExecute are linked attributes and can't be updated using 'With'
        // All in all this is a minor limitation
        | nm when nm.EndsWith("Command") || nm.EndsWith("CommandCanExecute") -> false
        | _ -> true

    let generateViewExtensions (data: ViewExtensionsData array) (w: StringWriter) : StringWriter =
        let newLine = "\n                             "

        w.printfn "[<AutoOpen>]"
        w.printfn "module ViewElementExtensions = "
        w.printfn ""
        w.printfn "    type ViewElement with"

        for m in data do
            if memberSupportsWith m.UniqueName then
                w.printfn ""
                w.printfn "        /// Adjusts the %s property in the visual element" m.UniqueName
                w.printfn "        member x.%s(value: %s) ="
                    m.UniqueName
                    (adaptType m.InputType) 
                generateMemberUpdater m.TargetFullName m.UpdateMember "value" w
                w.printfn "            x.WithAttribute(AttributeValue<_, _>(ViewAttributes.%sAttribKey, %s%s value, updater))"
                    m.UniqueName
                    (if not (String.IsNullOrWhiteSpace m.ConvertInputToModel) then "AVal.map " else "")
                    m.ConvertInputToModel

        let memberArgs =
            data
            |> Array.filter (fun m -> memberSupportsWith m.UniqueName)
            |> Array.mapi (fun index m -> sprintf "%s%s?%s: %s" (if index > 0 then ", " else "") (if index > 0 && index % 5 = 0 then newLine else "") m.LowerUniqueName (adaptType m.InputType))
            |> Array.fold (+) ""

        w.printfn ""
        w.printfn "        member %sx.With(%s) =" inlineFlag memberArgs
        for m in data do
            if memberSupportsWith m.UniqueName then
                w.printfn "            let x = match %s with None -> x | Some opt -> x.%s(opt)" m.LowerUniqueName m.UniqueName
        w.printfn "            x"
        w.printfn ""

        for m in data do
            if memberSupportsWith m.UniqueName then
                w.printfn "    /// Adjusts the %s property in the visual element" m.UniqueName
                w.printfn "    let %s (value: %s) (x: ViewElement) = x.%s(value)" m.LowerUniqueName (adaptType m.InputType) m.UniqueName
        w
        
    let generate data =
        let toString (w: StringWriter) = w.ToString()
        use writer = new StringWriter()
        
        writer
        // adaptive 
        |> generateNamespace data.Namespace
        |> generateAttributes data.Attributes
        |> generateBuilders data.Builders
        |> generateViewers data.Viewers
        |> generateConstructors data.Constructors
        |> generateViewExtensions data.ViewExtensions
        |> toString

    let generateCode
        (prepareData: BoundModel -> GeneratorData)
        (generate: GeneratorData -> string)
        (bindings: BoundModel) : WorkflowResult<string> =
        
        bindings
        |> prepareData
        |> generate
        |> WorkflowResult.ok
