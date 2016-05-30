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
        Arb.generate<char>
        |> Gen.map string
        |> Arb.fromGen

    Prop.forAll validStringArb <| fun s -> 
        let cV = tryConvertToChar s
        test <@ match cV with Success _ -> true | Error _ -> false @>

[<Property>]
let ``tryConvertToChar returns the same char from the one-character string`` () =
    let validStringArb = 
        Arb.generate<char>
        |> Gen.map string
        |> Arb.fromGen

    Prop.forAll validStringArb <| fun s -> 
        let cV = tryConvertToChar s
        test <@ match cV with Success c -> sprintf "%c" c = s | Error _ -> false @>

[<Property>]
let ``tryConvertToChar returns error for strings with length > 2 `` () =
    let stringLengthLargerThan2Arb =
        Arb.generate<string>
        |> Gen.suchThat (fun s -> String.length s >= 2)
        |> Arb.fromGen

    Prop.forAll stringLengthLargerThan2Arb <| fun s ->
        test <@ match tryConvertToChar s with Success _ -> false | Error _ -> true @>

[<Fact>]
let ``tryConvertToChar returns error for code points requiring two UTF-16 code units`` () =
    let input = "𤭢"

    let c = tryConvertToChar input

    test <@ match c with Success _ -> false | Error _ -> true @>
