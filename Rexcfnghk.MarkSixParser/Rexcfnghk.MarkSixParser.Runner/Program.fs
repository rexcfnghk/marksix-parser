open UseCases

open System

[<EntryPoint>]
let main _ = 
    printfn "Enter draw results"

    let drawResults = getDrawResultNumbers' ()
    printfn "The draw results are %A%s" drawResults Environment.NewLine

    let usersDrawList = getMultipleUsersDraw ()

    let prizesResult = checkMultipleResults drawResults usersDrawList
    printPrizes prizesResult

    stdin.ReadLine() |> ignore
    0 // return an integer exit code