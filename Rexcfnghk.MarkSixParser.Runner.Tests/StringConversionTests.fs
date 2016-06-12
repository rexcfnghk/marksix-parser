module Rexcfnghk.MarkSixParser.Runner.Tests.StringConversionTests

open Xunit
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.Runner.StringConversion

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

[<Property>]
let ``tryConvertToUsersDrawCount converts successfully for valid strings`` () =
    let validStringArb =
        Arb.generate<int>
        |> Gen.suchThat (fun x -> x >= 6)
        |> Gen.map (sprintf "%i")
        |> Arb.fromGen

    Prop.forAll validStringArb <| fun s ->
        test <@ match tryConvertToUsersDrawCount s with Success _ -> true | Error _ -> false @>

[<Property>]
let ``tryConvertToUsersDrawCount fails for ints less than six`` () =
    let invalidStringArb =
        Arb.generate<int>
        |> Gen.suchThat (fun x -> x < 6)
        |> Gen.map (sprintf "%i")
        |> Arb.fromGen

    Prop.forAll invalidStringArb <| fun s ->
        test <@ match tryConvertToUsersDrawCount s with Success _ -> false | Error _ -> true @>

[<Fact>]
let ``tryConvertToChar returns error for code points requiring two UTF-16 code units`` () =
    let input = "𤭢"

    let c = tryConvertToChar input

    test <@ match c with Success _ -> false | Error _ -> true @>
