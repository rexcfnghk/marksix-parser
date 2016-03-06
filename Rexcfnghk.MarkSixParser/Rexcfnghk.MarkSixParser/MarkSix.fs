module Rexcfnghk.MarkSixParser.MarkSix

open System
open System.Collections.Generic
open Models
open ValidationResult

let drawNumbers () =
    let r = Random()
    [ for _ in 1..6 -> 
        r.Next(1, 50) 
        |> MarkSixNumber.tryCreate
        |> Option.get ]

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
    let validateDrawResultsWithoutExtraNumber =
        let getDrawResultsWithoutExtraNumber = function
            | _ :: t -> ValidationResult.success t
            | [] -> "Draw result list is empty" |> ValidationResult.errorFromString

        let validateDrawResultsWithoutExtraNumberLength input =
            if List.length input = 6
            then Success input
            else 
                "Draw result list has incorrect length"
                |> ValidationResult.errorFromString

        let validateDrawResultsWithoutExtraNumberAreAllDrawnNumbers input =
            let allDrawnNumbers = 
                List.forall (function DrawnNumber _ -> true | _ -> false) input
            if allDrawnNumbers
            then Success input
            else 
                "Draw result list contains more than one extra number"
                |> ValidationResult.errorFromString
        
        getDrawResultsWithoutExtraNumber
        >=> validateDrawResultsWithoutExtraNumberLength
        >=> validateDrawResultsWithoutExtraNumberAreAllDrawnNumbers

    let drawResultsWithoutExtraNumber =
        drawResults
        |> List.rev
        |> validateDrawResultsWithoutExtraNumber
    match drawResultsWithoutExtraNumber with
    | Error e -> 
        errorHandler e
        Error e
    | Success s -> 
        let mutable points = 0

        for ud in usersDraw do
            for dr in s do
                if ud = dr
                then points <- points + 1

        Success points
