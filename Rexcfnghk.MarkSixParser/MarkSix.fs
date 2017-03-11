module Rexcfnghk.MarkSixParser.MarkSix

open System
open Models
open Combinations

let toUsersDraw usersDrawSet =
    if Set.count usersDrawSet >= 6
    then usersDrawSet |> UsersDraw |> Ok
    else
        "Users draw expects a list of at least six MarkSixNumbers"
        |> Result.errorFromString

let toDrawResults (drawnNumberSet, extraNumber) =
    match Set.toList drawnNumberSet, extraNumber with
    | [d1; d2; d3; d4; d5; d6], e ->
        (DrawnNumber d1, DrawnNumber d2, DrawnNumber d3,
         DrawnNumber d4, DrawnNumber d5, DrawnNumber d6,
         ExtraNumber e)
        |> (DrawResults >> Ok)
    | _ ->
        "drawResults expects a list of six MarkSixNumbers and one ExtraNumber"
        |> Result.errorFromString

let randomUsersDraw count (r: Random) =
    let rec randomUsersDrawImpl acc =
        if Set.count acc = count
        then acc
        else
            let m =
                r.Next(1, 50)
                |> MarkSixNumber.create
                |> Result.extract

            randomUsersDrawImpl (Set.add m acc)

    randomUsersDrawImpl Set.empty
    |> toUsersDraw
    |> Result.extract

let defaultRandomUsersDraw r = randomUsersDraw 6 r

let checkResults errorHandler drawResults usersDraw =
    let calculatePoints usersDraw (drawResultWithoutExtraNumber, extraNumber) =
        let points =
            (usersDraw, drawResultWithoutExtraNumber)
            ||> Set.intersect
            |> Set.count
            |> decimal

        let extraPoints =
            if Set.contains extraNumber usersDraw
            then 0.5m
            else 0.m

        points + extraPoints |> Points

    let extractDrawResults drawResults =
        let (DrawResults (
                DrawnNumber n1, DrawnNumber n2, DrawnNumber n3,
                DrawnNumber n4, DrawnNumber n5, DrawnNumber n6,
                ExtraNumber e)) = drawResults

        let drawResultsWithoutExtraNumber = [| n1; n2; n3; n4; n5; n6 |]
        let drawResultsSet = drawResultsWithoutExtraNumber |> Set.ofArray

        if Array.length drawResultsWithoutExtraNumber = Set.count drawResultsSet
        then Result.ok (drawResultsSet, e)
        else "Input draw results contain dupcliate" |> Result.errorFromString

    let extractUsersDraw usersDraw =
        let (UsersDraw set) = usersDraw

        if Set.count set >= 6
        then Result.ok set
        else
            "Users Draw must contain at least six elements"
            |> Result.errorFromString

    let drawResultsV = extractDrawResults drawResults
    let usersDrawV = extractUsersDraw usersDraw

    match drawResultsV, usersDrawV with
    | Error e, _ | _, Error e ->
        errorHandler e
        let (ErrorMessage m) = e
        Result.errorFromString m
    | Ok (drawResults, extraNumber), Ok usersDraw ->
        calculatePoints usersDraw (drawResults, extraNumber)
        |> Prize.fromPoints
        |> Result.ok

let checkMultipleUsersDraws
    resultsChecker
    (errorHandler: ErrorMessage -> unit)
    (drawResults: DrawResults)
    usersDrawList : Result<Prize.T list, ErrorMessage> =

    let flattenedCombinations =
        usersDrawList
        |> List.map (function UsersDraw u -> u |> Set.toArray |> combination 6)
        |> List.collect id
        |> List.map (Set.ofList >> UsersDraw)

    resultsChecker errorHandler drawResults
    |> Result.traverse <| flattenedCombinations
