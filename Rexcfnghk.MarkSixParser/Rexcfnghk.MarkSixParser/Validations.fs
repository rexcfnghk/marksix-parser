namespace Rexcfnghk.MarkSixParser

type ErrorMessage = ErrorMessage of string

[<StructuredFormatDisplay("{AsString}")>]
type ValidationResult<'T> =
    | Success of 'T
    | Error of ErrorMessage
    override this.ToString() =
        match this with
        | Success s -> sprintf "%A" s
        | Error e -> sprintf "%A" e
    member this.AsString = this.ToString()

module ValidationResult =
    let success = Success

    let error = Error

    let errorFromString<'T> : string -> ValidationResult<'T> = ErrorMessage >> Error

    let doubleMap successHandler errorHandler = function
        | Success x -> successHandler x
        | Error e -> errorHandler e

    let bind f = doubleMap f Error

    let (>>=) x f = bind f x

    let extractOption<'T> : ValidationResult<'T> -> 'T option = doubleMap Some (fun _ -> None)

    let extract<'T> : ValidationResult<'T> -> 'T = doubleMap id (fun (ErrorMessage e) -> invalidOp e)

    let (>=>) f g = f >> bind g