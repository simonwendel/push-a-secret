-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Secret exposing (Secret, decoder, encode)

import Json.Decode as D exposing (Decoder)
import Json.Encode as E


type alias Secret =
    { algorithm : String
    , iv : String
    , ciphertext : String
    , ttl : Int
    }


decoder : Decoder Secret
decoder =
    D.map4 Secret
        (D.field "algorithm" D.string)
        (D.field "iv" D.string)
        (D.field "ciphertext" D.string)
        (D.field "ttl" D.int)


encode : Secret -> E.Value
encode secret =
    E.object
        [ ( "algorithm", E.string secret.algorithm )
        , ( "iv", E.string secret.iv )
        , ( "ciphertext", E.string secret.ciphertext )
        , ( "ttl", E.int secret.ttl )
        ]
