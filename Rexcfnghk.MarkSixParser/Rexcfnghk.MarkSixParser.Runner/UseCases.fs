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

    let getInt32 = stdin.ReadLine >> validateInt32

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
            let decision = stdin.ReadLine()
            getUsersDrawNumbers' decision (usersDraw :: acc) (i + 1)

    let printUsersDrawList =
        let rec printUsersDrawListImpl acc = function
            | [] -> printfn "You entered %i user's draw(s)" acc
            | h :: t -> 
                printfn "User's draw #%i: %A" acc h
                printUsersDrawListImpl (acc + 1) t
        printUsersDrawListImpl 0

    let usersDrawList = getUsersDrawNumbers' "Y" [] 0
    printUsersDrawList usersDrawList
    usersDrawList

let checkMultipleResults drawResults =
    List.map (MarkSix.checkResults (printfn "%A") drawResults)

let printPrizes<'a> : 'a list -> unit = 
    List.iteri (printfn "Your prize for draw #%i is %A")