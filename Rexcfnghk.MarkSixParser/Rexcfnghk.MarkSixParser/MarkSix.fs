module Rexcfnghk.MarkSixParser.MarkSix

open System
open Models
open ValidationResult

let randomUsersDraw () =
    let r = Random()
    let l = [ for _ in 1..6 -> 
                r.Next(1, 50) 
                |> MarkSixNumber.create
                |> ValidationResult.extract ]
    match l with
    | [m1; m2; m3; m4; m5; m6] ->
        UsersDraw (m1, m2, m3, m4, m5, m6)
    | _ -> invalidOp "Internal error"

let private addUniqueToList maxCount errorHandler getNumber =
    let addToSet acc input =
        if Set.contains input acc
        then "Adding duplicate elements" |> ValidationResult.errorFromString
        else Set.add input acc |> Success

    let createFromGetNumber set = 
        getNumber >> MarkSixNumber.create >=> addToSet set

    let rec retryableErrorHandler set errorMessage =
        errorHandler errorMessage
        match createFromGetNumber set () with
        | Success s -> s
        | Error e -> retryableErrorHandler set e

    let rec addUniqueToListImpl acc =
        if Set.count acc = maxCount
        then acc |> Set.toList
        else
            let updated =
                createFromGetNumber acc ()
                |> ValidationResult.doubleMap id (retryableErrorHandler acc)

            addUniqueToListImpl updated

    addUniqueToListImpl Set.empty


[<Literal>]
let MaxDrawResultCount = 7

[<Literal>]
let MaxUsersDrawCount = 6

let getDrawResultNumbers errorHandler getNumber =
    match addUniqueToList MaxDrawResultCount errorHandler getNumber with
    | [d1; d2; d3; d4; d5; d6; e] -> 
        (DrawnNumber d1, DrawnNumber d2, DrawnNumber d3,
         DrawnNumber d4, DrawnNumber d5, DrawnNumber d6,
         ExtraNumber e)
        |> DrawResults
    | _ -> invalidOp "Internal error"

let getUsersDrawNumber errorHandler getNumber =
    match addUniqueToList MaxUsersDrawCount errorHandler getNumber with
    | [m1; m2; m3; m4; m5; m6] ->
        UsersDraw (m1, m2, m3, m4, m5, m6)
    | _ -> invalidOp "Internal error"

let checkResults errorHandler drawResults usersDraw =
    let allElementsAreUnique drawResults =
        let set = Set.ofList drawResults
        if Set.count set = List.length drawResults
        then Success drawResults
        else "There are duplicates in draw result list" |> ValidationResult.errorFromString

    let calculatePoints usersDraw (drawResultWithoutExtraNumber, extraNumber) =
        let points = 
            (Set.ofList usersDraw, Set.ofList drawResultWithoutExtraNumber)
            ||> Set.intersect
            |> Set.count
            |> decimal

        let extraPoints = 
            match List.tryFind ((=) extraNumber) usersDraw with
            | Some _ -> 0.5m
            | None -> 0.m

        points + extraPoints |> Points

    let extractedDrawResults =
        let (DrawResults (
                DrawnNumber n1, DrawnNumber n2, DrawnNumber n3, 
                DrawnNumber n4, DrawnNumber n5, DrawnNumber n6, 
                ExtraNumber e)) = drawResults
        [n1; n2; n3; n4; n5; n6; e]

    let extractedUsersDraw =
        let (UsersDraw (n1, n2, n3, n4, n5, n6)) = usersDraw
        [n1; n2; n3; n4; n5; n6]

    let splitDrawResults drawResults =
        match List.rev drawResults with
        | h :: t -> Success (t, h)
        | _ -> 
            "drawResults not in expected format" 
            |> ValidationResult.errorFromString

    let drawResultsValidated, usersDrawValidated =
        extractedDrawResults |> (allElementsAreUnique >=> splitDrawResults),
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
        calculatePoints usersDraw drawResults 
        |> Prize.fromPoints
        |> ValidationResult.success