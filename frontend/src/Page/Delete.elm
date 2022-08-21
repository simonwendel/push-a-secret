-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Page.Delete exposing
    ( Model
    , Msg
    , init
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
    | Checked Bool
    | Deleted Bool


init : String -> ( Model, Cmd Msg )
init id =
    ( { id = id
      , pleaseDelete = Nothing
      , exists = Nothing
      , deleted = False
      , firstLoad = True
      }
    , Storage.check id Checked
    )


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
            , Storage.delete model.id Deleted
            )

        DontDelete ->
            ( { model | pleaseDelete = Just False }, Cmd.none )

        Checked results ->
            ( { model | exists = Just results, firstLoad = False }, Cmd.none )

        Deleted results ->
            ( { model | deleted = results }, Cmd.none )
