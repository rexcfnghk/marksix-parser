module Rexcfnghk.MarkSixParser.Tests.ValidationResultFacts

open Swensen.Unquote
open Xunit
open Rexcfnghk.MarkSixParser

[<Fact>]
let ``retryable should be able to succeed eventually`` () =
    let mutable initial = 1
    let tester i = 
        if i > 1 
        then ValidationResult.success i 
        else ValidationResult.errorFromString "Fail"

    let result = ValidationResult.retryable ignore <| fun () ->
        let attempt = tester initial
        initial <- initial + 1
        attempt

    result >! 1
