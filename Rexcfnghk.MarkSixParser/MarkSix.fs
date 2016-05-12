module Rexcfnghk.MarkSixParser.MarkSix

open System
open Models
open ValidationResult

let toUsersDraw = function
    | [m1; m2; m3; m4; m5; m6] ->
        (m1, m2, m3, m4, m5, m6)
        |> (UsersDraw >> Success)
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
    fun _ -> r.Next(1, 50)
    |> List.init 6
    |> ValidationResult.traverse MarkSixNumber.create
    >>= toUsersDraw
    |> ValidationResult.extract

let private addUniqueToList maxCount errorHandler getNumber =
    let addToSet acc input =
        let set = Set.ofList acc
        if Set.count set = List.length acc
        then input :: acc |> ValidationResult.success
        else "Adding duplicate elements" |> ValidationResult.errorFromString

    let createFromGetNumber i list = 
        getNumber i
        |> MarkSixNumber.create 
        >>= addToSet list
        |> ValidationResult.retryable errorHandler

    let rec addUniqueToListImpl i acc =
        if i = maxCount
        then List.rev acc
        else
            let updated = createFromGetNumber i acc
            addUniqueToListImpl (i + 1) updated

    addUniqueToListImpl 0 []

let private getNumbers errorHandler f maxCount =
    addUniqueToList maxCount errorHandler
    >> f
    >> ValidationResult.extract

let getDrawResultNumbers errorHandler =
    getNumbers errorHandler toDrawResults 7

let getUsersDrawNumber errorHandler =
    getNumbers errorHandler toUsersDraw 6

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