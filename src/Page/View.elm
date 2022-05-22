module Page.View exposing (Model, init, view)

import Html exposing (Html, div, h1, text)


type alias Model =
    { id : String }


init : String -> ( Model, Cmd msg )
init id =
    ( { id = id }, Cmd.none )


view : Model -> Html msg
view { id } =
    div []
        [ h1 [] [ "Viewing secret with key: " ++ id |> text ]
        ]
