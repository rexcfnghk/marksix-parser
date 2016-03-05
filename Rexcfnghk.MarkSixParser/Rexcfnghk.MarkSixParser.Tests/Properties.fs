module Properties

open System
open Models
open FsCheck.Xunit
open Swensen.Unquote

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
    let resultListRev = List.rev <| MarkSix.addDrawResultNumbers (fun () -> r.Next(49) + 1) ignore
    match resultListRev with
    | (ExtraNumber _) :: _ -> true
    | _ -> false
