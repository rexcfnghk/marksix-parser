module Rexcfnghk.MarkSixParser.Runner.Decision

open Rexcfnghk.MarkSixParser

type T = Yes | No

val toResult : char -> ValidationResult<T>