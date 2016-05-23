module Rexcfnghk.MarkSixParser.Tests.ValidationResultTests

open FsCheck.Xunit
open Xunit
open Swensen.Unquote
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

[<Property>]
let ``map identity gives back the original ValidationResult`` (intV: ValidationResult<int>) =
    ValidationResult.map id intV =! intV

[<Property>]
let ``ErrorMessage can be wrapped and unwrapped`` s =
    let error = s |> ValidationResult.errorFromString
    test <@ match error with Success _ -> false | Error (ErrorMessage msg) -> msg = s @>