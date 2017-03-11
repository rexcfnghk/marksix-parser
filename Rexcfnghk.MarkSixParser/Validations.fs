namespace Rexcfnghk.MarkSixParser

type ErrorMessage = ErrorMessage of string

module Result =

    open Rexcfnghk.MarkSixParser.Utilities

    let ok = Ok

    let error = Error

    let errorFromString<'a> : string -> Result<'a, ErrorMessage> = ErrorMessage >> Error

    let doubleMap successHandler errorHandler = function
        | Ok x -> successHandler x
        | Error e -> errorHandler e

    let rec retryable errorHandler f =
        match f () with
        | Ok x -> x
        | Error e ->
            errorHandler e
            retryable errorHandler f

    let bind<'a, 'b> : ('a -> Result<'b, ErrorMessage>) -> Result<'a, ErrorMessage> -> Result<'b, ErrorMessage> =
        flip doubleMap Error

    let (>>=) x f = bind f x

    let map f = bind (f >> Ok)

    let (<!>) = map

    let extract<'a> : Result<'a, ErrorMessage> -> 'a = doubleMap id (function ErrorMessage e -> invalidOp e)

    let (>=>) f g = f >> bind g

    let apply fV xV =
        match fV, xV with
        | Ok f, Ok x -> Ok (f x)
        | Error e, _ | _, Error e -> Error e

    let (<*>) = apply

    // http://fsharpforfunandprofit.com/posts/elevated-world-4/#traverse
    let traverse f list =
        let cons h t = h :: t
        let folder h t = cons <!> (f h) <*> t
        List.foldBack folder list (Ok [])
