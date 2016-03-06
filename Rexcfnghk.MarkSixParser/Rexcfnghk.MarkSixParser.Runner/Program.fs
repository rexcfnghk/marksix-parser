// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open Models
open MarkSix
open System

let addDrawResultNumbers' = getDrawResultNumbers (Console.ReadLine >> int)
    
[<EntryPoint>]
let main argv = 
    let result = MarkSix.drawNumbers()
    printfn "%A" result
    System.Console.ReadLine() |> ignore
    0 // return an integer exit code