// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.CodeGen.AssemblyReader

open System.Collections.Generic
open Mono.Cecil

module AssemblyResolver =
    type RegistrableResolver() =
        inherit BaseAssemblyResolver()
        let cache = Dictionary<string, AssemblyDefinition>()

        override this.Resolve(name) =
            match cache.TryGetValue(name.FullName) with
            | true, assembly -> assembly
            | false, _ ->
                let assembly = base.Resolve(name)
                cache.[name.FullName] <- assembly
                assembly

        member this.RegisterAssembly(assembly: AssemblyDefinition) : unit =
            match cache.ContainsKey(assembly.Name.FullName) with
            | true -> ()
            | false -> cache.[assembly.Name.FullName] <- assembly

        override this.Dispose(disposing) =
            base.Dispose(disposing)
            
            if disposing then
                cache.Values |> Seq.iter (fun asm -> asm.Dispose())
                cache.Clear()

    let loadAssembly (resolver: RegistrableResolver) (path: string) =
        let readerParameters = ReaderParameters()
        readerParameters.AssemblyResolver <- resolver
        let assembly = AssemblyDefinition.ReadAssembly(path, readerParameters)
        resolver.RegisterAssembly assembly
        assembly

    let loadAllAssemblies (paths: seq<string>) =
        use resolver = new RegistrableResolver()
        let loadAssembly = loadAssembly resolver
        paths |> Seq.toArray |> Array.map loadAssembly