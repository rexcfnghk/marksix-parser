module Rexcfnghk.MarkSixParser.Combinations

let combination k n =
    let rec choose lo = function
        | 0 -> [[]]
        | i ->
            [ for j = lo to (Array.length n) - 1 do
                for ks in choose (j + 1) (i - 1) ->
                    n.[j] :: ks ]
    choose 0 k
