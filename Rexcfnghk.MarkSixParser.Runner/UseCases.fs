module Rexcfnghk.MarkSixParser.Runner.UseCases

open System
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.ValidationResult
open Rexcfnghk.MarkSixParser.MarkSix

[<Literal>]
let DuplicateErrorMessage = 
    "Duplicate mark six number entered"

let defaultErrorHandler x = printfn "%A" x

let readMarkSixNumber = 
    let validateInt32 string =
        match Int32.TryParse string with
        | true, i -> ValidationResult.success i
        | false, _ -> "Input is not an integer" |> ValidationResult.errorFromString

    stdin.ReadLine >> validateInt32 >=> MarkSixNumber.create

let rec getDrawNumbers maxCount acc =
    let tryAddToSet acc element =
        let updatedSet = Set.add element acc
        if Set.count updatedSet = Set.count acc
        then DuplicateErrorMessage |> ValidationResult.errorFromString
        else updatedSet |> ValidationResult.success

    if Set.count acc = maxCount
    then acc
    else 
        let readAndTryAddToSet = readMarkSixNumber >=> tryAddToSet acc
        let updatedSet = ValidationResult.retryable defaultErrorHandler readAndTryAddToSet
        getDrawNumbers maxCount updatedSet

let getDrawResultNumbers' () = 
    let tryReturnExtraNumber set element =
        if Set.exists ((=) element) set
        then DuplicateErrorMessage |> ValidationResult.errorFromString
        else element |> ValidationResult.success

    let drawnNumbers = getDrawNumbers 6 Set.empty
    let extraNumber = 
        readMarkSixNumber
        >=> tryReturnExtraNumber drawnNumbers
        |> ValidationResult.retryable defaultErrorHandler

    (drawnNumbers, extraNumber)
    |> MarkSix.toDrawResults
    |> ValidationResult.extract
            
let getMultipleUsersDraw () =
    let rec getUsersDrawNumbers' decision acc i =
        if decision = 'n' || decision = 'N'
        then List.rev acc
        else
            let newCount = i + 1
            printfn "Enter user's #%i draw" newCount
            let usersDraw = getDrawNumbers 6 Set.empty 
            printfn "Continue entering user's draw #%i [YyNn]?" (newCount + 1)
            let decision = stdin.ReadLine() |> char
            getUsersDrawNumbers' decision (usersDraw :: acc) newCount

    let printUsersDrawList =
        let rec printUsersDrawListImpl acc = function
            | [] -> printfn "You entered %i user's draw(s)" acc
            | h :: t -> 
                let newCount = acc + 1
                printfn "User's draw #%i: %A" newCount h
                printUsersDrawListImpl newCount t
        printUsersDrawListImpl 0

    let usersDrawList = 
        getUsersDrawNumbers' 'Y' [] 0
        |> ValidationResult.traverse MarkSix.toUsersDraw
        |> ValidationResult.extract

    printUsersDrawList usersDrawList
    usersDrawList

let checkMultipleResults =
    MarkSix.checkResults defaultErrorHandler >> ValidationResult.traverse

let printPrizes = function
    | Success l -> List.iteri (fun i -> printfn "Your prize for draw #%i is %A" (i + 1)) l
    | Error e -> printfn "%A" e