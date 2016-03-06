module Rexcfnghk.MarkSixParser.MarkSixNumber

open System

[<Sealed>]
type T =
    interface IComparable
    interface IComparable<T>
    interface IEquatable<T>

val create : int -> ValidationResult<T>

val createOption : (int -> T option)

val value : T -> int