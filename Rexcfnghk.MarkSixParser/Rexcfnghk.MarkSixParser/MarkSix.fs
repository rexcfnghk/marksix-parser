module Rexcfnghk.MarkSixParser.MarkSix

open System
open System.Collections.Generic
open Models
open ValidationResult

let drawNumbers () =
    let r = Random()
    [ for _ in 1..6 -> 
        r.Next(1, 50) 
        |> MarkSixNumber.create
        |> ValidationResult.extract ]

let getDrawResultNumbers getNumber errorHandler =
    let createFromGetNumber = getNumber >> MarkSixNumber.create

    let rec innerErrorHandler markSixElement e = 
        errorHandler e
        match createFromGetNumber () with
        | Success x -> markSixElement x
        | Error e -> innerErrorHandler markSixElement e

    // FSharpSet requires comparison, which is not necessary in this case
    let rec getDrawResultNumbersImpl (acc: HashSet<DrawResultElement>) =
        let count = acc.Count
        if count = 7
        then acc |> Seq.toList
        else
            let typer = if count = 6 then ExtraNumber else DrawnNumber
            let element = 
                createFromGetNumber ()
                |> ValidationResult.doubleMap typer (innerErrorHandler typer)
            acc.Add element |> ignore
            getDrawResultNumbersImpl acc

    getDrawResultNumbersImpl <| HashSet()

let checkResults errorHandler drawResults usersDraw =
    let validateSixMarkSixNumbers input =
        if List.length input = 6
        then Success input
        else 
            "MarkSixNumber list has incorrect length"
            |> ValidationResult.errorFromString

    let validateDrawResultsWithoutExtraNumber =
        let splitDrawResults drawResults =
            match List.rev drawResults with
            | h :: t -> ValidationResult.success (h, t)
            | [] -> "Draw result list is empty" |> ValidationResult.errorFromString

        let validateOneExtraNumbersWithSixDrawnumbers (extraNumber, drawnNumbers) =
            let drawnNumbersAreAllDrawnNumbers = List.forall (function DrawnNumber _ -> true | _ -> false) drawnNumbers
            let extraNumberIsExtraNumber = (function ExtraNumber _ -> true | _ -> false) extraNumber

            if List.length drawnNumbers = 6 && drawnNumbersAreAllDrawnNumbers && extraNumberIsExtraNumber
            then Success (extraNumber, drawnNumbers)
            else "There should be exactly six drawn numbers and one extra number" |> ValidationResult.errorFromString

        splitDrawResults
        >=> validateOneExtraNumbersWithSixDrawnumbers

    let drawResultsValidated =
        drawResults
        |> validateDrawResultsWithoutExtraNumber

    let usersDrawValidated = usersDraw |> validateSixMarkSixNumbers

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
        Set.intersect (Set.ofList usersDraw) (Set.ofList drawResultWithoutExtraNumber)
        |> Set.count
        |> ValidationResult.success
