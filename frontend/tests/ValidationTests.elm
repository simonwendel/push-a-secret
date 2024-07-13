-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module ValidationTests exposing (validationModuleTests)

import Expect exposing (atLeast, err, ok)
import Fuzz
import Test exposing (Test, concat, describe, fuzz, test)
import Validation as Sut


validationModuleTests : Test
validationModuleTests =
    let
        testConstants =
            describe "testing sanity of constants"
                [ test
                    "secret min length must be >= 0"
                    (\_ ->
                        Sut.secretConstraints.minLength
                            |> atLeast 0
                    )
                , test
                    "secret min length must be >= the min length"
                    (\_ ->
                        Sut.secretConstraints.maxLength
                            |> atLeast Sut.secretConstraints.minLength
                    )
                ]

        testInvalidSecrets =
            describe "testing invalid secrets"
                [ test
                    "empty secret is not valid"
                    (\_ ->
                        Sut.validateSecret ""
                            |> err
                    )
                , test
                    "secret shorter than min length is not valid"
                    (\_ ->
                        String.repeat (Sut.secretConstraints.minLength - 1) "a"
                            |> Sut.validateSecret
                            |> err
                    )
                , test
                    "secret longer than max length is not valid"
                    (\_ ->
                        String.repeat (Sut.secretConstraints.maxLength + 1) "a"
                            |> Sut.validateSecret
                            |> err
                    )
                ]

        testValidSecrets =
            describe "testing valid secrets"
                [ test
                    "secret with min length is valid"
                    (\_ ->
                        String.repeat Sut.secretConstraints.minLength "a"
                            |> Sut.validateSecret
                            |> ok
                    )
                , test
                    "secret with max length is valid"
                    (\_ ->
                        String.repeat Sut.secretConstraints.maxLength "a"
                            |> Sut.validateSecret
                            |> ok
                    )
                , fuzz (Fuzz.stringOfLengthBetween Sut.secretConstraints.minLength Sut.secretConstraints.maxLength)
                    "secret with length within allowed range is valid"
                    (Sut.validateSecret >> ok)
                ]
    in
    concat
        [ describe "(constants)"
            [ testConstants
            ]
        , describe "(function) validateSecret"
            [ testInvalidSecrets
            , testValidSecrets
            ]
        ]



-- {-| Fuzzing strings of a certain length using characters from the Basic Multilingual Plane,
-- including the unused range [2FE0, 2FEF].
-- -}
-- basicMultilingual : ( Int, Int ) -> Fuzzer String
-- basicMultilingual =
--     fuzzString (Random.Char.char (Char.toCode '\u{0000}') (Char.toCode '\u{FFFF}'))
-- fuzzString : Generator Char -> ( Int, Int ) -> Fuzzer String
-- fuzzString characterGenerator ( min, max ) =
--     let
--         stringGenerator =
--             rangeLengthString min max characterGenerator
--     in
--     Fuzz.custom stringGenerator Shrink.string
