module Rexcfnghk.MarkSixParser.Tests.Facts

open Rexcfnghk.MarkSixParser
open Swensen.Unquote
open Xunit

[<Fact>]
let ``checkResults returns error when drawResults's length is not seven`` () =
    let drawResults = []

    let validationResult = MarkSix.checkResults ignore drawResults []

    let error = match validationResult with
                | Error _ -> true
                | _ -> false

    test <@ error @>

