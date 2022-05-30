module Page.Loading exposing (view)

import Html exposing (Html, div, img)
import Html.Attributes exposing (id, src)
import Render exposing (renderContent)


view : Html msg
view =
    renderContent
        [ div [ id "spinner" ]
            [ div []
                [ img [ src "/loading.gif" ] []
                ]
            ]
        ]
