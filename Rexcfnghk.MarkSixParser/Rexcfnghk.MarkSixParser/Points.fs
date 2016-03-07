module Rexcfnghk.MarkSixParser.Points

open Models

let add p1 p2 =
    let (Points d1, Points d2) = p1, p2
    Points <| d1 + d2

let (.+.) = add

let getPrize = function
    | Points 3m -> Normal 40m<hkd>
    | Points 3.5m -> Normal 320m<hkd>
    | Points 4m -> Normal 640m<hkd>
    | Points 4.5m -> Normal 9600m<hkd>
    | Points 5m -> Third
    | Points 5.5m -> Second
    | Points 6m -> First
    | _ -> NoPrize