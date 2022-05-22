module Page.Home exposing (Model, Msg, init, update, view)

import Html exposing (Html, button, div, h1, input, p, text)
import Html.Attributes exposing (type_)
import Html.Events exposing (onClick, onInput)


type alias Model =
    { secretVisible : Bool, secretValue : String }


type Msg
    = ToggleVisibility
    | UpdateSecretValue String
    | CreateSecret


init : () -> ( Model, Cmd Msg )
init () =
    ( { secretVisible = False, secretValue = "" }, Cmd.none )


view : Model -> Html Msg
view { secretVisible } =
    div []
        [ h1 [] [ text "Create a new secret!" ]
        , p []
            [ input
                [ if secretVisible then
                    type_ "text"

                  else
                    type_ "password"
                ]
                []
            , button [ onClick ToggleVisibility, onInput UpdateSecretValue ] [ text "Show" ]
            , button [ onClick CreateSecret ] [ text "Create" ]
            ]
        ]


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        ToggleVisibility ->
            ( { model | secretVisible = not model.secretVisible }, Cmd.none )

        UpdateSecretValue newValue ->
            ( { model | secretValue = newValue }, Cmd.none )

        CreateSecret ->
            Debug.todo "branch 'CreateSecret _' not implemented"
