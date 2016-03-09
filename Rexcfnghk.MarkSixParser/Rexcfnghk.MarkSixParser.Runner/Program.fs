open UseCases

open System

[<EntryPoint>]
let main _ = 
    printfn "Enter draw results"
    let drawResults = getDrawResultNumbers' ()
    printfn "The draw results are %A" drawResults
    printfn "Enter user's draw"
    let usersDraw = getUsersDrawNumbers' ()
    printfn "User's draw is %A" usersDraw
    let prize = checkResults' drawResults usersDraw
    printfn "Your prize is %A"  prize

    Console.ReadLine() |> ignore
    0 // return an integer exit code