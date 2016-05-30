module Rexcfnghk.MarkSixParser.Runner.CharConversion

open Rexcfnghk.MarkSixParser
open System

let tryConvertToChar s =
    match Char.TryParse s with
    | true, c -> ValidationResult.success c
    | false, _ -> ValidationResult.errorFromString "Input is not a valid character"