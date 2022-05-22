module Main exposing (main)

import Browser exposing (Document)
import Html exposing (Html, text)


type alias Model =
    { page : Page }


type Msg
    = Nothing


type Page
    = Home
    | New
    | View
    | Delete


view : Model -> Document Msg
view model =
    { title = "Decerno Secret Push"
    , body = [ text "Cool beans!" ]
    }


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    ( model, Cmd.none )


main : Program () Model Msg
main =
    Browser.document
        { init = \_ -> ( { page = Home }, Cmd.none )
        , subscriptions = \_ -> Sub.none
        , update = update
        , view = view
        }
