#r @"packages/FAKE/tools/FakeLib.dll"
#r @"packages/FSharpLint.Fake/tools/FSharpLint.Fake.dll"

open System
open System.Diagnostics
open System.IO
open Fake
open Fake.Testing.XUnit2
open Fake.OpenCoverHelper
open FSharpLint.Fake

let buildDir = "./build/"
let testDir = "./tests/"
let testDlls = !! (testDir + "*.Tests.dll")
let deployDir ="./release/"
let isCIBuild = hasBuildParam "ci"
let openCoverResultsXmlPath = testDir @@ "results.xml"
let packageDir = Directory.GetCurrentDirectory() @@ "packages/"
    
Target "Clean" (fun _ -> CleanDirs [ buildDir; testDir; deployDir ])

Target "Lint" (fun _ ->
    !! "**/*.fsproj"
        |> Seq.iter (FSharpLint id)
)

Target "BuildRunner" (fun _ ->
    !! "Rexcfnghk.MarkSixParser.*/*.fsproj"
        -- "Rexcfnghk.MarkSixParser*.Tests/*.fsproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "Runner built"
)

Target "BuildTests" (fun _ ->
    !! "Rexcfnghk.MarkSixParser*.Tests/*.fsproj"
        |> MSBuildDebug testDir "Build"
        |> Log "Tests built")
        
Target "RunTests" (fun _ ->
    testDlls
        |> xUnit2 (fun p -> 
            { p with 
                ShadowCopy = not isCIBuild })
)

Target "OpenCover" (fun _ ->
    let configStartProcessInfoF (info: ProcessStartInfo) =
        let xunitExePath = packageDir @@ "xunit.runner.console/tools/xunit.console.exe"
        let filter = "+[Rexcfnghk.MarkSixParser*]Rexcfnghk.* -[Rexcfnghk.MarkSixParser*.Tests]*"
        let targetArgs = 
            ["Rexcfnghk.MarkSixParser.Tests.dll"; "Rexcfnghk.MarkSixParser.Runner.Tests.dll"]
            |> List.map ((@@) testDir)
            |> String.concat " "
        info.FileName <- packageDir @@ "OpenCover/tools/OpenCover.Console.exe"
        info.Arguments <- 
            sprintf """-target:"%s" -targetargs:"%s -noshadow" -output:"%s" -filter:"%s" %s"""
                xunitExePath targetArgs openCoverResultsXmlPath filter (if hasBuildParam "travis" then String.Empty else "-register:user")
                
    let result = ExecProcess configStartProcessInfoF (TimeSpan.FromMinutes 5.)
    
    if result <> 0 then
        failwith "Cannot run OpenCover"
)
        
Target "SendToCoveralls" (fun _ -> 
    let configStartProcessInfoF (info: ProcessStartInfo) =
        info.FileName <- packageDir @@ "coveralls.io/tools/coveralls.net.exe"
        info.Arguments <- sprintf "--opencover %s" openCoverResultsXmlPath

    let result = ExecProcess configStartProcessInfoF (TimeSpan.FromMinutes 1.)
    
    if result <> 0 then
        failwith "Cannot send code coverage results to Coveralls"
)

Target "RunReportGenerator" (fun _ -> 
    let configStartProcessInfoF (info: ProcessStartInfo) =
        info.FileName <- packageDir @@ "ReportGenerator/tools/ReportGenerator.exe"
        info.Arguments <- sprintf "-reports:%s -targetdir:%sreport" openCoverResultsXmlPath testDir
    
    let result = ExecProcess configStartProcessInfoF (TimeSpan.FromMinutes 1.)
    
    if result <> 0 then
        failwith "Cannot run ReportGenerator"
)

Target "Pack" (fun _ ->
    CreateDir deployDir
    !! (buildDir + "/**/*.*")
        -- "*.zip"
        |> Zip buildDir (deployDir @@ "marksix-parser.zip")
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
