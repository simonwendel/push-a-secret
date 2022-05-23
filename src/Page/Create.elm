module Page.Create exposing (Model, Msg, init, subscriptions, update, view)

import Crypto as Crypto
import Html exposing (Html, a, br, h1, text)


type alias Model =
    { key : Maybe Crypto.KeyModel
    , secret : Maybe String
    , encrypted : Maybe String
    }


type Msg
    = RequestKey
    | ReceivedKey Crypto.KeyModel


init : Maybe String -> ( Model, Cmd Msg )
init secret =
    ( { secret = secret, key = Nothing, encrypted = Nothing }, Crypto.requestKey () )


subscriptions : Model -> Sub Msg
subscriptions _ =
    Crypto.receiveKey ReceivedKey


view : Model -> Html msg
view model =
    h1 []
        [ case model.key of
            Just key ->
                "Super secret key: " ++ key.key ++ ", " ++ key.algorithm |> text

            Nothing ->
                "No key created!" |> text
        ]


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        ReceivedKey key ->
            ( { model | key = Just key }, Cmd.none )

        RequestKey ->
            ( model, Crypto.requestKey () )
