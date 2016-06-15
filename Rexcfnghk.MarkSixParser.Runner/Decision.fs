module Rexcfnghk.MarkSixParser.Runner.Decision

open System
open Rexcfnghk.MarkSixParser

type T = Yes | No

let (|EqualsIgnoreCase|_|) pattern char =
    let left, right = sprintf "%c" pattern, sprintf "%c" char
    let equalsIgnoreCase = String.Equals(left, right, StringComparison.CurrentCultureIgnoreCase)
    if equalsIgnoreCase
    then Some ()
    else None

let toResult = function
    | EqualsIgnoreCase 'y' -> Success Yes
    | EqualsIgnoreCase 'n' -> Success No
    | _ -> "Unrecognised decision" |> ValidationResult.errorFromString
