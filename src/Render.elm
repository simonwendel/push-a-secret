module Render exposing (render)

import Html exposing (Attribute, Html, a, div, footer, header, hr, img, node, span, text)
import Html.Attributes exposing (alt, class, href, id, src, target)


type alias RenderContent msg =
    { title : String
    , page : Html msg
    }


mainElement : List (Attribute msg) -> List (Html msg) -> Html msg
mainElement =
    node "main"


render : RenderContent msg -> List (Html msg)
render renderContent =
    [ div
        [ id "app" ]
        [ renderHeader renderContent.title
        , hr [ class "accent-dark" ] []
        , hr [ class "accent-light" ] []
        , renderPage renderContent.page
        , hr [ class "accent-light" ] []
        , hr [ class "accent-dark" ] []
        , renderFooter
        ]
    ]


renderHeader : String -> Html msg
renderHeader title =
    header
        []
        [ img [ id "logo", src "/logo.png" ] []
        , a [ id "app-title", href "/", alt "Go push a secret!" ] [ text title ]
        ]


renderPage : Html msg -> Html msg
renderPage pageContent =
    mainElement [] [ div [ id "page"] [ pageContent ] ]


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
