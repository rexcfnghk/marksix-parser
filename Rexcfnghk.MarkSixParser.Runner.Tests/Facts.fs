module Rexcfnghk.MarkSixParser.Runner.Tests.Facts

open Rexcfnghk.MarkSixParser
open Rexcfnghk.MarkSixParser.Models
open Rexcfnghk.MarkSixParser.Runner.MarkSixNumberReader
open Xunit
open FsCheck.Xunit
open Swensen.Unquote

[<Fact>]
let ``getDrawNumbers accepts markSixNumberReader parameter`` () =
    let m6Numbers = 
        [| 6; 7; 12; 15; 27; 36; 29 |]
        |> Array.map (MarkSixNumber.create)

    let drawResults = getDrawResultNumbers (Array.get m6Numbers) ignore ()

    let (DrawResults (DrawnNumber m1, DrawnNumber m2, DrawnNumber m3, 
                    DrawnNumber m4, DrawnNumber m5, DrawnNumber m6, ExtraNumber e)) = drawResults
    let i1, i2, i3, i4, i5, i6, e = MarkSixNumber.value m1, MarkSixNumber.value m2, MarkSixNumber.value m3, 
                                        MarkSixNumber.value m4, MarkSixNumber.value m5, MarkSixNumber.value m6, 
                                        MarkSixNumber.value e
     
    i1 =! 6
    i2 =! 7
    i3 =! 12
    i4 =! 15
    i5 =! 27
    i6 =! 36
    e =! 29

[<Fact>]
let ``getUsersDraw accepts markSixNumberReader parameter`` () = 
    let m6Numbers =
        [| 3; 4; 24; 28; 30; 32 |]
        |> Array.map (MarkSixNumber.create)

    let usersDraw = getUsersDrawNumbers (Array.get m6Numbers) ignore ()

    let (UsersDraw (m1, m2, m3, m4, m5, m6)) = usersDraw
    let i1, i2, i3, i4, i5, i6 = MarkSixNumber.value m1, MarkSixNumber.value m2, MarkSixNumber.value m3, 
                                    MarkSixNumber.value m4, MarkSixNumber.value m5, MarkSixNumber.value m6
     
    [| i1; i2; i3; i4; i5; i6 |] =! [| 3; 4; 24; 28; 30; 32 |]