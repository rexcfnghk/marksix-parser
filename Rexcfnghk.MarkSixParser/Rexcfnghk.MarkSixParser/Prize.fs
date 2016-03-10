module Rexcfnghk.MarkSixParser.Prize

open Models

[<Measure>] type hkd

[<StructuredFormatDisplay("{AsString}")>]
type T =
    | First
    | Second
    | Third
    | Normal of decimal<hkd>
    | NoPrize
    override this.ToString() =
        match this with
        | First -> "First"
        | Second -> "Second"
        | Third -> "Third"
        | Normal p -> sprintf "Fixed prize of HK$%s" <| (decimal p).ToString("#,##0.00")
        | NoPrize -> "No prize"
    member this.AsString = this.ToString()

let fromPoints = function
    | Points 3m -> Normal 40m<hkd>
    | Points 3.5m -> Normal 320m<hkd>
    | Points 4m -> Normal 640m<hkd>
    | Points 4.5m -> Normal 9600m<hkd>
    | Points 5m -> Third
    | Points 5.5m -> Second
    | Points 6m -> First
    | _ -> NoPrize
