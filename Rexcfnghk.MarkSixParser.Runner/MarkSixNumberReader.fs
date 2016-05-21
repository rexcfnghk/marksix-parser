module Rexcfnghk.MarkSixParser.Runner.MarkSixNumberReader

open System
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.ValidationResult
open Rexcfnghk.MarkSixParser.Runner.Decision
open ErrorHandling

let readMarkSixNumber reader = 
    let validateInt32 string =
        match Int32.TryParse string with
        | true, i -> ValidationResult.success i
        | false, _ -> "Input is not an integer" |> ValidationResult.errorFromString

    reader >> validateInt32 >=> MarkSixNumber.create

let rec private getDrawNumbers maxCount acc markSixNumberReader  =
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
        getDrawNumbers maxCount updatedSet markSixNumberReader 

let getSixMarkSixNumbers =
    getDrawNumbers 6 Set.empty 

let getDrawResultNumbers markSixNumberReader = 
    let tryReturnExtraNumber set element =
        if Set.exists ((=) element) set
        then DuplicateErrorMessage |> ValidationResult.errorFromString
        else element |> ValidationResult.success

    let drawnNumbers = getSixMarkSixNumbers markSixNumberReader
    let extraNumber = 
        let index = 6
        (fun () -> markSixNumberReader index)
        >=> tryReturnExtraNumber drawnNumbers
        |> ValidationResult.retryable defaultErrorHandler

    (drawnNumbers, extraNumber)
    |> MarkSix.toDrawResults
    |> ValidationResult.extract

let getUsersDrawNumbers markSixNumberReader postEnterPrompt =
    let usersDraw =
        getSixMarkSixNumbers markSixNumberReader
        |> MarkSix.toUsersDraw
        |> ValidationResult.extract
    postEnterPrompt usersDraw
    usersDraw
            
let getMultipleUsersDraw getSingleUsersDraw listPrompt decisionPrompt =
    let generator (index, decision) =
        if decision = Yes
        then
            let newIndex = index + 1 
            Some (getSingleUsersDraw index, (newIndex, decisionPrompt newIndex))
        else
            None

    let usersDrawList = 
        List.unfold generator (0, Yes)

    listPrompt usersDrawList
    usersDrawList