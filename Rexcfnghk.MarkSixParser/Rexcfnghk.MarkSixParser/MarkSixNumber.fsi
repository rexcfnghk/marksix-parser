module Rexcfnghk.MarkSixParser.MarkSixNumber

type T

val create : int -> ValidationResult<T>

val tryCreate : (int -> T option)

val value : T -> int