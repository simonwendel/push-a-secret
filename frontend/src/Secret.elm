module Secret exposing (Secret, decoder, encode)

import Json.Decode as D exposing (Decoder)
import Json.Encode as E


type alias Secret =
    { algorithm : String
    , iv : String
    , ciphertext : String
    }


decoder : Decoder Secret
decoder =
    D.map3 Secret
        (D.field "algorithm" D.string)
        (D.field "iv" D.string)
        (D.field "ciphertext" D.string)


encode : Secret -> E.Value
encode secret =
    E.object
        [ ( "algorithm", E.string secret.algorithm )
        , ( "iv", E.string secret.iv )
        , ( "ciphertext", E.string secret.ciphertext )
        ]
