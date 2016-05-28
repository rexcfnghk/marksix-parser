﻿module Rexcfnghk.MarkSixParser.Runner.ErrorHandling

[<Literal>]
let DuplicateErrorMessage = 
    "Duplicate mark six number entered"

let defaultErrorHandler<'a> : 'a -> unit = printfn "%A"

