module Route exposing (Route(..), toRoute)

import Url exposing (Url)
import Url.Parser as Parser exposing ((</>), Parser, s)


type Route
    = CreateRoute
    | ViewRoute String String
    | DeleteRoute String


route : Parser (Route -> a) a
route =
    Parser.oneOf
        [ Parser.map CreateRoute Parser.top
        , Parser.map ViewRoute (s "v" </> Parser.string </> Parser.string)
        , Parser.map DeleteRoute (s "d" </> Parser.string)
        ]


toRoute : Url -> Maybe Route
toRoute url =
    Parser.parse route url
