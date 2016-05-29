module Rexcfnghk.MarkSixParser.Runner.CharConversion

open Rexcfnghk.MarkSixParser
open System
open System.Globalization

let tryConvertToChar =
    StringInfo.GetNextTextElement
    >> Char.TryParse
    >> function 
        | true, c -> ValidationResult.success c
        | false, _ -> ValidationResult.errorFromString "Input is not a valid character"