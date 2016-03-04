namespace Rexcfnghk.MarkSixParser

type ErrorMessage = ErrorMessage of string

type ValidationResult<'a> =
    | Success of 'a
    | Error of ErrorMessage

module ValidationResult =
    let extract = function
        | Success a -> Some a
        | Error _ -> None

    let success = Success

    let error = Error