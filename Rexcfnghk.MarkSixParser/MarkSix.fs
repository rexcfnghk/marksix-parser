module Rexcfnghk.MarkSixParser.MarkSix

open System
open Models
open ValidationResult

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

let randomUsersDraw count =
    let r = Random()

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

let defaultRandomUsersDraw () = randomUsersDraw 6

let checkResults errorHandler drawResults usersDraw =
    let allElementsAreUnique list =
        let set = Set.ofList list
        if Set.count set = List.length list
        then Success list
        else "There are duplicates in draw result list" |> ValidationResult.errorFromString

    let calculatePoints usersDraw (drawResultWithoutExtraNumber, extraNumber) =
        let usersDrawSet, drawResultWithoutExtraNumberSet =
            Set.ofList usersDraw, Set.ofList drawResultWithoutExtraNumber
        
        let points = 
            (usersDrawSet, drawResultWithoutExtraNumberSet)
            ||> Set.intersect
            |> Set.count
            |> decimal

        let extraPoints = 
            if Set.contains extraNumber usersDrawSet
            then 0.5m 
            else 0.m

        points + extraPoints |> Points

    let extractedDrawResults =
        let (DrawResults (
                DrawnNumber n1, DrawnNumber n2, DrawnNumber n3, 
                DrawnNumber n4, DrawnNumber n5, DrawnNumber n6, 
                ExtraNumber e)) = drawResults
        [n1; n2; n3; n4; n5; n6], e

    let extractedUsersDraw =
        let (UsersDraw s) = usersDraw
        Set.toList s

    let drawResults, extraNumber = extractedDrawResults

    let drawResultsValidated, usersDrawValidated =
        drawResults |> allElementsAreUnique,
        extractedUsersDraw |> allElementsAreUnique

    match usersDrawValidated, drawResultsValidated with
    | Error e, _ | _, Error e -> 
        errorHandler e
        let (ErrorMessage m) = e
        ValidationResult.errorFromString m
    | Success usersDraw, Success drawResults -> 
        calculatePoints usersDraw (drawResults, extraNumber)
        |> Prize.fromPoints
        |> ValidationResult.success