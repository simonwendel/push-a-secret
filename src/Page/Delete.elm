module Page.Delete exposing
    ( Model
    , Msg
    , init
    , subscriptions
    , update
    , view
    )

import Html exposing (Html, button, h1, p, text)
import Html.Attributes exposing (class)
import Html.Events exposing (onClick)
import Page.NotFound as NotFound
import Render exposing (renderContent, renderRow)
import Storage


type alias Model =
    { id : String
    , exists : Maybe Bool
    , pleaseDelete : Maybe Bool
    , deleted : Bool
    }


type Msg
    = DoDelete
    | DontDelete
    | ReceivedCheck Storage.CheckResponse
    | ReceivedDeletion Storage.DeletionResponse


init : String -> ( Model, Cmd Msg )
init id =
    ( { id = id, pleaseDelete = Nothing, exists = Nothing, deleted = False }
    , Storage.requestCheck { id = id }
    )


subscriptions : Model -> Sub Msg
subscriptions _ =
    Sub.batch
        [ Storage.receiveCheck ReceivedCheck
        , Storage.receiveDeletion ReceivedDeletion
        ]


view : Model -> Html Msg
view model =
    case ( model.pleaseDelete, model.exists, model.deleted ) of
        ( Nothing, Just True, False ) ->
            renderContent
                [ h1 [] [ text "Delete secret?" ]
                , p [] [ "Are you sure you want to delete this secret?" |> text ]
                , renderRow
                    [ button [ onClick DoDelete, class "ok" ] [ text "✔" ]
                    , button [ onClick DontDelete, class "cancel" ] [ text "✖" ]
                    ]
                ]

        ( Just True, Just True, True ) ->
            renderContent
                [ h1 []
                    [ text "Secret Deleted!" ]
                ]

        ( Just False, Just True, _ ) ->
            renderContent
                [ h1 []
                    [ text "Secret Not Deleted!" ]
                ]

        _ ->
            NotFound.view


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        DoDelete ->
            ( { model | pleaseDelete = Just True }
            , Storage.requestDeletion { id = model.id }
            )

        DontDelete ->
            ( { model | pleaseDelete = Just False }, Cmd.none )

        ReceivedCheck { exists } ->
            ( { model | exists = Just exists }, Cmd.none )

        ReceivedDeletion { success } ->
            ( { model | deleted = success }, Cmd.none )
