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
        |> (List.replicate 7 >> List.map (MarkSixNumber.create >> Result.extract))
        |> function
            | [d1; d2; d3; d4; d5; d6; e] ->
                (DrawnNumber d1, DrawnNumber d2, DrawnNumber d3,
                 DrawnNumber d4, DrawnNumber d5, DrawnNumber d6, ExtraNumber e)
                |> DrawResults
            | _ -> failwith "Not expected to be here"
    let usersDraw =
        [1..6]
        |> List.map (MarkSixNumber.create >> Result.extract)
        |> function
            | [_; _; _; _; _; _] as l ->
                l |> Set.ofList |> UsersDraw
            | _ -> failwith "Not expected to be here"

    let result = MarkSix.checkResults ignore drawResults usersDraw

    test <@ match Result with Error _ -> true | _ -> false @>

[<Fact>]
let ``checkResults returns correct prize for sample usersDraw and drawResults`` () =
    let createMarkSixNumber = MarkSixNumber.create >> Result.extract
    let drawNumbers =
        [| 11; 16; 19; 21; 30; 33 |]
        |> Array.map createMarkSixNumber
        |> Set.ofArray
    let extraNumber = createMarkSixNumber 31

    let usersDraw =
        [| 3; 4; 24; 28; 30; 32 |]
        |> Array.map createMarkSixNumber
        |> Set.ofArray

    match MarkSix.toDrawResults (drawNumbers, extraNumber), MarkSix.toUsersDraw usersDraw with
    | _, Error (ErrorMessage e) | Error (ErrorMessage e), _ -> failwith e
    | Ok drawResults, Ok usersDraw ->
        let result = MarkSix.checkResults ignore drawResults usersDraw |> Result.extract
        result =! NoPrize

[<Fact>]
let ``checkResults returns correct prize for sample usersDraw and drawResults 2`` () =
    let createMarkSixNumber = MarkSixNumber.create >> Result.extract
    let drawNumbers =
        [| 11; 16; 19; 21; 30; 33 |]
        |> Array.map createMarkSixNumber
        |> Set.ofArray
    let extraNumber = createMarkSixNumber 31

    let usersDraw =
        [| 11; 14; 19; 31; 38; 39 |]
        |> Array.map createMarkSixNumber
        |> Set.ofArray

    match MarkSix.toDrawResults (drawNumbers, extraNumber), MarkSix.toUsersDraw usersDraw with
    | _, Error (ErrorMessage e) | Error (ErrorMessage e), _ -> failwith e
    | Ok drawResults, Ok usersDraw ->
        let result = MarkSix.checkResults ignore drawResults usersDraw |> Result.extract
        result =! NoPrize

[<Fact>]
let ``toDrawResults respects order of entering`` () =
    let createMarkSixNumber = MarkSixNumber.create >> Result.extract
    let drawNumbers =
        [| 6; 7; 12; 15; 27; 36 |]
        |> Array.map createMarkSixNumber
        |> Set.ofArray
    let extraNumber = createMarkSixNumber 29

    match MarkSix.toDrawResults (drawNumbers, extraNumber) with
    | Error (ErrorMessage e) -> failwith e
    | Ok drawResults ->
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
    let createMarkSixNumber = MarkSixNumber.create >> Result.extract
    let drawNumbers =
        [| 6; 7; 12; 15; 27; 36 |]
        |> Array.map createMarkSixNumber
        |> Set.ofArray
    let extraNumber = createMarkSixNumber 29

    match MarkSix.toDrawResults (drawNumbers, extraNumber) with
    | Error (ErrorMessage e) -> failwith e
    | Ok drawResults ->
        let (DrawResults (DrawnNumber m1, DrawnNumber m2, DrawnNumber m3,
                            DrawnNumber m4, DrawnNumber m5, DrawnNumber m6, ExtraNumber e)) = drawResults
        let i1, i2, i3, i4, i5, i6, e = MarkSixNumber.value m1, MarkSixNumber.value m2, MarkSixNumber.value m3,
                                           MarkSixNumber.value m4, MarkSixNumber.value m5, MarkSixNumber.value m6,
                                           MarkSixNumber.value e

        [i1; i2; i3; i4; i5; i6; e] =! [6; 7; 12; 15; 27; 36; 29]

[<Fact>]
let ``getUsersDrawNumbers respects order of entering`` () =
    let createMarkSixNumber = MarkSixNumber.create >> Result.extract
    let originalUsersDraw =
        [| 6; 7; 12; 15; 27; 36 |]
        |> Array.map createMarkSixNumber
        |> Set.ofArray

    match MarkSix.toUsersDraw originalUsersDraw with
    | Error (ErrorMessage e) -> failwith e
    | Ok usersDraw ->
        let (UsersDraw s) = usersDraw
        s =! originalUsersDraw

[<Fact>]
let ``getDrawResults accepts set of MarkSixNumbers`` () =
    let createMarkSixNumber = MarkSixNumber.create >> Result.extract
    let drawNumbers =
        [| 6; 7; 12; 15; 27; 36 |]
        |> Array.map createMarkSixNumber
        |> Set.ofArray
    let extraNumber = createMarkSixNumber 29

    match MarkSix.toDrawResults (drawNumbers, extraNumber) with
    | Error (ErrorMessage e) -> failwith e
    | Ok drawResults ->
        let (DrawResults (DrawnNumber m1, DrawnNumber m2, DrawnNumber m3,
                            DrawnNumber m4, DrawnNumber m5, DrawnNumber m6, ExtraNumber e)) = drawResults
        let i1, i2, i3, i4, i5, i6, e = MarkSixNumber.value m1, MarkSixNumber.value m2, MarkSixNumber.value m3,
                                           MarkSixNumber.value m4, MarkSixNumber.value m5, MarkSixNumber.value m6,
                                           MarkSixNumber.value e

        [i1; i2; i3; i4; i5; i6; e] =! [6; 7; 12; 15; 27; 36; 29]

[<Fact>]
let ``getUsersDraw accepts set of MarkSixNumbers`` () =
    let createMarkSixNumber = MarkSixNumber.create >> Result.extract
    let usersDrawNumbers =
        [| 6; 7; 12; 15; 27; 36 |]
        |> Array.map createMarkSixNumber
        |> Set.ofArray

    match MarkSix.toUsersDraw usersDrawNumbers with
    | Error (ErrorMessage e) -> failwith e
    | Ok usersDraw ->
        let (UsersDraw s) = usersDraw
        s =! usersDrawNumbers
