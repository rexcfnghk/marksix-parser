module Rexcfnghk.MarkSixParser.Tests.Properties

open System
open Rexcfnghk.MarkSixParser
open Models
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

let markSixNumberGen =
    Gen.elements [1..49]
    |> Gen.map (MarkSixNumber.create >> ValidationResult.extract)

let usersDrawArb = 
    markSixNumberGen
    |> Gen.listOfLength 6
    |> Gen.map Set.ofList
    |> Gen.suchThat (fun s -> Set.count s >= 6)
    |> Gen.map (Seq.take 6 >> Seq.toList)
    |> Arb.fromGen

let drawResultsArb =
    markSixNumberGen
    |> Gen.listOfLength 7
    |> Gen.map Set.ofList
    |> Gen.suchThat (fun s -> Set.count s >= 7)
    |> Gen.map (Seq.take 7 >> Seq.toList >> List.mapi (fun i -> if i = 6 then ExtraNumber else DrawnNumber))
    |> Arb.fromGen

[<Property>]
let ``drawRandom always returns six elements`` () =
    let numbers = MarkSix.drawNumbers ()
    numbers.Length =! 6

[<Property>]
let ``drawRandom always returns numbers between 1 and 49`` () =
    let isWithinRange x = x >= 1 && x <= 49
    let numbers = MarkSix.drawNumbers ()
    test <@ List.forall (MarkSixNumber.value >> isWithinRange) numbers @>

[<Property>]
let ``getDrawResultNumbers always add ExtraNumber at the end`` () =
    let r = Random()
    let resultListRev = List.rev <| MarkSix.getDrawResultNumbers ignore (fun () -> r.Next(1, 50))
    test <@ match resultListRev with (ExtraNumber _) :: _ -> true | _ -> false @>

[<Property>]
let ``getDrawResultNumbers always returns seven elements`` () =
    let r = Random()
    let result = MarkSix.getDrawResultNumbers ignore (fun () -> r.Next(1, 50))
    result.Length =! 7

[<Property>]
let ``drawResultsArb returns six DrawnNumbers and one ExtraNumber`` () =
    Prop.forAll drawResultsArb <| fun l ->
        let drawnNumbers, extraNumbers = List.partition (function DrawnNumber _ -> true | ExtraNumber _ -> false) l
        test <@ List.length drawnNumbers = 6 && List.length extraNumbers = 1 @>

[<Property>]
let ``checkResults returns correct Prize for arbitrary drawResults and usersDraw`` () =
    let splitDrawResults drawResults =
        let splitter = List.map (function DrawnNumber n | ExtraNumber n -> n) >> List.rev
        match splitter drawResults with
        | [] -> failwith "Should not reach here"
        | h :: t -> t, h

    let calculatePoints usersDraw (drawResultWithoutExtraNumber, extraNumber) =
        let points = 
            (Set.ofList usersDraw, Set.ofList drawResultWithoutExtraNumber)
            ||> Set.intersect
            |> Set.count
            |> decimal

        let extraPoints = 
            match List.tryFind ((=) extraNumber) usersDraw with
            | Some _ -> 0.5m
            | None -> 0.m

        points + extraPoints |> Points
    
    Prop.forAll drawResultsArb <| fun drawResults ->
        Prop.forAll usersDrawArb <| fun usersDraw ->
            let expected = 
                calculatePoints usersDraw (splitDrawResults drawResults)
                |> Prize.fromPoints
 
            let actual =
                MarkSix.checkResults ignore drawResults usersDraw
                |> ValidationResult.extract

            actual =! expected