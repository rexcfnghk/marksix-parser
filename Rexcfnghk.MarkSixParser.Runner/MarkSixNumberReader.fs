module Rexcfnghk.MarkSixParser.Runner.MarkSixNumberReader

open System
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.Result
open Rexcfnghk.MarkSixParser.Runner.Decision
open ErrorHandling

let readMarkSixNumber reader =
    let validateInt32 string =
        match Int32.TryParse string with
        | true, i -> ok i
        | false, _ -> "Input is not an integer" |> errorFromString

    reader >> validateInt32 >=> MarkSixNumber.create

let rec private getDrawNumbers maxCount acc markSixNumberReader  =
    let tryAddToSet acc element =
        let updatedSet = Set.add element acc
        if Set.count updatedSet = Set.count acc
        then DuplicateErrorMessage |> errorFromString
        else updatedSet |> ok

    let index = Set.count acc

    if index = maxCount
    then acc
    else
        let readAndTryAddToSet () =
            index |> markSixNumberReader >>= tryAddToSet acc
        let updatedSet =
            retryable defaultErrorHandler readAndTryAddToSet
        getDrawNumbers maxCount updatedSet markSixNumberReader

let getDrawResultNumbers markSixNumberReader =
    let tryReturnExtraNumber set element =
        if Set.exists ((=) element) set
        then DuplicateErrorMessage |> errorFromString
        else element |> ok

    let drawnNumbers = getDrawNumbers 6 Set.empty markSixNumberReader
    let extraNumber =
        let index = 6
        (fun () -> markSixNumberReader index)
        >=> tryReturnExtraNumber drawnNumbers
        |> retryable defaultErrorHandler

    (drawnNumbers, extraNumber)
    |> MarkSix.toDrawResults
    |> extract

let getUsersDrawNumbers n =
    getDrawNumbers n Set.empty
    >> MarkSix.toUsersDraw
    >> extract

let getMultipleUsersDraw getSingleUsersDraw decisionPrompt =
    let generator (index, decision) =
        if decision = Yes
        then
            let newIndex = index + 1
            Some (getSingleUsersDraw index, (newIndex, decisionPrompt newIndex))
        else
            None

    List.unfold generator (0, Yes)
