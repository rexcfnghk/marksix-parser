module Rexcfnghk.MarkSixParser.Runner.Tests.DecisionProperties

open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open Rexcfnghk.MarkSixParser.Runner.Decision

let validCharPatternArb =
    Gen.elements ['Y'; 'y'; 'N'; 'n']
    |> Arb.fromGen

let invalidCharPatternArb =
    Arb.generate<char>
    |> Gen.suchThat (fun c -> not <| List.contains c ['Y'; 'y'; 'N'; 'n'])
    |> Arb.fromGen

[<Property>]
let ``valid char patterns returns some decision`` () =
    Prop.forAll validCharPatternArb <| fun c ->
        let result = ofCharOption c
        test <@ match result with Some _ -> true | None -> false @>

[<Property>]
let ``invalid char patterns returns none`` () =
    Prop.forAll invalidCharPatternArb <| fun c ->
        let result = ofCharOption c
        test <@ match result with Some _ -> false | None -> true @>