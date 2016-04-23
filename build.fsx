#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing.XUnit2

let buildDir = "./build/"
let testDir = "./tests/"
let testDlls = !! (testDir + "*.Tests.dll")
let deployDir ="./release/"
    
Target "Clean" (fun _ -> CleanDirs [ buildDir; testDir ])

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
                ShadowCopy = hasBuildParam "ci" })
)

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
    ==> "Pack"
        
RunTargetOrDefault "Pack"