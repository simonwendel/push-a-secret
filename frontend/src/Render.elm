-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Render exposing (renderApp, renderContent, renderRow, setValueVisible)

import Html exposing (Attribute, Html, a, div, footer, header, hr, img, node, section, span, text)
import Html.Attributes exposing (alt, class, href, id, src, target)


type alias RenderContent msg =
    { title : String
    , page : Html msg
    }


renderContent : List (Html msg) -> Html msg
renderContent =
    section [ class "content" ]


renderRow : List (Attribute msg) -> List (Html msg) -> Html msg
renderRow attributes =
    div (class "row-of-items" :: attributes)


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
        , img
            [ class "logo"
            , class "only-on-large-screens"
            , src "/logo.png"
            , alt "A monkey covering their mouth."
            ]
            []
        , img
            [ class "logo"
            , class "only-on-small-screens"
            , src "/logo_small.png"
            , alt "Three monkeys, covering their ears, mouth, and eyes, respectively."
            ]
            []
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


setValueVisible : Bool -> Attribute msg
setValueVisible visible =
    if visible then
        class "not-hidden"

    else
        class "hidden"
