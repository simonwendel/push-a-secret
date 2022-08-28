-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Page.View exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Crypto
import Html exposing (Html, a, br, h1, p, section, text)
import Html.Attributes exposing (class, href)
import Page.Loading as Loading
import Page.NotFound as NotFound
import Render exposing (renderContent)
import Route
import Secret exposing (Secret)
import Storage
import Url.Builder exposing (crossOrigin)


type alias Model =
    { id : String
    , key : String
    , firstLoad : Bool
    , lookup : Maybe Secret
    , cleartext : Maybe String
    , base_url : String
    }


type Msg
    = Read (Maybe Secret)
    | Decrypted Crypto.DecryptionResponse


subscriptions : Model -> Sub Msg
subscriptions _ =
    Crypto.receiveDecryption Decrypted


init : String -> String -> String -> ( Model, Cmd Msg )
init id key base_url =
    ( { id = id
      , key = key
      , lookup = Nothing
      , cleartext = Nothing
      , base_url = base_url
      , firstLoad = True
      }
    , Storage.retrieve id Read
    )


view : Model -> Html Msg
view model =
    case model.cleartext of
        Just value ->
            let
                link =
                    crossOrigin model.base_url [ Route.delete_path, model.id ] []
            in
            renderContent
                [ h1 [] [ value |> text ]
                , p []
                    [ text "Use the following link to delete this secret:"
                    , br [] []
                    , br [] []
                    , a [ href link ] [ text link ]
                    ]
                ]

        Nothing ->
            if model.firstLoad then
                Loading.view

            else
                NotFound.view


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        Read secret ->
            ( { model | lookup = secret, firstLoad = False }
            , case secret of
                Just value ->
                    Crypto.requestDecryption
                        { key =
                            { key = model.key
                            , algorithm = value.algorithm
                            }
                        , iv = value.iv
                        , ciphertext = value.ciphertext
                        }

                Nothing ->
                    Cmd.none
            )

        Decrypted { cleartext } ->
            ( { model | cleartext = Just cleartext }, Cmd.none )
