module Render exposing (render)

import Css exposing (..)
import Html exposing (Html)
import Html.Styled as Styled exposing (a, div, footer, header, img, p, span, text)
import Html.Styled.Attributes exposing (css, href, src, target)


type alias RenderContent msg =
    { title : String
    , page : Styled.Html msg
    }


render : RenderContent msg -> List (Styled.Html msg)
render renderContent =
    let
        headerSettings =
            { logo =
                { height = px 40 }
            , title =
                { fontSize = rem 3.5
                }
            , padding =
                { vertical = px 40
                , horizontal = px 90
                }
            }

        footerSettings =
            { fontSize = rem 1.0
            , symbolSize = rem 2.0
            }
    in
    [ renderHeader renderContent.title headerSettings
    , renderPage renderContent.page
    , renderFooter footerSettings
    ]


type alias HeaderSettings a b c d =
    { logo :
        { height : ExplicitLength a
        }
    , title :
        { fontSize : ExplicitLength b }
    , padding :
        { vertical : ExplicitLength c
        , horizontal : ExplicitLength d
        }
    }


renderHeader : String -> HeaderSettings a b c d -> Styled.Html msg
renderHeader title headerSettings =
    header
        [ css
            [ paddingBottom headerSettings.padding.vertical
            , paddingTop headerSettings.padding.vertical
            , paddingLeft headerSettings.padding.horizontal
            , paddingRight headerSettings.padding.horizontal
            , width (pct 100)
            ]
        ]
        [ div []
            [ img
                [ src "/logo.png"
                , css
                    [ height headerSettings.logo.height
                    ]
                ]
                []
            ]
        , div
            [ css
                [ width (pct 100)
                , textAlign center
                ]
            ]
            [ span
                [ css
                    [ fontSize headerSettings.title.fontSize
                    , fontFamilies
                        [ "Courier New"
                        , "Courier"
                        , "monospace"
                        ]
                    ]
                ]
                [ text title ]
            ]
        ]


renderPage : Styled.Html msg -> Styled.Html msg
renderPage pageContent =
    div
        [ css
            [ backgroundColor (hex "#e8e8e8")
            ]
        ]
        [ pageContent ]


type alias FooterSettings a b =
    { fontSize : ExplicitLength a
    , symbolSize : ExplicitLength b
    }


renderFooter : FooterSettings a b -> Styled.Html msg
renderFooter footerSettings =
    footer
        [ css
            [ width (pct 100)
            , textAlign center
            , bottom (px 0)
            , left (px 0)
            , position fixed
            ]
        ]
        [ span
            [ css
                [ fontSize footerSettings.fontSize
                , lineHeight footerSettings.symbolSize
                , verticalAlign middle
                ]
            ]
            [ text "Made with " ]
        , span
            [ css
                [ fontSize footerSettings.symbolSize
                , fontWeight bold
                , verticalAlign middle
                ]
            ]
            [ a [ href "https://elm-lang.org/", Html.Styled.Attributes.target "_blank", css [ textDecoration none ] ] [ text "♡" ] ]
        , span
            [ css
                [ fontSize footerSettings.fontSize
                , lineHeight footerSettings.symbolSize
                , verticalAlign middle
                ]
            ]
            [ text " by Simon Wendel" ]
        ]
