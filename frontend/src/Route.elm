-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Route exposing (Route(..), Router, buildRouter, toRoute)

import Browser.Navigation as Nav
import Url exposing (Url)
import Url.Builder exposing (crossOrigin)
import Url.Parser as Parser exposing ((</>), Parser, s)


type Route
    = CreateRoute
    | ViewRoute String String
    | DeleteRoute String


view_path : String
view_path =
    "v"


delete_path : String
delete_path =
    "d"


type alias Router msg =
    { navigate : Url -> Cmd msg
    , load : String -> Cmd msg
    , viewLink : String -> String -> String
    , deleteLink : String -> String
    }


buildRouter : Nav.Key -> String -> Router msg
buildRouter navKey base_url =
    { navigate = \url -> Nav.pushUrl navKey (Url.toString url)
    , load = \url -> Nav.load url
    , deleteLink = \id -> crossOrigin base_url [ delete_path, id ] []
    , viewLink = \id key -> crossOrigin base_url [ view_path, id, key ] []
    }


route : Parser (Route -> a) a
route =
    Parser.oneOf
        [ Parser.map CreateRoute Parser.top
        , Parser.map ViewRoute (s view_path </> Parser.string </> Parser.string)
        , Parser.map DeleteRoute (s delete_path </> Parser.string)
        ]


toRoute : Url -> Maybe Route
toRoute url =
    Parser.parse route url
