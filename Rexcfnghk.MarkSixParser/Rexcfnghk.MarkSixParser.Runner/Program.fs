open Rexcfnghk.MarkSixParser
open System

let getDrawResultNumbers' () = MarkSix.getDrawResultNumbers (printfn "%A") (Console.ReadLine >> int)

let getUsersDrawNumbers' () = MarkSix.getUsersDrawNumber (printfn "%A") (Console.ReadLine >> int)

let checkResults' = MarkSix.checkResults (printfn "%A")
    
[<EntryPoint>]
let main _ = 
    printfn "Enter draw results"
    let drawResults = getDrawResultNumbers' ()
    printfn "The draw results are %A" drawResults
    printfn "Enter user's draw"
    let usersDraw = getUsersDrawNumbers' ()
    printfn "The draw results are %A" usersDraw
    let prize = checkResults' drawResults usersDraw
    printfn "Your prize is %A"  prize

    Console.ReadLine() |> ignore
    0 // return an integer exit code