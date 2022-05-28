module Page.Create exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Crypto
import Html exposing (Html, a, br, button, div, h1, input, p, text)
import Html.Attributes exposing (class, href, type_)
import Html.Events exposing (onClick, onInput)
import Storage
import Url.Builder exposing (crossOrigin)


type alias Model =
    { id : Maybe String
    , cleartext : String
    , visible : Bool
    , key : Maybe Crypto.Key
    , base_url : String
    }


type Msg
    = ToggleVisibility
    | UpdateCleartext String
    | RequestEncryption
    | ReceivedKey Crypto.Key
    | ReceivedEncryption Crypto.EncryptionResponse
    | StoredEncrypted Storage.StorageResponse


init : String -> ( Model, Cmd Msg )
init base_url =
    ( { base_url = base_url, visible = False, cleartext = "", id = Nothing, key = Nothing }, Crypto.requestKey () )


subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.batch
        [ Crypto.receiveKey ReceivedKey
        , Crypto.receiveEncryption ReceivedEncryption
        , Storage.receiveStorage StoredEncrypted
        ]


view : Model -> Html Msg
view { id, visible, base_url, key } =
    case ( id, key ) of
        ( Just idValue, Just keyValue ) ->
            let
                link =
                    crossOrigin base_url [ "v", idValue, keyValue.key ] []
            in
            div []
                [ h1 [] [ text "Secret created!" ]
                , p []
                    [ text "Please copy the following link and use it when distributing the secret:"
                    , br [] []
                    , br [] []
                    , a [ href link ] [ text link ]
                    ]
                ]

        _ ->
            div []
                [ h1 [] [ text "Create a new secret!" ]
                , div [ class "row-of-items" ]
                    [ input
                        [ onInput UpdateCleartext
                        , if visible then
                            type_ "text"

                          else
                            type_ "password"
                        ]
                        []
                    , button [ onClick ToggleVisibility, class "neutral" ] [ text "👁" ]
                    , button [ onClick RequestEncryption, class "ok" ] [ text "✔" ]
                    ]
                ]


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
                    ( model, Crypto.requestEncryption { key = key, cleartext = model.cleartext } )

                Nothing ->
                    ( model, Cmd.none )

        ReceivedKey key ->
            ( { model | key = Just key }, Cmd.none )

        ReceivedEncryption encrypted ->
            case model.key of
                Just keyValue ->
                    ( model, Storage.requestStorage { iv = encrypted.iv, ciphertext = encrypted.ciphertext, algorithm = keyValue.algorithm } )

                Nothing ->
                    ( model, Cmd.none )

        StoredEncrypted { id } ->
            ( { model | id = Just id }, Cmd.none )
