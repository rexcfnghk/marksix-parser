module Rexcfnghk.MarkSixParser.Runner.UseCases

open System
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.ValidationResult
open Rexcfnghk.MarkSixParser.MarkSix

let readMarkSixNumber = 
    let validateInt32 string =
        match Int32.TryParse string with
        | true, i -> ValidationResult.success i
        | false, _ -> "Input is not an integer" |> ValidationResult.errorFromString

    stdin.ReadLine >> validateInt32 >=> MarkSixNumber.create

// TODO: Cannot use Set in initial entry because of positioning
// Add to set to check for duplicates
let getDrawResultNumbers' () = 
    let duplicateErrorMessage = 
        "Duplicate mark six number entered"

    let tryAddToSet acc element =
        let updatedSet = Set.add element acc
        if Set.count updatedSet = Set.count acc
        then duplicateErrorMessage |> ValidationResult.errorFromString
        else updatedSet |> ValidationResult.success

    let tryReturnExtraNumber set element =
        if Set.exists ((=) element) set
        then duplicateErrorMessage |> ValidationResult.errorFromString
        else element |> ValidationResult.success

    let getDrawResultNumbersImpl acc =
        if Set.count acc = 6
        then acc
        else 
            let addMarkSixNumberToSet = readMarkSixNumber >=> tryAddToSet acc
            ValidationResult.retryable (printfn "%A") addMarkSixNumberToSet

    let drawnNumbers = getDrawResultNumbersImpl Set.empty
    let extraNumber = 
        readMarkSixNumber
        >=> tryReturnExtraNumber drawnNumbers
        |> ValidationResult.retryable (printfn "%A")

    (drawnNumbers, extraNumber)
    |> MarkSix.toDrawResults
    |> ValidationResult.extract
            
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