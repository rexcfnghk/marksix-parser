module Rexcfnghk.MarkSixParser.Runner.CharConversion

open Rexcfnghk.MarkSixParser
open System

let tryConvertToChar =
    Char.TryParse
    >> function 
        | true, c -> ValidationResult.success c
        | false, _ -> ValidationResult.errorFromString "Input is not a valid character"