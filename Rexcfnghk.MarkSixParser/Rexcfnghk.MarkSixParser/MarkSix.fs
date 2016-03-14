module Rexcfnghk.MarkSixParser.MarkSix

open System
open Models
open ValidationResult

let toUsersDraw = function
    | [m1; m2; m3; m4; m5; m6] ->
        UsersDraw (m1, m2, m3, m4, m5, m6)
        |> Success
    | _ -> 
        "Users draw expects a list of six MarkSixNumbers"
        |> ValidationResult.errorFromString

let toDrawResults = function
    | [d1; d2; d3; d4; d5; d6; e] -> 
        (DrawnNumber d1, DrawnNumber d2, DrawnNumber d3,
         DrawnNumber d4, DrawnNumber d5, DrawnNumber d6,
         ExtraNumber e)
        |> (DrawResults >> Success)
    | _ ->
        "drawResults expects a list of six MarkSixNumbers and one ExtraNumber"
        |> ValidationResult.errorFromString

let randomUsersDraw () =
    let r = Random()
    let l = [ for _ in 1..6 -> 
                r.Next(1, 50) 
                |> MarkSixNumber.create
                |> ValidationResult.extract ]
    l |> toUsersDraw |> ValidationResult.extract

let private addUniqueToList maxCount errorHandler getNumber =
    let addToSet acc input =
        if Set.contains input acc
        then "Adding duplicate elements" |> ValidationResult.errorFromString
        else Set.add input acc |> Success

    let createFromGetNumber set = 
        getNumber >> MarkSixNumber.create >=> addToSet set

    let rec retryableErrorHandler set =
        match createFromGetNumber set () with
        | Success s -> s
        | Error e -> 
            errorHandler e
            retryableErrorHandler set

    let rec addUniqueToListImpl acc =
        if Set.count acc = maxCount
        then acc |> Set.toList
        else
            let updated = retryableErrorHandler acc
            addUniqueToListImpl updated

    addUniqueToListImpl Set.empty

let getDrawResultNumbers errorHandler getNumber =
    let maxDrawResultCount = 7
    addUniqueToList maxDrawResultCount errorHandler getNumber
    |> toDrawResults
    |> ValidationResult.extract

let getUsersDrawNumber errorHandler getNumber =
    let maxUsersDrawCount = 6
    addUniqueToList maxUsersDrawCount errorHandler getNumber
    |> toUsersDraw
    |> ValidationResult.extract

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
        let (UsersDraw (n1, n2, n3, n4, n5, n6)) = usersDraw
        [n1; n2; n3; n4; n5; n6]

    let drawResults, extraNumber = extractedDrawResults

    let drawResultsValidated, usersDrawValidated =
        drawResults |> allElementsAreUnique,
        extractedUsersDraw |> allElementsAreUnique

    match usersDrawValidated, drawResultsValidated with
    | Error e, Success _ | Success _, Error e -> 
        errorHandler e
        Error e
    | Error e1, Error e2 ->
        errorHandler e1
        errorHandler e2
        let (ErrorMessage m1, ErrorMessage m2) = e1, e2
        m1 + m2 |> ValidationResult.errorFromString
    | Success usersDraw, Success drawResults -> 
        calculatePoints usersDraw (drawResults, extraNumber)
        |> Prize.fromPoints
        |> ValidationResult.success