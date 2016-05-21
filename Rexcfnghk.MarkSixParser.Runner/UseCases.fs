module Rexcfnghk.MarkSixParser.Runner.UseCases

open Rexcfnghk.MarkSixParser
open ErrorHandling
open MarkSixNumberReader

let private addOne = (+) 1

let markSixNumberReader _ =
    readMarkSixNumber stdin.ReadLine ()

let getDrawResultNumbers' () =
    printfn "Enter draw results"
    let drawResults = getDrawResultNumbers markSixNumberReader

    printfn "The draw results are %A" drawResults
    drawResults

let getUsersDrawNumbers' () =
    let usersDraw = getUsersDrawNumbers markSixNumberReader
    printfn "User's draw is %A" usersDraw
    usersDraw

let getMultipleUsersDraw' () =
    printfn "Enter users draw(s)"

    let printUsersDrawLength list = 
        list |> (List.length >> printfn "You entered %i user's draw(s)")
        list

    let printUsersDrawElements = List.iteri (addOne >> printfn "User's draw #%i: %A")

    let decisionPrompt () = 
        stdin.ReadLine 
        >> char
        >> Decision.toResult
        |> ValidationResult.retryable defaultErrorHandler

    let usersDraw = 
        getMultipleUsersDraw 
            (fun _ -> getUsersDrawNumbers' ())
            (fun _ -> decisionPrompt ())

    usersDraw
    |> (printUsersDrawLength >> printUsersDrawElements)

    usersDraw

let checkMultipleResults =
    MarkSix.checkResults defaultErrorHandler >> ValidationResult.traverse

let printPrizes = function
    | Success l -> List.iteri (addOne >> printfn "Your prize for draw #%i is %A") l
    | Error e -> printfn "%A" e