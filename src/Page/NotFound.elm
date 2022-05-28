module Page.NotFound exposing (view)

import Render exposing (renderContent)
import Html exposing (Html, h1, text)

view : Html msg
view =
    renderContent
        [ h1 []
            [ text "Oops, nothing here!"
            ]
        ]
