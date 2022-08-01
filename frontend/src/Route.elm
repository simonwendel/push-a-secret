module Route exposing (Route(..), delete_path, toRoute, view_path)

import Url exposing (Url)
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
