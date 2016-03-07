module Rexcfnghk.MarkSixParser.MarkSix

open System
open System.Collections.Generic
open Models
open Points
open Prize
open ValidationResult

let drawNumbers () =
    let r = Random()
    [ for _ in 1..6 -> 
        r.Next(1, 50) 
        |> MarkSixNumber.create
        |> ValidationResult.extract ]

let private addUniqueToList maxCount successHandler errorHandler getNumber =
    let createFromGetNumber = getNumber >> MarkSixNumber.create

    let rec retryErrorHandler successHandler e = 
        errorHandler e
        match createFromGetNumber () with
        | Success x -> successHandler x
        | Error e -> retryErrorHandler successHandler e

    // FSharpSet requires comparison, which is not necessary in this case
    let rec addUniqueToListImpl (acc: HashSet<'T>) =
        let count = acc.Count
        if count = maxCount
        then acc |> Seq.toList
        else
            let innerSuccessHandler = successHandler count
            let element = 
                createFromGetNumber ()
                |> ValidationResult.doubleMap innerSuccessHandler (retryErrorHandler innerSuccessHandler)
            acc.Add element |> ignore
            addUniqueToListImpl acc

    addUniqueToListImpl <| HashSet()

[<Literal>]
let MaxDrawResultCount = 7

[<Literal>]
let MaxUsersDrawCount = 6

let getDrawResultNumbers =
    let successHandler = function 6 -> ExtraNumber | _ -> DrawnNumber
    addUniqueToList MaxDrawResultCount successHandler

let getUsersDrawNumber =
    let successHandler _ = id
    addUniqueToList MaxUsersDrawCount successHandler

let checkResults errorHandler drawResults usersDraw =
    let allElementsAreUnique (drawResults: 'T list) =
        let set = HashSet(drawResults)
        if set.Count = drawResults.Length
        then Success drawResults
        else "There are duplicates in draw result list" |> ValidationResult.errorFromString

    let validateUsersDraw =
        let validateUsersDrawLength input = 
            if List.length input = 6
            then Success input
            else 
                "MarkSixNumber list has incorrect length"
                |> ValidationResult.errorFromString

        allElementsAreUnique
        >=> validateUsersDrawLength

    let validateDrawResults =
        let splitDrawResults drawResults =
            match List.rev drawResults with
            | h :: t -> ValidationResult.success (h, t)
            | [] -> "Draw result list is empty" |> ValidationResult.errorFromString

        let validateOneExtraNumbersWithSixDrawnumbers (extraNumber, drawnNumbers) =
            let drawnNumbersAreAllDrawnNumbers = List.choose (function DrawnNumber x -> Some x | _ -> None) drawnNumbers
            let extraNumberIsExtraNumber, extraNumber = (function ExtraNumber x -> true, x | DrawnNumber y -> false, y) extraNumber

            if List.length drawnNumbersAreAllDrawnNumbers = 6 && extraNumberIsExtraNumber
            then Success (extraNumber, drawnNumbersAreAllDrawnNumbers)
            else "There should be exactly six drawn numbers and one extra number" |> ValidationResult.errorFromString

        allElementsAreUnique
        >=> splitDrawResults
        >=> validateOneExtraNumbersWithSixDrawnumbers

    let drawResultsValidated, usersDrawValidated =
        drawResults |> validateDrawResults,
        usersDraw |> validateUsersDraw

    match usersDrawValidated, drawResultsValidated with
    | Error e, Success _ | Success _, Error e -> 
        errorHandler e
        Error e
    | Error e1, Error e2 ->
        errorHandler e1
        errorHandler e2
        let (ErrorMessage m1, ErrorMessage m2) = e1, e2
        m1 + m2 |> ErrorMessage |> Error
    | Success usersDraw, Success (extraNumber, drawResultWithoutExtraNumber) -> 
        let points = 
            (Set.ofList usersDraw, Set.ofList drawResultWithoutExtraNumber)
            ||> Set.intersect
            |> Set.count
            |> (decimal >> Points)

        let extraPoints = 
            match List.tryFind ((=) extraNumber) usersDraw with
            | Some _ -> Points 0.5m
            | None -> Points 0.m

        points .+. extraPoints |> Prize.fromPoints |> ValidationResult.success
