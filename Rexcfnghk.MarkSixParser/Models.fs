module Rexcfnghk.MarkSixParser.Models

type DrawnNumber = DrawnNumber of MarkSixNumber.T

type ExtraNumber = ExtraNumber of MarkSixNumber.T

type Points = 
    | Points of decimal
    static member (+) (Points left, Points right) =
        Points (left + right)

type DrawResults = DrawResults of DrawnNumber * DrawnNumber * DrawnNumber *
                                  DrawnNumber * DrawnNumber * DrawnNumber * ExtraNumber

type UsersDraw = UsersDraw of Set<MarkSixNumber.T>
