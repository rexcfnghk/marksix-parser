module Rexcfnghk.MarkSixParser.Runner.Program

open UseCases

[<EntryPoint>]
let main _ =
    let drawResults = getDrawResultNumbers' ()

    let usersDrawList = getMultipleUsersDraw' ()

    let prizesResult = checkMultipleUsersDraws' drawResults usersDrawList
    printPrizes prizesResult

    stdin.ReadLine() |> ignore
    0 // return an integer exit code
