module Page.Create exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Crypto as Crypto
import Html exposing (Html, a, br, button, div, h1, input, p, text)
import Html.Attributes exposing (href, type_)
import Html.Events exposing (onClick, onInput)
import Url.Builder exposing (crossOrigin)


type alias Model =
    { visible : Bool
    , cleartext : String
    , key : Maybe Crypto.Key
    , encrypted : Maybe Crypto.EncryptedValue
    , base_url : String
    }


type Msg
    = ToggleVisibility
    | UpdateCleartext String
    | EncryptCleartext
    | ReceivedKey Crypto.Key
    | ReceivedEncrypted Crypto.EncryptedValue


init : String -> ( Model, Cmd Msg )
init base_url =
    ( { base_url = base_url, visible = False, cleartext = "", key = Nothing, encrypted = Nothing }, Crypto.requestKey () )


subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.batch
        [ Crypto.receiveKey ReceivedKey
        , Crypto.receiveEncrypted ReceivedEncrypted
        ]


view : Model -> Html Msg
view { visible, encrypted, base_url } =
    case encrypted of
        Just value ->
            let
                link =
                    crossOrigin base_url [ "view", value.key.key ] []
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

        Nothing ->
            div []
                [ h1 [] [ text "Create a new secret!" ]
                , p []
                    [ input
                        [ onInput UpdateCleartext
                        , if visible then
                            type_ "text"

                          else
                            type_ "password"
                        ]
                        []
                    , button [ onClick ToggleVisibility ] [ text "Show" ]
                    , button [ onClick EncryptCleartext ] [ text "Create" ]
                    ]
                ]


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        ToggleVisibility ->
            ( { model | visible = not model.visible }, Cmd.none )

        UpdateCleartext newValue ->
            ( { model | cleartext = newValue }, Cmd.none )

        EncryptCleartext ->
            case model.key of
                Just key ->
                    ( model, Crypto.requestEncryption { key = key, cleartext = model.cleartext } )

                Nothing ->
                    ( model, Crypto.requestKey () ) |> Debug.log "Trying to fetch key again: "

        ReceivedKey key ->
            ( { model | key = Just key }, Cmd.none )

        ReceivedEncrypted response ->
            ( { model | encrypted = Just response }, Cmd.none )
