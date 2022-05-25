module Page.View exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Crypto
import Html exposing (Html, a, br, div, h1, p, text)
import Html.Attributes exposing (href)
import Page.NotFound as NotFound
import Storage
import Url.Builder exposing (crossOrigin)


type alias Model =
    { id : Maybe String
    , key : Maybe String
    , lookup : Maybe Storage.LookupResponse
    , cleartext : Maybe String
    , base_url : String
    }


type Msg
    = ReceivedLookup Storage.LookupResponse
    | ReceivedDecryption Crypto.DecryptionResponse


subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.batch
        [ Storage.receiveLookup ReceivedLookup, Crypto.receiveDecryption ReceivedDecryption ]


init : Maybe String -> Maybe String -> String -> ( Model, Cmd Msg )
init id key base_url =
    ( { id = id, key = key, lookup = Nothing, cleartext = Nothing, base_url = base_url }
    , case id of
        Just idValue ->
            Storage.requestLookup { id = idValue }

        Nothing ->
            Cmd.none
    )


view : Model -> Html Msg
view model =
    case ( model.id, model.cleartext ) of
        ( Just idValue, Just cleartextValue ) ->
            let
                link =
                    crossOrigin model.base_url [ "d", idValue ] []
            in
            div []
                [ h1 [] [ cleartextValue |> text ]
                , p []
                    [ text "Use the following link to delete this secret:"
                    , br [] []
                    , br [] []
                    , a [ href link ] [ text link ]
                    ]
                ]

        _ ->
            NotFound.view


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
