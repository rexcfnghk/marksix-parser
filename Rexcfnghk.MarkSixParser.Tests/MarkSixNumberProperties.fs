module Rexcfnghk.MarkSixParser.Tests.MarkSixNumberProperties

open Rexcfnghk.MarkSixParser
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open System

let outOfRangeArb =
    Arb.Default.Int32().Generator
    |> Gen.filter (fun i -> i < 1 || i > 49)
    |> Arb.fromGen

let markSixNumberGen =
    Gen.elements [1..49]
    |> Gen.map (MarkSixNumber.create >> Result.extract)

[<Property>]
let ``MarkSixNumber.create returns error for integers out of range`` () =
    Prop.forAll outOfRangeArb <| fun x ->
        test <@ match MarkSixNumber.create x with Error _ -> true | _ -> false @>

[<Property>]
let ``Wrapping and unwrapping MarkSixNumber gives same underlying int`` () =
    let markSixNumberArb = markSixNumberGen |> Arb.fromGen
    Prop.forAll markSixNumberArb <| fun m ->
        m
        |> MarkSixNumber.value
        |> MarkSixNumber.create
        |> Result.extract
        =! m

[<Property>]
let ``x less than y infers MarkSixNumber<x> is less than MarkSixNumber<y>`` () =
    let xyGenerator =
        Gen.elements [1..49]
        |> Gen.two
        |> Gen.filter (fun (x, y) -> x < y)
        |> Arb.fromGen
    let extract = MarkSixNumber.create >> Result.extract
    Prop.forAll xyGenerator <| fun (x, y) ->
        let m1, m2 = extract x, extract y
        m1 <! m2

[<Property>]
let ``Equal MarkSixNumbers have equal hash codes`` () =
    let m6Arb =
        Gen.constant 42
        |> Gen.map (MarkSixNumber.create >> Result.extract)
        |> Gen.two
        |> Arb.fromGen

    Prop.forAll m6Arb <| fun (x, y) ->
        hash x =! hash y
