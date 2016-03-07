module Rexcfnghk.MarkSixParser.Prize

open Models

[<Measure>] type hkd

type T =
    | First
    | Second
    | Third
    | Normal of decimal<hkd>
    | NoPrize

let fromPoints = function
    | Points 3m -> Normal 40m<hkd>
    | Points 3.5m -> Normal 320m<hkd>
    | Points 4m -> Normal 640m<hkd>
    | Points 4.5m -> Normal 9600m<hkd>
    | Points 5m -> Third
    | Points 5.5m -> Second
    | Points 6m -> First
    | _ -> NoPrize

