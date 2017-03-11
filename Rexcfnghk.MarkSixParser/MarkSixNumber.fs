module Rexcfnghk.MarkSixParser.MarkSixNumber

open System

[<CustomComparison; CustomEquality>]
[<StructuredFormatDisplay("{AsString}")>]
type T =
    | MarkSixNumber of int

    interface IComparable<T> with
        member this.CompareTo other =
            let (MarkSixNumber thisInt, MarkSixNumber otherInt) = this, other
            compare thisInt otherInt

    interface IComparable with
        member this.CompareTo other =
            (this :> IComparable<T>).CompareTo(other :?> T)

    interface IEquatable<T> with
        member this.Equals other =
            let (MarkSixNumber thisInt, MarkSixNumber otherInt) = this, other
            thisInt = otherInt

    override this.Equals other =
        (this :> IEquatable<T>).Equals(other :?> T)

    override this.GetHashCode() =
        let (MarkSixNumber thisInt) = this
        thisInt

    override this.ToString() = sprintf "%i" <| this.GetHashCode()

    member private this.AsString = this.ToString()

let create input =
    if input >= 1 && input <= 49 then
        input
        |> (MarkSixNumber >> Result.ok)
    else
        "Input out of range"
        |> Result.errorFromString

let value (MarkSixNumber num) = num
