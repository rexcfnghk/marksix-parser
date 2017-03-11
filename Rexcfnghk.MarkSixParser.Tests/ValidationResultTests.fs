module Rexcfnghk.MarkSixParser.Tests.ValidationResultTests

open FsCheck.Xunit
open Xunit
open Swensen.Unquote
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.Result

[<Fact>]
let ``retryable should be able to succeed eventually`` () =
    let mutable initial = 1
    let tester i =
        if i > 1
        then ok i
        else errorFromString "Fail"

    let result = retryable ignore <| fun () ->
        let attempt = tester initial
        initial <- initial + 1
        attempt

    result >! 1

[<Property>]
let ``map identity gives back the original ValidationResult`` (intV: Result<int, ErrorMessage>) =
    map id intV =! intV

[<Property>]
let ``ErrorMessage can be wrapped and unwrapped`` s =
    let error = s |> errorFromString
    test <@ match error with Ok _ -> false | Error (ErrorMessage msg) -> msg = s @>

[<Property>]
let ``return and apply is the same as calling map`` (f: int -> int) x =
    ok f <*> x =! map f x

[<Property>]
let ``All elements in list must be return success before traverse is successful`` (f: int -> Result<string, ErrorMessage>) list =
    let allSuccess = function Ok _ -> true | Error _ -> false

    let result = traverse f list
    let isListAllSuccess = List.map f list |> List.forall allSuccess

    test <@ match result with Ok _ -> isListAllSuccess | Error _ -> not isListAllSuccess @>
