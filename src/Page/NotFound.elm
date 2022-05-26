module Page.NotFound exposing (view)

import Html.Styled exposing (Html, h1, text)


view : Html msg
view =
    h1 [] [ text "Oops, nothing here!" ]
