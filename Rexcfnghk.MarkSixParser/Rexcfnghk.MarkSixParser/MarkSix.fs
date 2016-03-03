module MarkSix

open Models
open System

let drawNumbers () =
    let r = Random()
    [ for _ in 1..6 -> r.Next(49) + 1 |> MarkSixNumber.create |> Option.get ]

let addDrawResultNumbers =
    let rec addDrawResultNumbersImpl acc getNumber =
        let count = Set.count acc
        if count = 7
        then acc |> Set.toList
        else
            let element = 
                getNumber ()
                |> if count = 6 then ExtraNumber else DrawnNumber
            let updated = Set.add element acc
            addDrawResultNumbersImpl updated getNumber
    addDrawResultNumbersImpl Set.empty
