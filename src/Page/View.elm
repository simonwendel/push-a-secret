module Page.View exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Html exposing (Html, h1, text)
import Page.NotFound as NotFound
import Storage


type alias Model =
    { id : Maybe String
    , key : Maybe String
    , lookup : Maybe Storage.LookupResponse
    }


type Msg
    = ReceivedLookup Storage.LookupResponse


subscriptions : Model -> Sub Msg
subscriptions _ =
    Storage.receiveLookup ReceivedLookup


init : Maybe String -> Maybe String -> ( Model, Cmd msg )
init id key =
    ( { id = id, key = key, lookup = Nothing }
    , case id of
        Just idValue ->
            Storage.requestLookup { id = idValue }

        Nothing ->
            Cmd.none
    )


view : Model -> Html msg
view model =
    h1 []
        [ case model.lookup of
            Just lookupValue ->
                "Super secret stuff: (" ++ lookupValue.iv ++ ", " ++ lookupValue.ciphertext ++ ")" |> text

            _ ->
                NotFound.view
        ]


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        ReceivedLookup lookup ->
            ( { model | lookup = Just lookup }, Cmd.none )
