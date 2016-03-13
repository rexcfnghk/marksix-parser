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

let getMultipleUsersDraw () =
    let rec getUsersDrawNumbers' decision acc i =
        if decision = "n" || decision = "N"
        then List.rev acc
        else
            printfn "Enter user's #%i draw" (i + 1)
            let usersDraw = MarkSix.getUsersDrawNumber (printfn "%A") tryGetInteger
            printfn "Continue entering user's draw #%i [YyNn]?" (i + 2)
            let decision = Console.ReadLine()
            getUsersDrawNumbers' decision (usersDraw :: acc) (i + 1)

    let printUsersDrawList list =
        printfn "You entered %i user's draw(s)" (List.length list)
        List.iter (printfn "%A") list

    let usersDrawList = getUsersDrawNumbers' "Y" [] 0
    printUsersDrawList usersDrawList
    usersDrawList

let checkMultipleResults drawResults usersDrawList =
    List.map (MarkSix.checkResults (printfn "%A") drawResults) usersDrawList

let printPrizes prizes = 
    List.iteri (printfn "Your prize for draw #%i is %A") prizes