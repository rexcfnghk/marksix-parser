module Rexcfnghk.MarkSixParser.Models

type DrawnNumber = DrawnNumber of MarkSixNumber.T

type ExtraNumber = ExtraNumber of MarkSixNumber.T

type Points = Points of decimal

type DrawResults = DrawResults of DrawnNumber * DrawnNumber * DrawnNumber * 
                                       DrawnNumber * DrawnNumber * DrawnNumber * ExtraNumber

type UsersDraw = UsersDraw of MarkSixNumber.T * MarkSixNumber.T * MarkSixNumber.T * 
                             MarkSixNumber.T * MarkSixNumber.T * MarkSixNumber.T