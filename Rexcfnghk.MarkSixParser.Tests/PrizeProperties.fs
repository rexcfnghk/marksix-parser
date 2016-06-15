module Rexcfnghk.MarkSixParser.Tests.PrizeProperties

open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open Rexcfnghk.MarkSixParser.Prize
open Rexcfnghk.MarkSixParser.Models

[<Property>]
let ``Points can be converted to prize`` () =
    let pointsArb =
        [1m .. 0.5m .. 6m]
        |> Gen.elements
        |> Gen.map Points
        |> Arb.fromGen

    let map =
        [|
            (Points 3m, Normal 40m<hkd>)
            (Points 3.5m, Normal 320m<hkd>)
            (Points 4m, Normal 640m<hkd>)
            (Points 4.5m, Normal 9600m<hkd>)
            (Points 5m, Third)
            (Points 5.5m, Second)
            (Points 6m, First)
        |]
        |> Map.ofArray

    Prop.forAll pointsArb <| fun p ->
        let expected = Map.tryFind p map
        let actual = fromPoints p
        test <@ match expected with Some x -> x = actual | None -> actual = NoPrize @>
