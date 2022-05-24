module Page.View exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Crypto
import Html exposing (Html, h1, text)
import Page.NotFound as NotFound
import Storage


type alias Model =
    { id : Maybe String
    , key : Maybe String
    , lookup : Maybe Storage.LookupResponse
    , cleartext : Maybe String
    }


type Msg
    = ReceivedLookup Storage.LookupResponse
    | ReceivedDecryption Crypto.DecryptionResponse


subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.batch
        [ Storage.receiveLookup ReceivedLookup, Crypto.receiveDecryption ReceivedDecryption ]


init : Maybe String -> Maybe String -> ( Model, Cmd msg )
init id key =
    ( { id = id, key = key, lookup = Nothing, cleartext = Nothing }
    , case id of
        Just idValue ->
            Storage.requestLookup { id = idValue }

        Nothing ->
            Cmd.none
    )


view : Model -> Html msg
view model =
    h1 []
        [ case model.cleartext of
            Just cleartextValue ->
                "Super secret stuff: " ++ cleartextValue |> text

            Nothing ->
                NotFound.view
        ]


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        ReceivedLookup lookup ->
            case model.key of
                Just keyValue ->
                    ( { model | lookup = Just lookup }
                    , Crypto.requestDecryption
                        { key =
                            { key = keyValue
                            , algorithm = lookup.algorithm
                            }
                        , iv = lookup.iv
                        , ciphertext = lookup.ciphertext
                        }
                    )

                Nothing ->
                    ( model, Cmd.none )

        ReceivedDecryption { cleartext } ->
            ( { model | cleartext = Just cleartext }, Cmd.none )
