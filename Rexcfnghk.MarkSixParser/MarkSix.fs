module Rexcfnghk.MarkSixParser.MarkSix

open System
open Models
open ValidationResult
open Combinations

let toUsersDraw usersDrawSet =
    if Set.count usersDrawSet >= 6
    then usersDrawSet |> UsersDraw |> Success
    else 
        "Users draw expects a list of at least six MarkSixNumbers"
        |> ValidationResult.errorFromString

let toDrawResults (drawnNumberSet, extraNumber) =
    match Set.toList drawnNumberSet, extraNumber with
    | [d1; d2; d3; d4; d5; d6], e -> 
        (DrawnNumber d1, DrawnNumber d2, DrawnNumber d3,
         DrawnNumber d4, DrawnNumber d5, DrawnNumber d6,
         ExtraNumber e)
        |> (DrawResults >> Success)
    | _ ->
        "drawResults expects a list of six MarkSixNumbers and one ExtraNumber"
        |> ValidationResult.errorFromString

let randomUsersDraw count (r: Random) =
    let rec randomUsersDrawImpl acc =
        if Set.count acc = count
        then acc
        else
            let m = 
                r.Next(1, 50)
                |> MarkSixNumber.create
                |> ValidationResult.extract

            randomUsersDrawImpl (Set.add m acc)
    
    randomUsersDrawImpl Set.empty
    |> toUsersDraw
    |> ValidationResult.extract

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
        then ValidationResult.success (drawResultsSet, e)
        else "Input draw results contain dupcliate" |> ValidationResult.errorFromString

    let extractUsersDraw usersDraw =
        let (UsersDraw set) = usersDraw

        if Set.count set >= 6
        then ValidationResult.success set
        else 
            "Users Draw must contain at least six elements" 
            |> ValidationResult.errorFromString

    let drawResultsV = extractDrawResults drawResults
    let usersDrawV = extractUsersDraw usersDraw

    match drawResultsV, usersDrawV with
    | Error e, _ | _, Error e -> 
        errorHandler e
        let (ErrorMessage m) = e
        ValidationResult.errorFromString m
    | Success (drawResults, extraNumber), Success usersDraw -> 
        calculatePoints usersDraw (drawResults, extraNumber)
        |> Prize.fromPoints
        |> ValidationResult.success

let checkMultipleUsersDraws 
    resultsChecker 
    (errorHandler: ErrorMessage -> unit) 
    (drawResults: DrawResults) 
    usersDrawList : ValidationResult<Prize.T list> =

    let flattenedCombinations =
        usersDrawList
        |> List.map (function UsersDraw u -> u |> Set.toArray |> combination 6)
        |> List.collect id
        |> List.map (Set.ofList >> UsersDraw)

    resultsChecker errorHandler drawResults
    |> ValidationResult.traverse <| flattenedCombinations