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
        | _ when not m.CanBeUpdated ->
            w.printfn "            let updater = (fun _ _ -> ())"
        | UpdateProperty p -> 
            let hasApply = not (System.String.IsNullOrWhiteSpace(p.ConvertModelToValue)) || not (System.String.IsNullOrWhiteSpace(p.UpdateCode))
            match p.CollectionData with 
            | Some collectionData when not hasApply ->
                w.printfn "            let updater ="
                w.printfn "                ViewUpdaters.updateElementCollection %s FSharp.Core.Operators.id" shortName
                w.printfn "                |> (fun f token (target: %s) -> f token target.%s)" (targetFullName.Replace("'T", "_")) p.Name 
            | Some collectionData ->
                w.printfn "            let updater ="
                w.printfn "                %s %s" p.UpdateCode shortName
            | None when p.ModelType = "ViewElement" && not hasApply -> 
                w.printfn "            let updater = ViewElementUpdater.Create %s FSharp.Core.Operators.id (fun (target: %s) child -> target.%s <- child)" shortName targetFullName p.Name
            | None when not (System.String.IsNullOrWhiteSpace(p.UpdateCode)) ->
                if not (String.IsNullOrWhiteSpace(p.ConvertModelToValue)) then 
                    w.printfn "            let %s = AVal.map %s %s" shortName p.ConvertModelToValue shortName
                w.printfn "            let updater = %s %s" p.UpdateCode shortName
            | None -> 
                w.printfn "            let updater ="
                w.printfn "                ViewUpdaters.valueUpdater %s (fun (target: %s) prevOpt curr ->" shortName targetFullName
                w.printfn "                    match prevOpt with"
                w.printfn "                    | ValueSome prev when prev = curr -> ()"
                w.printfn "                    | _ -> "
                for re in p.RelatedEvents do
                    w.printfn "                        let %sHandlerOpt =" re.ShortName
                    w.printfn "                            match %s with" re.ShortName
                    w.printfn "                            | Some ev -> "
                    w.printfn "                                let hopt = getAssociatedEventHandler ev"
                    w.printfn "                                match hopt with"
                    w.printfn "                                | Some handler -> target.%s.RemoveHandler handler; hopt"  re.Name 
                    w.printfn "                                | None -> None"
                    w.printfn "                            | None -> None" 
                   
                w.printfn "                        target.%s <- %s curr" p.Name p.ConvertModelToValue
                for re in p.RelatedEvents do
                    w.printfn "                        match %sHandlerOpt with" re.ShortName
                    w.printfn "                        | Some handler -> target.%s.AddHandler handler"  re.Name
                    w.printfn "                        | None -> ()" 
                   
                w.printfn "                )"

        | UpdateEvent e -> 
            if not (String.IsNullOrWhiteSpace(e.ConvertModelToValue)) then
                w.printfn "            let updater = eventUpdater %s %s (* ModelType = %s *) (fun (target: %s) -> target.%s)" shortName e.ConvertModelToValue e.ModelType targetFullName e.Name
            else 
                w.printfn "            let updater = eventUpdater %s makeEventHandler (* ModelType = %s *) (fun (target: %s) -> target.%s)" shortName e.ModelType targetFullName e.Name
        | UpdateAttachedProperty ap -> 
            let hasApply = not (System.String.IsNullOrWhiteSpace(ap.ConvertModelToValue)) || not (System.String.IsNullOrWhiteSpace(ap.UpdateCode))
            if ap.ModelType = "ViewElement" && not hasApply then
                w.printfn "            let updater = ViewElementUpdater.Create %s FSharp.Core.Operators.id (fun target content -> %s.Set%s(target, content))" shortName targetFullName ap.Name
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

        match data.BaseName with 
        | None ->
            w.printfn "        let attribBuilder ="
            w.printfn "            new AttributesBuilder<%s>(attribCount)" data.FullName
        | Some nameOfBaseCreator ->
            w.printfn "        let attribBuilderBase ="
            w.printfn "            ViewBuilders.Build%s(attribCount, %s)" nameOfBaseCreator baseMembers
            w.printfn "        let attribBuilder = attribBuilderBase.Retarget<%s>()" data.FullName
            w.printfn "        let _ = (fun () -> attribBuilderBase.Sample :?> %s) |> ignore // enforce subtype relationship" data.FullName

        let events = (data.UpdateMembers |> Array.choose (function (UpdateEvent e) -> Some e | _ -> None))
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

    let memberSupportsWith (m: ViewExtensionsData) = 
        match m.UniqueName with 
        | "Created" | "Ref" | "Tag" -> false
        // MINOR TODO: Command and CommandCanExecute are linked attributes and can't be updated using 'With'
        // All in all this is a minor limitation
        | nm when nm.EndsWith("Command") || nm.EndsWith("CommandCanExecute") -> false
        // MINOR TODO: properties with related events can't be updated as yet because the update logic can't unhook the event handler
        // because it doesn't have access to the handler value
        | _ when (match m.UpdateMember with UpdateProperty p when p.RelatedEvents.Length > 0 -> true | _ -> false) -> false
        | _ -> true

    let generateViewExtensions (data: ViewExtensionsData array) (w: StringWriter) : StringWriter =
        let newLine = "\n                             "

        w.printfn "[<AutoOpen>]"
        w.printfn "module ViewElementExtensions = "
        w.printfn ""
        w.printfn "    type ViewElement with"

        for m in data do
            if memberSupportsWith m then
                w.printfn ""
                w.printfn "        /// Adjusts the %s property in the visual element" m.UniqueName
                w.printfn "        member x.%s(value: %s) =" m.UniqueName (adaptType m.InputType) 
                generateMemberUpdater m.TargetFullName m.UpdateMember "value" w
                w.printfn "            x.WithAttribute(AttributeValue<_, _>(ViewAttributes.%sAttribKey, %s%s value, updater))"
                    m.UniqueName
                    (if not (String.IsNullOrWhiteSpace m.ConvertInputToModel) then "AVal.map " else "")
                    m.ConvertInputToModel

        let memberArgs =
            data
            |> Array.filter memberSupportsWith
            |> Array.mapi (fun index m -> sprintf "%s%s?%s: %s" (if index > 0 then ", " else "") (if index > 0 && index % 5 = 0 then newLine else "") m.LowerUniqueName (adaptType m.InputType))
            |> Array.fold (+) ""

        w.printfn ""
        w.printfn "        member %sx.With(%s) =" inlineFlag memberArgs
        for m in data do
            if memberSupportsWith m then
                w.printfn "            let x = match %s with None -> x | Some opt -> x.%s(opt)" m.LowerUniqueName m.UniqueName
        w.printfn "            x"
        w.printfn ""

        for m in data do
            if memberSupportsWith m then
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
