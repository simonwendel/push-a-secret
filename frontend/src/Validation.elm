module Validation exposing (secretConstraints, validateSecret)

import Result exposing (Result)


secretConstraints : { minLength : Int, maxLength : Int }
secretConstraints =
    { minLength = 1, maxLength = 72 }


validateSecret : String -> Result String String
validateSecret secret =
    let
        length =
            String.length secret
    in
    if length < secretConstraints.minLength then
        Err "Secret is too short."

    else if length > secretConstraints.maxLength then
        Err "Secret is too long."

    else
        Ok secret
