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

import Aria exposing (ariaDescription, ariaLabel)
import Crypto
import Html exposing (Html, button, em, h1, p, text, textarea)
import Html.Attributes exposing (class, cols, readonly)
import Html.Events exposing (onClick)
import Page.Loading as Loading
import Page.NotFound as NotFound
import Render exposing (renderContent, renderRow, setValueVisible)
import Route exposing (Router)
import Secret exposing (Secret)
import Storage


type alias Model =
    { id : String
    , key : String
    , firstLoad : Bool
    , lookup : Maybe Secret
    , cleartext : Maybe String
    , visible : Bool
    , router : Router Msg
    }


type Msg
    = Read (Maybe Secret)
    | Decrypted Crypto.DecryptionResponse
    | Toggle
    | Delete


subscriptions : Model -> Sub Msg
subscriptions _ =
    Crypto.receiveDecryption Decrypted


init : String -> String -> Router Msg -> ( Model, Cmd Msg )
init id key router =
    ( { id = id
      , key = key
      , lookup = Nothing
      , cleartext = Nothing
      , firstLoad = True
      , visible = False
      , router = router
      }
    , Storage.retrieve id Read
    )


view : Model -> Html Msg
view model =
    case model.cleartext of
        Just value ->
            renderContent
                [ h1 [] [ text "View secret" ]
                , p []
                    [ text "We've retrieved a secret previously stored in our database and decrypted it with your key."
                    ]
                , p [ class "only-on-wide-screens" ]
                    [ em []
                        [ text "Tip: Click the Show/Hide button to toggle displaying secret. Hit the Delete button to delete the secret forever."
                        ]
                    ]
                , textarea
                    [ readonly True
                    , Html.Attributes.value value
                    , setValueVisible model.visible
                    , cols 120
                    , ariaLabel "secret"
                    , ariaDescription "Showing decrypted secret"
                    ]
                    []
                , renderRow [ class "row-of-buttons" ]
                    [ button [ onClick Toggle, class "neutral" ]
                        [ text <|
                            if model.visible then
                                "Hide"

                            else
                                "Show"
                        ]
                    , button [ onClick Delete, class "cancel" ]
                        [ text "Delete"
                        ]
                    ]
                ]

        Nothing ->
            if model.firstLoad then
                Loading.view

            else
                NotFound.view


update : Msg -> Model -> ( Model, Cmd Msg )
update msg ({ router, id } as model) =
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

        Toggle ->
            ( { model | visible = not model.visible }, Cmd.none )

        Delete ->
            ( model, router.delete id )
