module Rexcfnghk.MarkSixParser.Tests.PrintFProperties

open FsCheck.Xunit
open Rexcfnghk.MarkSixParser

[<Property>]
let ``ValidationResult %O equals to its %A representation in printf`` (v: ValidationResult<int>) =
    sprintf "%A" v = sprintf "%O" v

[<Property>]
let ``Prize %O equals to its %A representation in printf`` (v: Prize.T) =
    sprintf "%A" v = sprintf "%O" v