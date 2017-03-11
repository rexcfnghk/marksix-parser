module Rexcfnghk.MarkSixParser.Runner.UseCases

open Rexcfnghk.MarkSixParser
open StringConversion
open ErrorHandling
open MarkSixNumberReader
open Result
open Combinations
open Models

let private addOne = (+) 1

let markSixNumberReader _ =
    readMarkSixNumber stdin.ReadLine ()

let getUsersDrawCount () =
    stdin.ReadLine
    >> tryConvertToUsersDrawCount
    |> retryable defaultErrorHandler

let getDrawResultNumbers' () =
    printfn "Enter draw results"
    let drawResults = getDrawResultNumbers markSixNumberReader

    printfn "The draw results are %A\n" drawResults
    drawResults

let getUsersDrawNumbers' () =
    printfn "How many numbers does this user's draw contain?"
    let usersDrawCount = getUsersDrawCount ()

    printfn "Enter a user's draw of %i numbers" usersDrawCount
    let usersDraw = getUsersDrawNumbers usersDrawCount markSixNumberReader

    printfn "User's draw is %A. Continue entering next one? [YyNn]" usersDraw
    usersDraw

let getMultipleUsersDraw' () =
    printfn "Enter users draw(s)"

    let printUsersDrawLength list =
        list |> (List.length >> printfn "You entered %i user's draw(s)")
        list

    let printUsersDrawElements = List.iteri (addOne >> printfn "User's draw #%i: %A")

    let decisionPrompt () =
        stdin.ReadLine
        >> tryConvertToChar
        >=> Decision.toResult
        |> retryable defaultErrorHandler

    let usersDraw =
        getMultipleUsersDraw
            (fun _ -> getUsersDrawNumbers' ())
            (fun _ -> decisionPrompt ())

    usersDraw
    |> (printUsersDrawLength >> printUsersDrawElements)

    printf "\n"
    usersDraw

let checkMultipleUsersDraws' =
    MarkSix.checkMultipleUsersDraws MarkSix.checkResults defaultErrorHandler

let printPrizes = function
    | Ok l -> List.iteri (addOne >> printfn "Your prize for draw #%i is %A") l
    | Error e -> printfn "%A" e
