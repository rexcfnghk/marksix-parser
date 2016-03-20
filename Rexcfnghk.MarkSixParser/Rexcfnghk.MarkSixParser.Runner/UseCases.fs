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
            let newCount = i + 1
            printfn "Enter user's #%i draw" newCount
            let usersDraw = MarkSix.getUsersDrawNumber (printfn "%A") tryGetInteger
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

let checkMultipleResults drawResults =
    List.map (MarkSix.checkResults (printfn "%A") drawResults)

let printPrizes<'a> : 'a list -> unit = 
    List.iteri (fun i -> printfn "Your prize for draw #%i is %A" (i + 1))