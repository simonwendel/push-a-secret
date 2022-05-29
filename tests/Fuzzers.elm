module Fuzzers exposing (unicodeBasicLatin)

import Fuzz exposing (Fuzzer)
import Random exposing (Generator)
import Random.Char exposing (basicLatin)
import Random.String exposing (rangeLengthString)
import Shrink


unicodeBasicLatin : ( Int, Int ) -> Fuzzer String
unicodeBasicLatin =
    fuzzString basicLatin


fuzzString : Generator Char -> ( Int, Int ) -> Fuzzer String
fuzzString characterGenerator ( min, max ) =
    let
        stringGenerator =
            rangeLengthString min max characterGenerator
    in
    Fuzz.custom stringGenerator Shrink.string
