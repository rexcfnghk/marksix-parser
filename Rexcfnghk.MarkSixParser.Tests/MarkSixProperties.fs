module Rexcfnghk.MarkSixParser.Tests.MarkSixProperties

open Rexcfnghk.MarkSixParser
open Models
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

let markSixNumberGen =
    Gen.elements [1..49]
    |> Gen.map (MarkSixNumber.create >> ValidationResult.extract)

let markSixNumberArb = Arb.fromGen markSixNumberGen

let markSixNumberSetGen count =
    markSixNumberGen
    |> Gen.listOfLength count
    |> Gen.map Set.ofList
    |> Gen.suchThat (fun s -> Set.count s >= count)
    |> Gen.map (Seq.take count >> Set.ofSeq)

let usersDrawArb = 
    markSixNumberSetGen 6
    |> Gen.map (MarkSix.toUsersDraw >> ValidationResult.extract)
    |> Arb.fromGen

let drawResultsArb =
    markSixNumberSetGen 7
    |> Gen.map (Set.toList >> function [] -> failwith "unexpected" | h :: t -> Set.ofList t, h)
    |> Arb.fromGen

let invalidLengthUsersDrawArb =
    Arb.Default.NonNegativeInt().Generator
    |> Gen.suchThat (fun x -> x.Get <> 0 && x.Get <> 6)
    |> Gen.map (fun (NonNegativeInt x) -> x)
    >>= (fun x -> Gen.listOfLength x markSixNumberGen)
    |> Gen.map Set.ofList
    |> Arb.fromGen

let invalidLengthDrawResultsArb =
    markSixNumberGen
    |> Gen.listOf
    |> Gen.suchThat (fun l -> List.length l >= 2 && List.length l <> 7)
    |> Gen.map Set.ofList
    |> Gen.suchThat (fun s -> Set.count s >= 2 && Set.count s <> 7)
    |> Gen.map (Set.toList >> function [] -> failwith "unexpected" | h :: t -> Set.ofList t, h)
    |> Arb.fromGen

[<Property>]
let ``drawRandom always returns numbers between 1 and 49`` () =
    let isWithinRange x = x >= 1 && x <= 49
    let (UsersDraw (m1, m2, m3, m4, m5, m6)) = MarkSix.randomUsersDraw ()
    let numbers = [m1; m2; m3; m4; m5; m6]
    test <@ List.forall (MarkSixNumber.value >> isWithinRange) numbers @>

[<Property>]
let ``toUsersDraw fails when given set length not equals to six`` () =
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
                |> ValidationResult.extract

            let extractedUsersDraw =
                let (UsersDraw (n1, n2, n3, n4, n5, n6)) = usersDraw
                [n1; n2; n3; n4; n5; n6]

            let expected = 
                calculatePoints extractedUsersDraw (drawResultsSet, extraNumber)
                |> Prize.fromPoints
 
            let actual =
                MarkSix.checkResults ignore extractedDrawResults usersDraw
                |> ValidationResult.extract

            actual =! expected