-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Storage exposing
    ( check
    , delete
    , retrieve
    , store
    )

import Api
import Dict
import Secret exposing (Secret)


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
        Secret.decoder
        (\result ->
            case result of
                Ok ( secret, _ ) ->
                    msg (Just secret)

                Err _ ->
                    msg Nothing
        )


store : Secret -> (Maybe String -> msg) -> Cmd msg
store secret msg =
    api.post (Secret.encode secret)
        Secret.decoder
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
