module Properties

open System
open Models
open Xunit
open FsCheck.Xunit
open Swensen.Unquote

[<Property>]
let ``drawRandom always returns six elements`` () =
    let numbers = MarkSix.drawNumbers ()
    numbers.Length =! 6

[<Property>]
let ``drawRandom always returns numbers between 1 and 49`` () =
    let numbers = MarkSix.drawNumbers ()
    List.forall (fun (ChosenNumber n) -> n > 1 && n < 49) numbers

[<Property>]
let ``addDrawResultNumbers always add ExtraNumber at the end`` () =
    let r = Random()
    let resultListRev = List.rev <| MarkSix.addDrawResultNumbers (fun () -> r.Next(49) + 1)
    match resultListRev with
    | (ExtraNumber _) :: _ -> true
    | _ -> false
