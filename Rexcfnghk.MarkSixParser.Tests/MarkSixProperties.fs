module Rexcfnghk.MarkSixParser.Tests.MarkSixProperties

open Rexcfnghk.MarkSixParser
open Models
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote
open System

let markSixNumberGen =
    Gen.elements [1..49]
    |> Gen.map (MarkSixNumber.create >> Result.extract)

let markSixNumberArb = Arb.fromGen markSixNumberGen

let markSixNumberSetGen count =
    markSixNumberGen
    |> Gen.listOfLength count
    |> Gen.map Set.ofList
    |> Gen.filter (fun s -> Set.count s >= count)
    |> Gen.map (Seq.take count >> Set.ofSeq)

let usersDrawArb =
    markSixNumberSetGen 6
    |> Gen.map (MarkSix.toUsersDraw >> Result.extract)
    |> Arb.fromGen

let drawResultsArb =
    markSixNumberSetGen 7
    |> Gen.map (Set.toList >> function [] -> failwith "unexpected" | h :: t -> Set.ofList t, h)
    |> Arb.fromGen

let invalidLengthUsersDrawArb =
    markSixNumberGen
    |> Gen.listOf
    |> Gen.filter (fun l -> List.length l <> 6)
    |> Gen.map Set.ofList
    |> Gen.filter (fun s -> Set.count s < 6)
    |> Arb.fromGen

let invalidLengthDrawResultsArb =
    markSixNumberGen
    |> Gen.listOf
    |> Gen.filter (fun l -> List.length l >= 2 && List.length l <> 7)
    |> Gen.map Set.ofList
    |> Gen.filter (fun s -> Set.count s >= 2 && Set.count s <> 7)
    |> Gen.map (Set.toList >> function [] -> failwith "unexpected" | h :: t -> Set.ofList t, h)
    |> Arb.fromGen

[<Property>]
let ``drawRandom always returns numbers between 1 and 49`` () =
    let isWithinRange x = x >= 1 && x <= 49
    let r = Random ()
    let (UsersDraw s) = MarkSix.defaultRandomUsersDraw r
    Set.forall (MarkSixNumber.value >> isWithinRange) s

[<Property>]
let ``drawRandom can generate UsersDraw when input count is between six and ten`` () =
    let countArb =
        Arb.generate<NonNegativeInt>
        |> Gen.filter (fun (NonNegativeInt x) -> x >= 6 && x <= 10)
        |> Gen.map (fun (NonNegativeInt x) -> x)
        |> Arb.fromGen

    Prop.forAll countArb <| fun count ->
        let (UsersDraw usersDraw) = MarkSix.randomUsersDraw count (Random ())
        Set.count usersDraw =! count

[<Property>]
let ``toUsersDraw fails when given set length is less than six`` () =
    Prop.forAll invalidLengthUsersDrawArb <| fun l ->
        test <@ match MarkSix.toUsersDraw l with
                | Error _ -> true
                | _ -> false @>

[<Property>]
let ``toDrawResults fails when given list length not equals to seven`` () =
    Prop.forAll invalidLengthDrawResultsArb <| fun drawResults ->
            test <@ match MarkSix.toDrawResults drawResults with
                    | Error _ -> true
                    | _ -> false @>

[<Property(MaxTest = 500)>]
let ``checkResults returns correct Prize for arbitrary drawResults and usersDraw`` () =
    let calculatePoints usersDraw (drawResultWithoutExtraNumber, extraNumber) =
        let points =
            (Set.ofList usersDraw, drawResultWithoutExtraNumber)
            ||> Set.intersect
            |> Set.count
            |> decimal

        let extraPoints =
            match List.tryFind ((=) extraNumber) usersDraw with
            | Some _ -> 0.5m
            | None -> 0.m

        points + extraPoints |> Points

    Prop.forAll drawResultsArb <| fun (drawResultsSet, extraNumber) ->
        Prop.forAll usersDrawArb <| fun usersDraw ->
            let extractedDrawResults =
                (drawResultsSet, extraNumber)
                |> MarkSix.toDrawResults
                |> Result.extract

            let extractedUsersDraw =
                let (UsersDraw s) = usersDraw
                Set.toList s

            let expected =
                calculatePoints extractedUsersDraw (drawResultsSet, extraNumber)
                |> Prize.fromPoints

            let actual =
                MarkSix.checkResults ignore extractedDrawResults usersDraw
                |> Result.extract

            actual =! expected

[<Property>]
let ``checkResults should fail for usersDraw with less than six elements`` () =
    Prop.forAll drawResultsArb <| fun (drawResultsSet, extraNumber) ->
        Prop.forAll invalidLengthUsersDrawArb <| fun usersDrawSet ->
            let drawResults =
                (drawResultsSet, extraNumber)
                |> MarkSix.toDrawResults
                |> Result.extract

            let usersDraw =
                usersDrawSet
                |> UsersDraw

            test <@ match MarkSix.checkResults ignore drawResults usersDraw with
                    | Ok _ -> false
                    | Error _ -> true @>

[<Property>]
let ``checkMultipleUsersDraws should return success for valid usersDrawList`` () =
    Prop.forAll drawResultsArb <| fun (drawResultsSet, extraNumber) ->
        Prop.forAll usersDrawArb <| fun usersDraw ->
            let drawResults =
                (drawResultsSet, extraNumber)
                |> MarkSix.toDrawResults
                |> Result.extract

            test <@ match MarkSix.checkMultipleUsersDraws MarkSix.checkResults ignore drawResults [usersDraw] with
                     | Ok _ -> true
                     | Error _ -> false @>

[<Property>]
let ``checkMultipleUsersDraws should return correct number of combinations for valid usersDrawList`` () =
    let usersDrawArb =
        Gen.choose (6, 10)
        >>= markSixNumberSetGen
        |> Gen.map (MarkSix.toUsersDraw >> Result.extract)
        |> Arb.fromGen

    let factorial =
        let rec factorialImpl acc = function
            | 0 -> acc
            | n -> factorialImpl (acc * n) (n - 1)
        factorialImpl 1

    let expectedNumberOfCombinations arr =
        let n = Array.length arr
        factorial n / (factorial 6 * (factorial (n - 6)))

    Prop.forAll drawResultsArb <| fun (drawResultsSet, extraNumber) ->
        Prop.forAll usersDrawArb <| fun usersDraw ->
            let drawResults =
                (drawResultsSet, extraNumber)
                |> MarkSix.toDrawResults
                |> Result.extract

            let (UsersDraw set) = usersDraw
            let numberOfCombinations = set |> Set.toArray |> expectedNumberOfCombinations

            test <@ match MarkSix.checkMultipleUsersDraws MarkSix.checkResults ignore drawResults [usersDraw] with
                     | Ok x -> List.length x = numberOfCombinations
                     | Error _ -> false @>
