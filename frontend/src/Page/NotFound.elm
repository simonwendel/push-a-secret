-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Page.NotFound exposing (view)

import Html exposing (Html, a, h1, p, text)
import Html.Attributes exposing (href)
import Render exposing (renderContent)


view : Html msg
view =
    renderContent
        [ h1 []
            [ text "Oops, nothing here!"
            ]
        , p []
            [ text
                """
                There is nothing here. Either you entered a faulty URL or you tried to access some secret
                you don't have the key for.
                """
            ]
        , p []
            [ text "Either way, your best bet is to try something different or "
            , a [ href "/" ] [ text "create a secret" ]
            , text " of your own to proudly give to friends..."
            ]
        ]
