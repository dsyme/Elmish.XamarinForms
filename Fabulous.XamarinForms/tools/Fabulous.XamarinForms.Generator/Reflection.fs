// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.Generator

open System
open System.IO
open System.Runtime.Loader
open Fabulous.CodeGen
open Fabulous.CodeGen.AssemblyReader.Models
open Fabulous.XamarinForms.Generator.Helpers

module Reflection =    
    let loadAllAssemblies (paths: seq<string>) =
        let toFullPath p = Path.Combine(Environment.CurrentDirectory, p)
        
        paths
        |> Seq.map (toFullPath >> AssemblyLoadContext.Default.LoadFromAssemblyPath)
        |> Seq.toArray
    
    let tryGetPropertyInAssembly (assembly: System.Reflection.Assembly) (typeName, propertyName) =
        let toCleanTypeName propertyReturnType =
            propertyReturnType.ToString()
                              .Replace("[", "<")
                              .Replace("]", ">")
            |> Text.removeDotNetGenericNotation
        
        nullable {
            let! ``type`` = assembly.GetType(typeName)
            match ``type``.ContainsGenericParameters with
            | true -> return None // Generic types are not supported
            | false ->
                let! propertyInfo = ``type``.GetField(propertyName + "Property")
                let! property = propertyInfo.GetValue(null) :?> Xamarin.Forms.BindableProperty
                return
                    Some
                        { Name = propertyName
                          Type = property.ReturnType |> toCleanTypeName
                          DefaultValue = property.DefaultValue }
        }
                        
                            
    let tryGetProperty (assemblies: System.Reflection.Assembly array) (typeName, propertyName) =
        assemblies
        |> Array.tryPick (fun asm -> tryGetPropertyInAssembly asm (typeName, propertyName))