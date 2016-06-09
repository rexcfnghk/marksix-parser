module Rexcfnghk.MarkSixParser.Runner.StringConversion

open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.ValidationResult
open System

let private tryConvert converter errorMessage s =
    match converter s with
    | true, x -> ValidationResult.success x
    | false, _ -> ValidationResult.errorFromString errorMessage

let tryConvertToChar = tryConvert Char.TryParse "Input is not a valid character"

let tryConvertToUsersDrawCount = 
    let greaterThanOrEqualsToSix x =
         if x >= 6 
         then ValidationResult.success x
         else 
            ValidationResult.errorFromString "User's draw count must be greater than six"

    tryConvert Int32.TryParse "Input is not a valid integer"
    >=> greaterThanOrEqualsToSix
