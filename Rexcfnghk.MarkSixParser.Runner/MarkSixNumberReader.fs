module Rexcfnghk.MarkSixParser.Runner.MarkSixNumberReader

open System
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.ValidationResult
open ErrorHandling

let readMarkSixNumber = 
    let validateInt32 string =
        match Int32.TryParse string with
        | true, i -> ValidationResult.success i
        | false, _ -> "Input is not an integer" |> ValidationResult.errorFromString

    stdin.ReadLine >> validateInt32 >=> MarkSixNumber.create

let rec private getDrawNumbers markSixNumberReader maxCount acc =
    let tryAddToSet acc element =
        let updatedSet = Set.add element acc
        if Set.count updatedSet = Set.count acc
        then DuplicateErrorMessage |> ValidationResult.errorFromString
        else updatedSet |> ValidationResult.success

    let index = Set.count acc

    if index = maxCount
    then acc
    else 
        let readAndTryAddToSet () = 
            index |> markSixNumberReader >>= tryAddToSet acc
        let updatedSet = 
            ValidationResult.retryable defaultErrorHandler readAndTryAddToSet
        getDrawNumbers markSixNumberReader maxCount updatedSet

let getDrawResultNumbers markSixNumberReader postEnterPrompt () = 
    let tryReturnExtraNumber set element =
        if Set.exists ((=) element) set
        then DuplicateErrorMessage |> ValidationResult.errorFromString
        else element |> ValidationResult.success

    let drawnNumbers = getDrawNumbers markSixNumberReader 6 Set.empty
    let extraNumber = 
        readMarkSixNumber
        >=> tryReturnExtraNumber drawnNumbers
        |> ValidationResult.retryable defaultErrorHandler

    let drawResults = 
        (drawnNumbers, extraNumber)
        |> MarkSix.toDrawResults
        |> ValidationResult.extract
    postEnterPrompt drawResults
    drawResults
            
let getMultipleUsersDraw markSixNumberReader preEnterPrompt postEnterPrompt listPrompt () =
    let rec getUsersDrawNumbers' decision acc i =
        if decision = 'n' || decision = 'N'
        then List.rev acc
        else
            let newCount = i + 1
            preEnterPrompt newCount
            let usersDraw = getDrawNumbers markSixNumberReader 6 Set.empty 
            postEnterPrompt (newCount + 1)
            let decision = stdin.ReadLine() |> char
            getUsersDrawNumbers' decision (usersDraw :: acc) newCount

    let usersDrawList = 
        getUsersDrawNumbers' 'Y' [] 0
        |> ValidationResult.traverse MarkSix.toUsersDraw
        |> ValidationResult.extract

    listPrompt usersDrawList
    usersDrawList