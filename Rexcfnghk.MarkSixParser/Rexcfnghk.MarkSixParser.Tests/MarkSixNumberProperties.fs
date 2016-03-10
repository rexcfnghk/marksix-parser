module Rexcfnghk.MarkSixParser.Tests.MarkSixNumberProperties

open Rexcfnghk.MarkSixParser
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

let outOfRangeArb =
    Arb.Default.Int32().Generator
    |> Gen.suchThat (fun i -> i < 1 || i > 49)
    |> Arb.fromGen

let markSixNumberGen =
    Gen.elements [1..49]
    |> Gen.map (MarkSixNumber.create >> ValidationResult.extract)

[<Property>]
let ``MarkSixNumber.create returns error for integers out of range`` () =
    Prop.forAll outOfRangeArb <| fun x -> 
        test <@ match MarkSixNumber.create x with Error _ -> true | _ -> false @>

[<Property>]
let ``MarkSixNumber's custom equality works`` () =
    let markSixNumberArb = markSixNumberGen |> Arb.fromGen
    Prop.forAll markSixNumberArb <| fun m -> 
        m 
        |> MarkSixNumber.value
        |> MarkSixNumber.create
        |> ValidationResult.extract
        =! m