module Rexcfnghk.MarkSixParser.Tests.UtilitiesTests

open Rexcfnghk.MarkSixParser.Utilities
open FsCheck.Xunit

[<Property>]
let ``flip is communicative`` (f: int -> int -> int) x y =
    (f |> flip |> flip) x y = f x y
