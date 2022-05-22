module Page.Create exposing (view)

import Html exposing (Html, h1, text)


view : Html msg
view =
    h1 [] [ text "Created a new secret!" ]
