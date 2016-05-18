module Rexcfnghk.MarkSixParser.Runner.UseCases

open Rexcfnghk.MarkSixParser
open ErrorHandling
open MarkSixNumberReader

let markSixNumberReader _ =
    readMarkSixNumber ()

let getDrawResultNumbers' () =
    printfn "Enter draw results"
    getDrawResultNumbers markSixNumberReader (printfn "The draw results are %A")

let getUsersDrawNumbers' () =
    getUsersDrawNumbers markSixNumberReader (printfn "User's draw is %A")

let getMultipleUsersDraw' () =
    printfn "Enter users draw(s)"
    let addOne = (+) 1

    let printUsersDrawLength list = 
        list |> (List.length >> printfn "You entered %i user's draw(s)")
        list

    let printUsersDrawElements = List.iteri (addOne >> printfn "User's draw #%i: %A")

    getMultipleUsersDraw 
        (fun _ -> getUsersDrawNumbers' ())
        (printUsersDrawLength >> printUsersDrawElements)
        (stdin.ReadLine >> char)

let checkMultipleResults =
    MarkSix.checkResults defaultErrorHandler >> ValidationResult.traverse

let printPrizes = function
    | Success l -> List.iteri (fun i -> printfn "Your prize for draw #%i is %A" (i + 1)) l
    | Error e -> printfn "%A" e