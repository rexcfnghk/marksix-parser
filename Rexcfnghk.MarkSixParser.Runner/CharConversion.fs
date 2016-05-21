module Rexcfnghk.MarkSixParser.Runner.CharConversion

open Rexcfnghk.MarkSixParser
open System
open System.Globalization

let tryConvertToChar s =
    try
        s
        |> StringInfo.GetNextTextElement
        |> char
        |> ValidationResult.success
    with
        | :? FormatException ->
            "Input is not a valid character" 
            |> ValidationResult.errorFromString