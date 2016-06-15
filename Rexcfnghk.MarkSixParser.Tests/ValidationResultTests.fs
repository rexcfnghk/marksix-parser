module Rexcfnghk.MarkSixParser.Tests.ValidationResultTests

open FsCheck.Xunit
open Xunit
open Swensen.Unquote
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.ValidationResult

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

[<Property>]
let ``return and apply is the same as calling map`` (f: int -> int) x =
    f
    |> ValidationResult.success
    <*> x
    =! ValidationResult.map f x

[<Property>]
let ``All elements in list must be return success before traverse is successful`` (f: int -> ValidationResult<string>) list =
    let allSuccess = function Success _ -> true | Error _ -> false

    let result = ValidationResult.traverse f list
    let isListAllSuccess = List.map f list |> List.forall allSuccess

    test <@ match result with Success _ -> isListAllSuccess | Error _ -> not isListAllSuccess @>
