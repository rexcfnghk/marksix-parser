#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing.XUnit2
open Fake.OpenCoverHelper

let buildDir = "./build/"
let testDir = "./tests/"
let testDlls = !! (testDir + "*.Tests.dll")
let deployDir ="./release/"
    
Target "Clean" (fun _ -> CleanDirs [ buildDir; testDir; deployDir ])

Target "BuildRunner" (fun _ ->
    !! "Rexcfnghk.MarkSixParser.*/*.fsproj"
        -- "Rexcfnghk.MarkSixParser.Tests/*.fsproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "Runner built"
)

Target "BuildTests" (fun _ ->
    !! "Rexcfnghk.MarkSixParser.Tests/*.fsproj"
        |> MSBuildDebug testDir "Build"
        |> Log "Tests built")
        
Target "RunTests" (fun _ ->
    testDlls
        |> xUnit2 (fun p -> 
            { p with 
                ShadowCopy = not <| hasBuildParam "ci" })
)

Target "OpenCover" (fun _ -> 
    OpenCover (fun p ->
        { p with
            ExePath = "packages/OpenCover/tools/OpenCover.Console.exe"
            TestRunnerExePath = "packages/xunit.runner.console/tools/xunit.console.exe"
            Filter = "+[Rexcfnghk.MarkSixParser*]Rexcfnghk.* -[Rexcfnghk.MarkSixParser.Tests]*"
            Register = RegisterUser
            Output = testDir + "results.xml" })
        <| testDir + "Rexcfnghk.MarkSixParser.Tests.dll -noshadow")

Target "Pack" (fun _ ->
    CreateDir deployDir
    !! (buildDir + "/**/*.*")
        -- "*.zip"
        |> Zip buildDir (deployDir + "marksix-parser.zip")
) 

"Clean"
    ==> "BuildRunner"
    ==> "BuildTests"
    ==> "RunTests"
    ==> "OpenCover"
    ==> "Pack"
        
RunTargetOrDefault "Pack"
