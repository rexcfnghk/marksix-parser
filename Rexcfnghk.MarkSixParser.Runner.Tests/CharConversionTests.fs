module Rexcfnghk.MarkSixParser.Runner.Tests.CharConversionTests

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.Runner.CharConversion

[<Property>]
let ``tryConvertToChar converts successfully for one-character string`` () =
    let validStringArb = 
        ['a'..'z']
        |> Gen.elements
        |> Gen.map string
        |> Arb.fromGen

    Prop.forAll validStringArb <| fun s -> 
        let c = tryConvertToChar s
        test <@ match c with Success _ -> true | Error _ -> false @>

[<Fact>]
let ``tryConvertToChar returns error for code points requiring two UTF-16 code units`` () =
    let input = "𤭢"

    let c = tryConvertToChar input

    test <@ match c with Success _ -> false | Error _ -> true @>
