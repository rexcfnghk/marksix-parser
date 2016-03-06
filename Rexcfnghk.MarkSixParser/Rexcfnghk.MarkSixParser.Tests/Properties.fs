module Properties

open System
open Rexcfnghk.MarkSixParser
open Models
open FsCheck
open FsCheck.Xunit
open Swensen.Unquote

let outOfRangeArb =
    Arb.Default.Int32().Generator
    |> Gen.suchThat (fun i -> i < 1 || i > 49)
    |> Arb.fromGen

[<Property>]
let ``drawRandom always returns six elements`` () =
    let numbers = MarkSix.drawNumbers ()
    numbers.Length =! 6

[<Property>]
let ``drawRandom always returns numbers between 1 and 49`` () =
    let isWithinRange x = x >= 1 && x <= 49
    let numbers = MarkSix.drawNumbers ()
    List.forall (MarkSixNumber.value >> isWithinRange) numbers

[<Property>]
let ``addDrawResultNumbers always add ExtraNumber at the end`` () =
    let r = Random()
    let resultListRev = List.rev <| MarkSix.addDrawResultNumbers (fun () -> r.Next(1, 50)) ignore
    match resultListRev with
    | (ExtraNumber _) :: _ -> true
    | _ -> false

[<Property>]
let ``MarkSixNumber.create returns error for integers out of range`` () =
    Prop.forAll outOfRangeArb <| fun x -> 
        match MarkSixNumber.create x with
        | Error e ->
            let (ErrorMessage m) = e
            m = "Input out of range"
        | _ -> false