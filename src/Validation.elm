module Validation exposing (secretConstraints, validSecret)

import Result exposing (Result)


secretConstraints : { minLength : Int, maxLength : Int }
secretConstraints =
    { minLength = 1, maxLength = 72 }


validSecret : String -> Result String String
validSecret string =
    let
        length =
            String.length string
    in
    if length < secretConstraints.minLength then
        Err "Secret is too short."

    else if length > secretConstraints.maxLength then
        Err "Secret is too long."

    else
        Ok string
