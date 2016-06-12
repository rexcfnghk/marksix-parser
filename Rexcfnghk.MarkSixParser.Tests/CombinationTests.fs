module Rexcfnghk.MarkSixParser.Tests.CombinationTests

open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open Rexcfnghk.MarkSixParser.Combinations

[<Property>]
let ``combination should return correct number of combinations for arrays more than six elements and less than ten elements`` () =
    let factorial =
        let rec factorialImpl acc = function
            | 0 -> acc
            | n -> factorialImpl (acc * n) (n - 1)
        factorialImpl 1

    let expectedNumberOfCombinations arr =
        let n = Array.length arr
        factorial n / (factorial 6 * (factorial (n - 6)))

    let arrayArb =
        Arb.generate<int>
        |> Gen.filter (fun x -> x > 0)
        |> Gen.arrayOf
        |> Gen.filter (fun arr -> Array.length arr > 6 && Array.length arr <= 10)
        |> Arb.fromGen

    Prop.forAll arrayArb <| fun arr -> 
        let result = combination 6 arr

        List.length result =! expectedNumberOfCombinations arr