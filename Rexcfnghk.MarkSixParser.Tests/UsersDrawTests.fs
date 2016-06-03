module Rexcfnghk.MarkSixParser.Tests.UsersDrawTests

open FsCheck.Xunit
open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.Models
open Swensen.Unquote

[<Property>]
let ``UsersDraw should accept a set of MarkSixNumbers`` (s: Set<MarkSixNumber.T>) =
    let usersDraw = UsersDraw s
    let (UsersDraw set) = usersDraw
    set.GetType() =! typeof<Set<MarkSixNumber.T>>