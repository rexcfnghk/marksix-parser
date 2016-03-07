module Rexcfnghk.MarkSixParser.Models

type DrawResultElement =
    | DrawnNumber of MarkSixNumber.T
    | ExtraNumber of MarkSixNumber.T

[<Measure>] type hkd

type Prize =
    | First
    | Second
    | Third
    | Normal of decimal<hkd>
    | NoPrize

type Points = Points of decimal