module Rexcfnghk.MarkSixParser.Models

type DrawResultElement =
    | DrawnNumber of MarkSixNumber.T
    | ExtraNumber of MarkSixNumber.T

type Points = Points of decimal