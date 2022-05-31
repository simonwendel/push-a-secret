module Page.Create exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Crypto
import Html exposing (Html, a, button, em, h1, input, label, p, span, strong, text)
import Html.Attributes exposing (autofocus, class, href, maxlength, minlength, required, type_)
import Html.Events exposing (onClick, onInput)
import Render exposing (renderContent, renderRow)
import Storage
import String exposing (fromInt)
import Url.Builder exposing (crossOrigin)
import Validation exposing (secretConstraints, validateSecret)


type alias Model =
    { id : Maybe String
    , cleartext : String
    , visible : Bool
    , key : Maybe Crypto.Key
    , base_url : String
    , error_message : Maybe String
    }


type Msg
    = ToggleVisibility
    | UpdateCleartext String
    | RequestEncryption
    | ReceivedKey Crypto.Key
    | ReceivedEncryption Crypto.EncryptionResponse
    | StoredEncrypted Storage.CreateResponse


init : String -> ( Model, Cmd Msg )
init base_url =
    ( { base_url = base_url, visible = False, cleartext = "", id = Nothing, key = Nothing, error_message = Nothing }, Crypto.requestKey () )


subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.batch
        [ Crypto.receiveKey ReceivedKey
        , Crypto.receiveEncryption ReceivedEncryption
        , Storage.receiveCreate StoredEncrypted
        ]


view : Model -> Html Msg
view { id, visible, base_url, key, error_message } =
    case ( id, key ) of
        ( Just idValue, Just keyValue ) ->
            let
                link =
                    crossOrigin base_url [ "v", idValue, keyValue.key ] []
            in
            renderContent
                [ h1 [] [ text "Your secret has been created!" ]
                , p [] [ text "Please copy the following link and use it when distributing the secret:" ]
                , p [] [ a [ href link ] [ text link ] ]
                ]

        _ ->
            renderContent <|
                [ h1 [] [ text "Create a secret!" ]
                , p []
                    [ text "Create your new secret by entering it in the password box below. A secret"
                    , strong [] [ text " MUST " ]
                    , "have a character length of at least "
                        ++ fromInt secretConstraints.minLength
                        ++ " and at most "
                        ++ fromInt secretConstraints.maxLength
                        ++ "."
                        |> text
                    ]
                , p []
                    [ em []
                        [ text "HINT: Click the 'show' icon to view the hidden text or hit the checkmark to encrypt your secret."
                        ]
                    ]
                , renderRow
                    [ label []
                        [ text "Secret: "
                        , input
                            [ onInput UpdateCleartext
                            , autofocus True
                            , required True
                            , minlength secretConstraints.minLength
                            , maxlength secretConstraints.maxLength
                            , if visible then
                                type_ "text"

                              else
                                type_ "password"
                            ]
                            []
                        ]
                    , button [ onClick ToggleVisibility, class "neutral" ] [ text "👁" ]
                    , button [ onClick RequestEncryption, class "ok" ] [ text "✔" ]
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
        ToggleVisibility ->
            ( { model | visible = not model.visible }, Cmd.none )

        UpdateCleartext newValue ->
            ( { model | cleartext = newValue }, Cmd.none )

        RequestEncryption ->
            case model.key of
                Just key ->
                    case validateSecret model.cleartext of
                        Ok validCleartext ->
                            ( model, Crypto.requestEncryption { key = key, cleartext = validCleartext } )

                        Err message ->
                            ( { model | error_message = Just message }, Cmd.none )

                Nothing ->
                    ( model, Cmd.none )

        ReceivedKey key ->
            ( { model | key = Just key }, Cmd.none )

        ReceivedEncryption encrypted ->
            case model.key of
                Just keyValue ->
                    ( model, Storage.requestCreate { iv = encrypted.iv, ciphertext = encrypted.ciphertext, algorithm = keyValue.algorithm } )

                Nothing ->
                    ( model, Cmd.none )

        StoredEncrypted { id } ->
            ( { model | id = Just id }, Cmd.none )
