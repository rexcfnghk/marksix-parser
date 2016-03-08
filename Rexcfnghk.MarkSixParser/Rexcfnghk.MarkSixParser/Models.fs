module Rexcfnghk.MarkSixParser.Models

[<StructuredFormatDisplay("{AsString}")>]
type DrawResultElement =
    | DrawnNumber of MarkSixNumber.T
    | ExtraNumber of MarkSixNumber.T
    override this.ToString() =
        match this with
        | DrawnNumber n -> sprintf "DrawnNumber %A" n
        | ExtraNumber n -> sprintf "ExtraNumber %A" n
    member this.AsString = this.ToString()

type Points = Points of decimal