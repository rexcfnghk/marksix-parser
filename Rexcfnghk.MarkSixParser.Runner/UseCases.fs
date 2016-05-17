module Rexcfnghk.MarkSixParser.Runner.UseCases

open Rexcfnghk.MarkSixParser
open ErrorHandling

let checkMultipleResults =
    MarkSix.checkResults defaultErrorHandler >> ValidationResult.traverse

let printPrizes = function
    | Success l -> List.iteri (fun i -> printfn "Your prize for draw #%i is %A" (i + 1)) l
    | Error e -> printfn "%A" e