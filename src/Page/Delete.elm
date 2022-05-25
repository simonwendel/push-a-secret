module Page.Delete exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Html exposing (Html, button, div, h1, p, text)
import Html.Events exposing (onClick)
import Page.NotFound as NotFound
import Storage


type alias Model =
    { id : Maybe String
    , exists : Maybe Bool
    , pleaseDelete : Maybe Bool
    , deleted : Bool
    }


type Msg
    = DoDelete
    | DontDelete
    | ReceivedCheck Storage.CheckResponse
    | ReceivedDeletion Storage.DeletionResponse


init : Maybe String -> ( Model, Cmd Msg )
init id =
    ( { id = id, pleaseDelete = Nothing, exists = Nothing, deleted = False }
    , case id of
        Just idValue ->
            Storage.requestCheck { id = idValue }

        Nothing ->
            Cmd.none
    )


subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.batch
        [ Storage.receiveCheck ReceivedCheck
        , Storage.receiveDeletion ReceivedDeletion
        ]


view : Model -> Html Msg
view model =
    h1 []
        [ case ( model.pleaseDelete, model.exists, model.deleted ) of
            ( Nothing, Just True, False ) ->
                div []
                    [ h1 [] [ text "Delete secret?" ]
                    , p [] [ "Are you sure you want to delete this secret?" |> text ]
                    , p []
                        [ button [ onClick DoDelete ] [ text "Yes!" ]
                        , button [ onClick DontDelete ] [ text "No!" ]
                        ]
                    ]

            ( Just True, Just True, True ) ->
                div []
                    [ h1 [] [ text "Secret Deleted!" ] ]

            ( Just False, Just True, _ ) ->
                div []
                    [ h1 [] [ text "Secret Not Deleted!" ] ]

            _ ->
                NotFound.view
        ]


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        DoDelete ->
            ( { model | pleaseDelete = Just True }
            , case model.id of
                Just idValue ->
                    Storage.requestDeletion { id = idValue }

                Nothing ->
                    Cmd.none
            )

        DontDelete ->
            ( { model | pleaseDelete = Just False }, Cmd.none )

        ReceivedCheck { exists } ->
            ( { model | exists = Just exists }, Cmd.none )

        ReceivedDeletion { success } ->
            ( { model | deleted = success }, Cmd.none )
