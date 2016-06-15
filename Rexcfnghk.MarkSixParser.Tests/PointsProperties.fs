module Rexcfnghk.MarkSixParser.Tests.PointsProperties

open Rexcfnghk.MarkSixParser
open Models
open Points
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

[<Property>]
let ``Points addition has identity`` d1 =
    let p1 = Points d1
    let p2 = Points 0.m
    p1 .+. p2 =! p1

[<Property>]
let ``Points addition is communicative`` () =
    let pointsArb =
        Gen.elements [1.m .. 6.m]
        |> Gen.map Points
        |> Arb.fromGen
    Prop.forAll pointsArb <| fun p1 ->
        Prop.forAll pointsArb <| fun p2 ->
            p1 .+. p2 =! p2 .+. p1

[<Property>]
let ``Points addition is associative`` () =
    let pointsArb =
        Gen.elements [1.m .. 6.m]
        |> Gen.map Points
        |> Arb.fromGen
    Prop.forAll pointsArb <| fun p1 ->
        Prop.forAll pointsArb <| fun p2 ->
            Prop.forAll pointsArb <| fun p3 ->
                (p1 .+. p2) .+. p3 =! p1 .+. (p2 .+. p3)
