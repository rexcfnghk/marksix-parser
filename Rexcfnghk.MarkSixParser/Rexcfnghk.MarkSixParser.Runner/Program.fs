open UseCases

open System

[<EntryPoint>]
let main _ = 
    printfn "Enter draw results"

    let drawResults = getDrawResultNumbers' ()
    printfn "The draw results are %A%s" drawResults Environment.NewLine

    printfn "Enter user's draw"
    let usersDraw = getUsersDrawNumbers' ()
    printfn "User's draw is %A%s" usersDraw Environment.NewLine

    let prize = checkResults' drawResults usersDraw
    printfn "Your prize is %A"  prize

    Console.ReadLine() |> ignore
    0 // return an integer exit code