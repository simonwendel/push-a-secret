module Storage exposing
    ( Secret
    , check
    , delete
    , retrieve
    , store
    )

import Api
import Dict
import Http
import Json.Decode as D exposing (Decoder)
import Json.Encode as E


type alias Secret =
    { algorithm : String
    , iv : String
    , ciphertext : String
    }


api : Api.Client msg a
api =
    Api.client


check : String -> (Bool -> msg) -> Cmd msg
check id msg =
    api.head id
        (\result ->
            case result of
                Ok _ ->
                    msg True

                Err _ ->
                    msg False
        )


retrieve : String -> (Maybe Secret -> msg) -> Cmd msg
retrieve id msg =
    api.get id
        decoder
        (\result ->
            case result of
                Ok ( secret, _ ) ->
                    msg (Just secret)

                Err _ ->
                    msg Nothing
        )


store : Secret -> (Maybe String -> msg) -> Cmd msg
store secret msg =
    api.post (encode secret)
        decoder
        (\result ->
            case result of
                Ok ( _, headers ) ->
                    case Dict.get "location" headers of
                        Just location ->
                            msg <| Just <| String.replace "/secret/" "" location

                        Nothing ->
                            msg Nothing

                Err _ ->
                    msg Nothing
        )


delete : String -> (Bool -> msg) -> Cmd msg
delete id msg =
    api.delete id
        (\result ->
            case result of
                Ok _ ->
                    msg True

                Err _ ->
                    msg False
        )


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
