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
import Page.Loading as Loading
import Page.NotFound as NotFound
import Render exposing (renderContent, renderRow)
import Storage


type alias Model =
    { id : String
    , exists : Maybe Bool
    , pleaseDelete : Maybe Bool
    , deleted : Bool
    , firstLoad : Bool
    }


type Msg
    = DoDelete
    | DontDelete
    | ReceivedCheck Storage.CheckResponse
    | ReceivedDeletion Storage.DeletionResponse


init : String -> ( Model, Cmd Msg )
init id =
    ( { id = id, pleaseDelete = Nothing, exists = Nothing, deleted = False, firstLoad = True }
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
                , p [] [ text "Are you sure you want to delete this secret?" ]
                , renderRow
                    [ button [ onClick DoDelete, class "ok" ] [ text "✔" ]
                    , button [ onClick DontDelete, class "cancel" ] [ text "✖" ]
                    ]
                ]

        ( Just True, Just True, True ) ->
            renderContent
                [ h1 []
                    [ text "Secret deleted" ]
                , p []
                    [ text
                        """
                        Any old view link to this secret has now stopped working and we can't retrieve it for you,
                        it would be pretty bad if we could.
                        """
                    ]
                ]

        ( Just False, Just True, _ ) ->
            renderContent
                [ h1 []
                    [ text "Secret not deleted" ]
                , p []
                    [ text
                        """
                        The secret is still in our database and any view link created for it should still work.
                        We really hope you saved the link, or the key is lost forever.
                        """
                    ]
                ]

        _ ->
            if model.firstLoad then
                Loading.view

            else
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
            ( { model | exists = Just exists, firstLoad = False }, Cmd.none )

        ReceivedDeletion { success } ->
            ( { model | deleted = success }, Cmd.none )
