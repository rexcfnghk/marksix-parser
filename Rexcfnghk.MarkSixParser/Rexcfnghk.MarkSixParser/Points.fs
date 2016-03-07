module Rexcfnghk.MarkSixParser.Points

open Models

let add p1 p2 =
    let (Points d1, Points d2) = p1, p2
    Points <| d1 + d2

let (.+.) = add