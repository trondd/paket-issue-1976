#r "packages/Fake/tools/Fakelib.dll"

open Fake
open Fake.AssemblyInfoFile
open Fake.FileSystemHelper
open Fake.GitVersionHelper
open Fake.ProcessHelper
open Fake.SCPHelper
open Fake.Testing.XUnit2
open Fake.VSTest

open FSharp.Data

open System
open System.IO



Target "Default" DoNothing

Target "Clean"( fun _ ->
  !! "*.sln"
  |> MSBuildRelease "" "Clean"
  |> Log "AppBuild-Output: "
)


Target "Compile"( fun _ ->
    !! "*.sln"
    |> MSBuildRelease "" "Build"
    |> Log "AppBuild-Output: "
)

let paketTemplate folder = PaketTemplate.PaketTemplate (fun p ->
  { p with
      TemplateFilePath = Some ( folder </> "paket.template")
      Version = Some "1.0.0"
      Authors = ["Nobody"]
  })

Target "Package" ( fun _ ->
    ["ClassLibrary"; "YetAnotherClassLibrary"]
    |> List.iter paketTemplate

    Paket.Pack (fun p -> { p with
                            BuildPlatform = "AnyCPU"
                            //IncludeReferencedProjects = true
                            OutputPath = "temp/VersionIsCorrect"
                            })
    Paket.Pack (fun p -> { p with
                            BuildPlatform = "AnyCPU"
                            IncludeReferencedProjects = true
                            OutputPath = "temp/VersionIsMissing"
                            })
)

"Clean"
    ==> "Compile"
    ==> "Package"
    ==> "Default"

RunTargetOrDefault "Default"
