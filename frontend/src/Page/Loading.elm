-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Page.Loading exposing (view)

import Html exposing (Html, div, span, text)
import Html.Attributes exposing (id)
import Render exposing (renderContent)


view : Html msg
view =
    renderContent
        [ div [ id "loader" ]
            [ div []
                [ span [] [ text "Loading..." ]
                ]
            ]
        ]
