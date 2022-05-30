module Fuzzers.Unicode exposing (basicMultilingual)

import Fuzz exposing (Fuzzer)
import Random exposing (Generator)
import Random.Char
import Random.String exposing (rangeLengthString)
import Shrink


{-| Fuzzing strings of a certain length using characters from the Basic Multilingual Plane,
including the unused range [2FE0, 2FEF].
-}
basicMultilingual : ( Int, Int ) -> Fuzzer String
basicMultilingual =
    fuzzString (Random.Char.char (Char.toCode '\u{0000}') (Char.toCode '\u{FFFF}'))


fuzzString : Generator Char -> ( Int, Int ) -> Fuzzer String
fuzzString characterGenerator ( min, max ) =
    let
        stringGenerator =
            rangeLengthString min max characterGenerator
    in
    Fuzz.custom stringGenerator Shrink.string
