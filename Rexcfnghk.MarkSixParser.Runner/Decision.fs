module Rexcfnghk.MarkSixParser.Runner.Decision

open System

type T = Yes | No

let (|EqualsIgnoreCase|_|) pattern char = 
    let left, right = sprintf "%c" pattern, sprintf "%c" char
    let equalsIgnoreCase = String.Equals(left, right, StringComparison.CurrentCultureIgnoreCase)
    if equalsIgnoreCase
    then Some ()
    else None

let ofCharOption = function
    | EqualsIgnoreCase 'y' -> Some Yes
    | EqualsIgnoreCase 'n' -> Some No
    | _ -> None