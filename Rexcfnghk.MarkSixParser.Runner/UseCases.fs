module Rexcfnghk.MarkSixParser.Runner.UseCases

open Rexcfnghk.MarkSixParser
open ErrorHandling
open MarkSixNumberReader

let getDrawResultNumbers' =
    getDrawResultNumbers (printfn "Enter draw results") (printfn "The draw results are %A")

let getMultipleUsersDraw' =
    getMultipleUsersDraw (printfn "Enter user's #%i draw") (printfn "Continue entering user's draw #%i [YyNn]?")

let checkMultipleResults =
    MarkSix.checkResults defaultErrorHandler >> ValidationResult.traverse

let printPrizes = function
    | Success l -> List.iteri (fun i -> printfn "Your prize for draw #%i is %A" (i + 1)) l
    | Error e -> printfn "%A" e