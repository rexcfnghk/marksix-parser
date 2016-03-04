module MarkSixNumber

open Rexcfnghk.MarkSixParser

type T = MarkSixNumber of int

let create input = 
    if input > 1 && input <= 49 then
        input
        |> (MarkSixNumber >> ValidationResult.success)
    else 
        "Input out of range"
        |> (ErrorMessage >> ValidationResult.error)

let tryCreate = create >> ValidationResult.extract

let value (MarkSixNumber num) = num