-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Render exposing (renderApp, renderContent, renderRow)

import Html exposing (Html, a, div, footer, header, hr, img, node, section, span, text)
import Html.Attributes exposing (alt, class, href, id, src, target)


type alias RenderContent msg =
    { title : String
    , page : Html msg
    }


renderContent : List (Html msg) -> Html msg
renderContent =
    section [ class "content" ]


renderRow : List (Html msg) -> Html msg
renderRow =
    div [ class "row-of-items" ]


renderApp : RenderContent msg -> List (Html msg)
renderApp content =
    [ renderHeader content.title
    , hr [ class "accent-1" ] []
    , hr [ class "accent-2" ] []
    , renderPage content.page
    , hr [ class "accent-2" ] []
    , hr [ class "accent-1" ] []
    , renderFooter
    ]


renderHeader : String -> Html msg
renderHeader title =
    header
        []
        [ a [ id "app-title", href "/", alt "Go push a secret!" ] [ text title ]
        , img [ id "logo", src "/logo.png" ] []
        ]


renderPage : Html msg -> Html msg
renderPage page =
    node "main" [] [ page ]


renderFooter : Html msg
renderFooter =
    footer
        []
        [ span [] [ text "Made with " ]
        , span
            [ id "footer-symbol" ]
            [ a [ href "https://elm-lang.org/", target "_blank" ] [ text "♡" ] ]
        , span [] [ text " by Simon Wendel!" ]
        ]
