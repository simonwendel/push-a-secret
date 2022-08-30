-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Page.Create exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Crypto
import Html exposing (Html, a, button, em, h1, p, span, text, textarea)
import Html.Attributes exposing (autocomplete, autofocus, class, cols, href, maxlength, minlength, required, spellcheck)
import Html.Events exposing (onClick, onInput)
import Render exposing (renderContent, renderRow, setValueVisible)
import Route exposing (Router)
import Storage
import String exposing (fromInt)
import Validation exposing (secretConstraints, validateSecret)


type alias Model =
    { id : Maybe String
    , cleartext : String
    , visible : Bool
    , key : Maybe Crypto.Key
    , router : Router Msg
    , error_message : Maybe String
    }


type Msg
    = Toggle
    | UpdateText String
    | Encrypt
    | GeneratedKey Crypto.Key
    | Encrypted Crypto.EncryptionResponse
    | Stored (Maybe String)


defaultTtl : Int
defaultTtl =
    1


init : Router Msg -> ( Model, Cmd Msg )
init router =
    ( { router = router
      , visible = False
      , cleartext = ""
      , id = Nothing
      , key = Nothing
      , error_message = Nothing
      }
    , Crypto.requestKey ()
    )


subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.batch
        [ Crypto.receiveKey GeneratedKey
        , Crypto.receiveEncryption Encrypted
        ]


view : Model -> Html Msg
view { id, visible, router, key, error_message } =
    case ( id, key ) of
        ( Just idValue, Just keyValue ) ->
            let
                link =
                    router.viewLink idValue keyValue.key
            in
            renderContent
                [ h1 [] [ text "Secret created" ]
                , p [] [ text "Please copy the following link and use it when distributing the secret:" ]
                , p [] [ a [ href link ] [ text link ] ]
                ]

        _ ->
            renderContent <|
                [ h1 [] [ text "Create secret" ]
                , p []
                    [ text "Create your new secret by entering it in the textbox below. A secret"
                    , em [] [ text " must " ]
                    , "have a character length of at least "
                        ++ fromInt secretConstraints.minLength
                        ++ " and at most "
                        ++ fromInt secretConstraints.maxLength
                        ++ "."
                        |> text
                    ]
                , p [ class "only-on-large-screens" ]
                    [ em []
                        [ text "HINT: Click the 'show' icon to view the hidden text or hit the checkmark to encrypt your secret."
                        ]
                    ]
                , textarea
                    [ onInput UpdateText
                    , autofocus True
                    , required True
                    , autocomplete False
                    , minlength secretConstraints.minLength
                    , maxlength secretConstraints.maxLength
                    , setValueVisible visible
                    , cols secretConstraints.maxLength
                    , spellcheck False
                    ]
                    []
                , renderRow [ class "row-of-buttons" ]
                    [ button [ onClick Toggle, class "neutral" ]
                        [ text <|
                            if visible then
                                "Hide"

                            else
                                "Show"
                        ]
                    , button [ onClick Encrypt, class "ok" ]
                        [ text "Create"
                        ]
                    ]
                ]
                    ++ (case error_message of
                            Just message ->
                                [ p []
                                    [ span [ class "error" ] [ text message ]
                                    ]
                                ]

                            Nothing ->
                                []
                       )


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        Toggle ->
            ( { model | visible = not model.visible }, Cmd.none )

        UpdateText newValue ->
            ( { model | cleartext = newValue }, Cmd.none )

        Encrypt ->
            case model.key of
                Just key ->
                    case validateSecret model.cleartext of
                        Ok validCleartext ->
                            ( model, Crypto.requestEncryption { key = key, cleartext = validCleartext } )

                        Err message ->
                            ( { model | error_message = Just message }, Cmd.none )

                Nothing ->
                    ( model, Cmd.none )

        GeneratedKey key ->
            ( { model | key = Just key }, Cmd.none )

        Encrypted encrypted ->
            case model.key of
                Just keyValue ->
                    ( model
                    , Storage.store
                        { iv = encrypted.iv
                        , ciphertext = encrypted.ciphertext
                        , algorithm = keyValue.algorithm
                        , ttl = defaultTtl
                        }
                        Stored
                    )

                Nothing ->
                    ( model, Cmd.none )

        Stored id ->
            ( { model | id = id }, Cmd.none )
