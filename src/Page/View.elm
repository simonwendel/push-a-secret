module Page.View exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Crypto
import Html.Styled exposing (Html, a, br, div, h1, p, text)
import Html.Styled.Attributes exposing (href)
import Page.NotFound as NotFound
import Storage
import Url.Builder exposing (crossOrigin)


type alias Model =
    { id : String
    , key : String
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


init : String -> String -> String -> ( Model, Cmd Msg )
init id key base_url =
    ( { id = id, key = key, lookup = Nothing, cleartext = Nothing, base_url = base_url }
    , Storage.requestLookup { id = id }
    )


view : Model -> Html Msg
view model =
    case model.cleartext of
        Just cleartextValue ->
            let
                link =
                    crossOrigin model.base_url [ "d", model.id ] []
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

        Nothing ->
            NotFound.view


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        ReceivedLookup lookup ->
            ( { model | lookup = Just lookup }
            , Crypto.requestDecryption
                { key =
                    { key = model.key
                    , algorithm = lookup.algorithm
                    }
                , iv = lookup.iv
                , ciphertext = lookup.ciphertext
                }
            )

        ReceivedDecryption { cleartext } ->
            ( { model | cleartext = Just cleartext }, Cmd.none )
