module ValidationTests exposing (validationModuleTests)

import Expect exposing (atLeast, equal, err, ok)
import Fuzzers exposing (unicodeBasicLatin)
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
                        Sut.validSecret ""
                            |> err
                    )
                , test
                    "secret shorter than min length is not valid"
                    (\_ ->
                        String.repeat (Sut.secretConstraints.minLength - 1) "a"
                            |> Sut.validSecret
                            |> err
                    )
                , test
                    "secret longer than max length is not valid"
                    (\_ ->
                        String.repeat (Sut.secretConstraints.maxLength + 1) "a"
                            |> Sut.validSecret
                            |> err
                    )
                ]

        testValidSecrets =
            describe "testing valid secrets"
                [ test
                    "secret with min length is valid"
                    (\_ ->
                        String.repeat Sut.secretConstraints.minLength "a"
                            |> Sut.validSecret
                            |> ok
                    )
                , test
                    "secret with max length is valid"
                    (\_ ->
                        String.repeat Sut.secretConstraints.maxLength "a"
                            |> Sut.validSecret
                            |> ok
                    )
                , fuzz (unicodeBasicLatin ( Sut.secretConstraints.minLength, Sut.secretConstraints.maxLength ))
                    "secret with length within allowed range is valid"
                    (Sut.validSecret >> ok)
                ]
    in
    concat
        [ describe "(constants)"
            [ testConstants
            ]
        , describe "(function) validSecret"
            [ testInvalidSecrets
            , testValidSecrets
            ]
        ]
