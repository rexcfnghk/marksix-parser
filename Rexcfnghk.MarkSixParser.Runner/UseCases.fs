﻿module Rexcfnghk.MarkSixParser.Runner.UseCases

open System
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.ValidationResult

let readMarkSixNumber () = 
    let validateInt32 string =
        match Int32.TryParse string with
        | true, i -> ValidationResult.success i
        | false, _ -> "Input is not an integer" |> ValidationResult.errorFromString

    let createMarkSixNumberFromReadLine = stdin.ReadLine >> validateInt32 >=> MarkSixNumber.create
    ValidationResult.retryable (printfn "%A") createMarkSixNumberFromReadLine

let getDrawResultNumbers' () = 
    let rec getDrawResultNumbersImpl acc =
        if Set.count acc = 7
        then acc
        else 
            let markSixNumber = readMarkSixNumber ()
            getDrawResultNumbersImpl (Set.add markSixNumber acc)

    getDrawResultNumbersImpl Set.empty
            
let getMultipleUsersDraw () =
    let rec getUsersDrawNumbers' decision acc i =
        if decision = "n" || decision = "N"
        then List.rev acc
        else
            let newCount = i + 1
            printfn "Enter user's #%i draw" newCount
            let usersDraw = MarkSix.getUsersDrawNumber (printfn "%A") readMarkSixNumber
            printfn "Continue entering user's draw #%i [YyNn]?" (newCount + 1)
            let decision = stdin.ReadLine()
            getUsersDrawNumbers' decision (usersDraw :: acc) newCount

    let printUsersDrawList =
        let rec printUsersDrawListImpl acc = function
            | [] -> printfn "You entered %i user's draw(s)" acc
            | h :: t -> 
                let newCount = acc + 1
                printfn "User's draw #%i: %A" newCount h
                printUsersDrawListImpl newCount t
        printUsersDrawListImpl 0

    let usersDrawList = getUsersDrawNumbers' "Y" [] 0
    printUsersDrawList usersDrawList
    usersDrawList

let checkMultipleResults =
    MarkSix.checkResults (printfn "%A") >> ValidationResult.traverse

let printPrizes = function
    | Success l -> List.iteri (fun i -> printfn "Your prize for draw #%i is %A" (i + 1)) l
    | Error e -> printfn "%A" e