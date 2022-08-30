module Aria exposing (ariaDescription, ariaLabel)

import Html exposing (Attribute)
import Html.Attributes exposing (attribute)


ariaLabel : String -> Attribute msg
ariaLabel =
    attribute "aria-label"


ariaDescription : String -> Attribute msg
ariaDescription =
    attribute "aria-description"
