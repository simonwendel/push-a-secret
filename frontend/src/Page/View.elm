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
import Storage
import Url.Builder exposing (crossOrigin)


type alias Model =
    { id : String
    , key : String
    , firstLoad : Bool
    , lookup : Maybe Storage.ReadResponse
    , cleartext : Maybe String
    , base_url : String
    }


type Msg
    = ReadEncrypted Storage.ReadResponse
    | DecryptedSecret Crypto.DecryptionResponse


subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.batch
        [ Storage.receiveRead ReadEncrypted, Crypto.receiveDecryption DecryptedSecret ]


init : String -> String -> String -> ( Model, Cmd Msg )
init id key base_url =
    ( { id = id, key = key, lookup = Nothing, cleartext = Nothing, base_url = base_url, firstLoad = True }
    , Storage.requestRead { id = id }
    )


view : Model -> Html Msg
view model =
    case model.cleartext of
        Just cleartextValue ->
            let
                link =
                    crossOrigin model.base_url [ "d", model.id ] []
            in
            section [ class "content" ]
                [ h1 [] [ cleartextValue |> text ]
                , p []
                    [ text "Use the following link to delete this secret:"
                    , br [] []
                    , br [] []
                    , a [ href link ] [ text link ]
                    ]
                ]

        _ ->
            if model.firstLoad then
                Loading.view

            else
                NotFound.view


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        ReadEncrypted lookup ->
            ( { model | lookup = Just lookup, firstLoad = False }
            , Crypto.requestDecryption
                { key =
                    { key = model.key
                    , algorithm = lookup.algorithm
                    }
                , iv = lookup.iv
                , ciphertext = lookup.ciphertext
                }
            )

        DecryptedSecret { cleartext } ->
            ( { model | cleartext = Just cleartext }, Cmd.none )
