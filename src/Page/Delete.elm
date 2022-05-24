module Page.Delete exposing
    ( Model
    , Msg
    , init
    , update
    , view
    )

import Html exposing (Html, h1, text)


type alias Model =
    { key : Maybe String
    , secret : Maybe String
    , encrypted : Maybe String
    }


type Msg
    = NoMsg


init : Maybe String -> ( Model, Cmd msg )
init key =
    ( { key = key, secret = Nothing, encrypted = Nothing }, Cmd.none )


view : Model -> Html msg
view model =
    h1 []
        [ case model.key of
            Just key ->
                "Super secret key: " ++ key |> text

            Nothing ->
                "No key!" |> text
        ]


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        NoMsg ->
            ( model, Cmd.none )
