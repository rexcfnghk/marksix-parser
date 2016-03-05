﻿module MarkSix

open System
open System.Collections.Generic
open Models
open Rexcfnghk.MarkSixParser

let drawNumbers () =
    let r = Random()
    [ for _ in 1..6 -> 
        r.Next(49) + 1 
        |> MarkSixNumber.tryCreate
        |> Option.get ]

let addDrawResultNumbers getNumber errorHandler =
    let createFromGetNumber = () |> (getNumber >> MarkSixNumber.create)

    let rec innerErrorHandler markSixElement e = 
        errorHandler e
        match createFromGetNumber with
        | Success x -> markSixElement x
        | Error e -> innerErrorHandler markSixElement e

    let rec addDrawResultNumbersImpl (acc: HashSet<DrawResultElement>) =
        let count = acc.Count
        if count = 7
        then acc |> Seq.toList
        else
            let typer = if count = 6 then ExtraNumber else DrawnNumber
            let element = 
                createFromGetNumber
                |> ValidationResult.doubleMap typer (innerErrorHandler typer)
                //|> Option.map (if count = 6 then ExtraNumber else DrawnNumber)
            acc.Add element |> ignore
            addDrawResultNumbersImpl acc

    addDrawResultNumbersImpl <| HashSet()