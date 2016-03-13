module UseCases

open System
open Rexcfnghk.MarkSixParser

let tryGetInteger () = 
    let validateInt32 string =
        match Int32.TryParse string with
        | true, i -> ValidationResult.success i
        | false, _ -> "Input is not an integer" |> ValidationResult.errorFromString

    let rec retryableErrorHandler getResult errorHandler =
        match getResult () with
        | Success i -> i
        | Error e -> 
            errorHandler e
            retryableErrorHandler getResult errorHandler

    let getInt32 = Console.ReadLine >> validateInt32

    retryableErrorHandler getInt32 (printfn "%A")

let getDrawResultNumbers' () = MarkSix.getDrawResultNumbers (printfn "%A") tryGetInteger

let getUsersDrawNumbers' () = MarkSix.getUsersDrawNumber (printfn "%A") tryGetInteger

let checkResults' = MarkSix.checkResults (printfn "%A")

