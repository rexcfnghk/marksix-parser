#r @"packages/FAKE/tools/FakeLib.dll"
open System
open System.Diagnostics
open Fake
open Fake.Testing.XUnit2
open Fake.OpenCoverHelper

let buildDir = "./build/"
let testDir = "./tests/"
let testDlls = !! (testDir + "*.Tests.dll")
let deployDir ="./release/"
let isCIBuild = hasBuildParam "ci"
let openCoverResultsXmlPath = testDir + "results.xml"
    
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
                ShadowCopy = not isCIBuild })
)

Target "OpenCover" (fun _ -> 
    OpenCover (fun p ->
        { p with
            ExePath = "packages/OpenCover/tools/OpenCover.Console.exe"
            TestRunnerExePath = "packages/xunit.runner.console/tools/xunit.console.exe"
            Filter = "+[Rexcfnghk.MarkSixParser*]Rexcfnghk.* -[Rexcfnghk.MarkSixParser.Tests]*"
            Register = RegisterUser
            Output = openCoverResultsXmlPath })
        <| testDir + "Rexcfnghk.MarkSixParser.Tests.dll -noshadow")
        
Target "SendToCoveralls" (fun _ -> 
    let configStartProcessInfoF (info: ProcessStartInfo) =
        info.FileName <- "./packages/coveralls.io/tools/coveralls.net.exe"
        info.Arguments <- sprintf "--opencover %s" openCoverResultsXmlPath

    let result = ExecProcess configStartProcessInfoF (TimeSpan.FromMinutes 1.)
    
    if result <> 0 then
        failwith "Cannot send code coverage results to Coveralls"
)

Target "RunReportGenerator" (fun _ -> 
    let configStartProcessInfoF (info: ProcessStartInfo) =
        info.FileName <- "./packages/ReportGenerator/tools/ReportGenerator.exe"
        info.Arguments <- sprintf "-reports:%s -targetdir:%s/report" openCoverResultsXmlPath testDir
    
    let result = ExecProcess configStartProcessInfoF (TimeSpan.FromMinutes 1.)
    
    if result <> 0 then
        failwith "Cannot run ReportGenerator"
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
    ==> "OpenCover"
    =?> ("SendToCoveralls", isCIBuild)
    ==> "Pack"
        
"OpenCover"
    ==> "RunReportGenerator"
        
RunTargetOrDefault "Pack"
