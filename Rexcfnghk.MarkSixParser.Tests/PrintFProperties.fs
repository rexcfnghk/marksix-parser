module Rexcfnghk.MarkSixParser.Tests.PrintFProperties

open FsCheck
open FsCheck.Xunit
open Rexcfnghk.MarkSixParser

[<Property>]
let ``ValidationResult %O equals to its %A representation in printf`` (v: ValidationResult<int>) =
    sprintf "%A" v = sprintf "%O" v

[<Property>]
let ``Prize %O equals to its %A representation in printf`` (v: Prize.T) =
    sprintf "%A" v = sprintf "%O" v

[<Property>]
let ``MarkSixNumber %O equals to its %A representation in printf`` () =
    let m6Arb =
        Gen.elements [1..49]
        |> Gen.map (MarkSixNumber.create >> ValidationResult.extract)
        |> Arb.fromGen

    Prop.forAll m6Arb <| fun m ->
        sprintf "%A" m = sprintf "%O" m