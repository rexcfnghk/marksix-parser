module Rexcfnghk.MarkSixParser.Tests.Facts

open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.Models
open Rexcfnghk.MarkSixParser.Prize
open Swensen.Unquote
open Xunit

[<Fact>]
let ``checkResults returns error when drawResult contains duplicate`` () =
    let drawResults = 
        1 
        |> (List.replicate 7 >> List.map (MarkSixNumber.create >> ValidationResult.extract))
        |> function
            | [d1; d2; d3; d4; d5; d6; e] ->
                (DrawnNumber d1, DrawnNumber d2, DrawnNumber d3,
                 DrawnNumber d4, DrawnNumber d5, DrawnNumber d6, ExtraNumber e)
                |> DrawResults
            | _ -> failwith "Not expected to be here"
    let usersDraw = 
        [1..6]
        |> List.map (MarkSixNumber.create >> ValidationResult.extract)
        |> function
            | [m1; m2; m3; m4; m5; m6] ->
                UsersDraw (m1, m2, m3, m4, m5, m6)
            | _ -> failwith "Not expected to be here"

    let validationResult = MarkSix.checkResults ignore drawResults usersDraw

    let error = match validationResult with
                | Error _ -> true
                | _ -> false

    test <@ error @>

[<Fact>]
let ``checkResults returns correct prize for sample usersDraw and drawResults`` () =
    let drawResults = 
        [11; 16; 19; 21; 30; 33; 31]
        |> List.map (MarkSixNumber.create >> ValidationResult.extract)
        |> (MarkSix.toDrawResults >> ValidationResult.extract)

    let usersDraw = 
        [3; 4; 24; 28; 30; 32]
        |> List.map (MarkSixNumber.create >> ValidationResult.extract)
        |> (MarkSix.toUsersDraw >> ValidationResult.extract)

    let result = MarkSix.checkResults ignore drawResults usersDraw |> ValidationResult.extract
    result =! NoPrize

[<Fact>]
let ``checkResults returns correct prize for sample usersDraw and drawResults 2`` () =
    let drawResults = 
        [11; 16; 19; 21; 30; 33; 31]
        |> List.map (MarkSixNumber.create >> ValidationResult.extract)
        |> (MarkSix.toDrawResults >> ValidationResult.extract)

    let usersDraw = 
        [11; 14; 19; 31; 38; 39]
        |> List.map (MarkSixNumber.create >> ValidationResult.extract)
        |> (MarkSix.toUsersDraw >> ValidationResult.extract)
        
    let result = MarkSix.checkResults ignore drawResults usersDraw |> ValidationResult.extract
    result =! NoPrize