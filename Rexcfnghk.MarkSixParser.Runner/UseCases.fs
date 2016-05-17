module Rexcfnghk.MarkSixParser.Runner.UseCases

open Rexcfnghk.MarkSixParser
open ErrorHandling
open MarkSixNumberReader

let getDrawResultNumbers' =
    printfn "Enter draw results"
    getDrawResultNumbers  (printfn "The draw results are %A")

let getMultipleUsersDraw' =
    let printUsersDrawLength list = 
        list |> (List.length >> printfn "You entered %i user's draw(s)")
        list
    let printUsersDrawElements = List.iteri (fun i -> printfn "User's draw #%i: %A" (i + 1))

    getMultipleUsersDraw 
        (printfn "Enter user's #%i draw") 
        (printfn "Continue entering user's draw #%i [YyNn]?")
        (printUsersDrawLength >> printUsersDrawElements)

let checkMultipleResults =
    MarkSix.checkResults defaultErrorHandler >> ValidationResult.traverse

let printPrizes = function
    | Success l -> List.iteri (fun i -> printfn "Your prize for draw #%i is %A" (i + 1)) l
    | Error e -> printfn "%A" e