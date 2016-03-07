module Rexcfnghk.MarkSixParser.Tests.Facts

open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.Models
open Swensen.Unquote
open Xunit

[<Fact>]
let ``checkResults returns error when drawResults's length is not seven`` () =
    let drawResults = []
    let usersDraw = [1..6] |> List.map (MarkSixNumber.create >> ValidationResult.extract)

    let validationResult = MarkSix.checkResults ignore drawResults usersDraw

    let error = match validationResult with
                | Error _ -> true
                | _ -> false

    test <@ error @>

[<Fact>]
let ``checkResults returns error when drawResult contains duplicate`` () =
    let drawResults = 1 |> (List.replicate 7 >> List.mapi (fun i -> MarkSixNumber.create >> ValidationResult.extract >> if i = 6 then ExtraNumber else DrawnNumber))
    let usersDraw = [1..6] |> List.map (MarkSixNumber.create >> ValidationResult.extract)

    let validationResult = MarkSix.checkResults ignore drawResults usersDraw

    let error = match validationResult with
                | Error _ -> true
                | _ -> false

    test <@ error @>

