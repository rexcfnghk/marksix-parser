module Rexcfnghk.MarkSixParser.Tests.Properties

open System
open Rexcfnghk.MarkSixParser
open Models
open Points
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

let outOfRangeArb =
    Arb.Default.Int32().Generator
    |> Gen.suchThat (fun i -> i < 1 || i > 49)
    |> Arb.fromGen

let markSixNumberGen =
    Gen.elements [1..49]
    |> Gen.map (MarkSixNumber.create >> ValidationResult.extract)

let usersDrawArb = 
    markSixNumberGen
    |> Gen.listOfLength 6
    |> Arb.fromGen

let drawResultsArb =
    let drawnNumbersGen =
        markSixNumberGen
        |> Gen.map DrawnNumber
        |> Gen.listOfLength 6

    let extraNumberGen =
        markSixNumberGen
        |> Gen.map ExtraNumber
        |> Gen.listOfLength 1

    List.append <!> drawnNumbersGen <*> extraNumberGen
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
let ``MarkSixNumber.create returns error for integers out of range`` () =
    Prop.forAll outOfRangeArb <| fun x -> 
        test <@ match MarkSixNumber.create x with Error _ -> true | _ -> false @>

[<Property>]
let ``drawResultsArb returns six DrawnNumbers`` () =
    Prop.forAll drawResultsArb <| fun l ->
        let drawnNumbers, _ = List.partition (function DrawnNumber _ -> true | ExtraNumber _ -> false) l
        List.length drawnNumbers =! 6

[<Property>]
let ``drawResultsArb returns one ExtraNumber`` () =
    Prop.forAll drawResultsArb <| fun l ->
        let _, extraNumbers = List.partition (function DrawnNumber _ -> true | ExtraNumber _ -> false) l
        List.length extraNumbers =! 1

[<Property>]
let ``drawResultsArb returns six DrawnNumbers and one ExtraNumber`` () =
    ``drawResultsArb returns six DrawnNumbers`` .&. ``drawResultsArb returns one ExtraNumber``

[<Property>]
let ``MarkSixNumber's custom equality works`` () =
    let markSixNumberArb = markSixNumberGen |> Arb.fromGen
    Prop.forAll markSixNumberArb <| fun m -> 
        m 
        |> MarkSixNumber.value
        |> MarkSixNumber.create
        |> ValidationResult.extract
        =! m

[<Property>]
let ``Points addition has identity`` d1 =
    let p1 = Points d1
    let p2 = Points 0.m
    p1 .+. p2 =! p1

[<Property>]
let ``Poitns addition is communicative`` () =
    let pointsArb = 
        Gen.elements [1.m .. 6.m]
        |> Gen.map Points
        |> Arb.fromGen
    Prop.forAll pointsArb <| fun p1 -> 
        Prop.forAll pointsArb <| fun p2 ->
            p1 .+. p2 =! p2 .+. p1

[<Property>]
let ``Poitns addition is associative`` () =
    let pointsArb = 
        Gen.elements [1.m .. 6.m]
        |> Gen.map Points
        |> Arb.fromGen
    Prop.forAll pointsArb <| fun p1 -> 
        Prop.forAll pointsArb <| fun p2 ->
            Prop.forAll pointsArb <| fun p3 ->
                (p1 .+. p2) .+. p3 =! p1 .+. (p2 .+. p3)