module Route exposing (Route(..), toRoute)

import Url exposing (Url)
import Url.Parser as Parser exposing ((</>), Parser, s)


type Route
    = HomeRoute
    | CreateRoute
    | ViewRoute String
    | DeleteRoute String


route : Parser (Route -> a) a
route =
    Parser.oneOf
        [ Parser.map HomeRoute Parser.top
        , Parser.map CreateRoute (s "create")
        , Parser.map ViewRoute (s "view" </> Parser.string)
        , Parser.map DeleteRoute (s "delete" </> Parser.string)
        ]


toRoute : Url -> Maybe Route
toRoute url =
    Parser.parse route url
