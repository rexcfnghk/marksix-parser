module Rexcfnghk.MarkSixParser.Runner.Tests.MarkSixNumberReaderProperties

open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.Runner
open Rexcfnghk.MarkSixParser.Runner.Decision

let m6Gen =
    Gen.elements [1..49]
    |> Gen.map (MarkSixNumber.create >> ValidationResult.extract)

let markSixNumberSetGen count =
    m6Gen
    |> Gen.listOfLength count
    |> Gen.map Set.ofList
    |> Gen.suchThat (fun s -> Set.count s >= count)
    |> Gen.map (Seq.take count >> Set.ofSeq)

let usersDrawArb = 
    markSixNumberSetGen 6
    |> Gen.map (MarkSix.toUsersDraw >> ValidationResult.extract)

[<Property>]
let ``getMultipleUsersDraw can accept multiple users draws`` (NonNegativeInt size) =
    size > 0 ==> lazy
    
    let usersDrawArrayArb =
        usersDrawArb 
        |> Gen.arrayOfLength size
        |> Arb.fromGen

    let decisionArray =
        No :: List.replicate (size - 1) Yes
        |> List.rev
        |> List.toArray

    Prop.forAll usersDrawArrayArb <| fun array ->
        let list = 
            MarkSixNumberReader.getMultipleUsersDraw
                (Array.get array) 
                ignore
                (fun i -> Array.get decisionArray (i - 1))

        list =! Array.toList array