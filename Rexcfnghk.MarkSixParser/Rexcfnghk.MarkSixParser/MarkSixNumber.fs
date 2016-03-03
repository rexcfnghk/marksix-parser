module MarkSixNumber

type T = MarkSixNumber of int

let create input = 
    if input > 1 && input <=49
    then Some <| MarkSixNumber input
    else None

let value (MarkSixNumber num) = num