module Rexcfnghk.MarkSixParser.MarkSixNumber

open System

[<Sealed>]
type T =
    interface IComparable
    interface IComparable<T>
    interface IEquatable<T>

val create : int -> Result<T, ErrorMessage>

val value : T -> int
