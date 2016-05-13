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

[<Fact>]
let ``toDrawResults respects order of entering`` () =
    let drawResults = 
        [6; 7; 12; 15; 27; 36; 29]
        |> List.map (MarkSixNumber.create >> ValidationResult.extract)
        |> (MarkSix.toDrawResults >> ValidationResult.extract)

    let (DrawResults (DrawnNumber m1, DrawnNumber m2, DrawnNumber m3, 
                        DrawnNumber m4, DrawnNumber m5, DrawnNumber m6, ExtraNumber e)) = drawResults
    let i1, i2, i3, i4, i5, i6, e = MarkSixNumber.value m1, MarkSixNumber.value m2, MarkSixNumber.value m3, 
                                       MarkSixNumber.value m4, MarkSixNumber.value m5, MarkSixNumber.value m6, 
                                       MarkSixNumber.value e
     
    i1 =! 6
    i2 =! 7
    i3 =! 12
    i4 =! 15
    i5 =! 27
    i6 =! 36
    e =! 29

[<Fact>]
let ``getDrawResults respects order of entering`` () =
    let ints = [| 6; 7; 12; 15; 27; 36; 29 |]
    let mutable i = 0

    let drawResults = MarkSix.getDrawResultNumbers ignore <| fun () -> 
        let result = ints.[i]
        i <- i + 1
        result

    let (DrawResults (DrawnNumber m1, DrawnNumber m2, DrawnNumber m3, 
                        DrawnNumber m4, DrawnNumber m5, DrawnNumber m6, ExtraNumber e)) = drawResults
    let i1, i2, i3, i4, i5, i6, e = MarkSixNumber.value m1, MarkSixNumber.value m2, MarkSixNumber.value m3, 
                                       MarkSixNumber.value m4, MarkSixNumber.value m5, MarkSixNumber.value m6, 
                                       MarkSixNumber.value e
     
    [i1; i2; i3; i4; i5; i6; e] =! List.ofArray ints