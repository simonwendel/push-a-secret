-- SPDX-FileCopyrightText: 2022 Simon Wendel
-- SPDX-License-Identifier: GPL-3.0-or-later


module Main exposing (main)

import Browser exposing (Document, UrlRequest(..))
import Browser.Navigation as Nav
import Html
import Page.Create as Create
import Page.Delete as Delete
import Page.NotFound as NotFound
import Page.View as View
import Render exposing (renderApp)
import Route exposing (Route(..), toRoute)
import Url exposing (Url)


type alias Flags =
    { base_url : String }


type alias Model =
    { page : Page
    , key : Nav.Key
    , base_url : String
    }


type Page
    = Create Create.Model
    | View View.Model
    | Delete Delete.Model
    | NotFound


type Msg
    = ClickedLink UrlRequest
    | ChangedUrl Url
    | GotCreateMsg Create.Msg
    | GotViewMsg View.Msg
    | GotDeleteMsg Delete.Msg


main : Program Flags Model Msg
main =
    Browser.application
        { init = init
        , subscriptions = subscriptions
        , update = update
        , view = view
        , onUrlChange = ChangedUrl
        , onUrlRequest = ClickedLink
        }


subscriptions : Model -> Sub Msg
subscriptions model =
    case model.page of
        Create createModel ->
            Create.subscriptions createModel |> Sub.map GotCreateMsg

        View viewModel ->
            View.subscriptions viewModel |> Sub.map GotViewMsg

        _ ->
            Sub.none


init : Flags -> Url -> Nav.Key -> ( Model, Cmd Msg )
init { base_url } url key =
    updateUrl url { page = NotFound, key = key, base_url = base_url }


view : Model -> Document Msg
view { page } =
    let
        title =
            "Push-a-Secret!"

        content =
            case page of
                Create createModel ->
                    Create.view createModel |> Html.map GotCreateMsg

                View viewModel ->
                    View.view viewModel |> Html.map GotViewMsg

                Delete deleteModel ->
                    Delete.view deleteModel |> Html.map GotDeleteMsg

                NotFound ->
                    NotFound.view
    in
    { title = title
    , body = renderApp { title = title, page = content }
    }


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        ClickedLink urlRequest ->
            case urlRequest of
                External url ->
                    ( model, Nav.load url )

                Internal url ->
                    ( model, Nav.pushUrl model.key (Url.toString url) )

        ChangedUrl url ->
            updateUrl url model

        GotCreateMsg createMsg ->
            case model.page of
                Create createModel ->
                    toCreate model (Create.update createMsg createModel)

                _ ->
                    ( model, Cmd.none )

        GotViewMsg viewMsg ->
            case model.page of
                View viewModel ->
                    toView model (View.update viewMsg viewModel)

                _ ->
                    ( model, Cmd.none )

        GotDeleteMsg deleteMsg ->
            case model.page of
                Delete deleteModel ->
                    toDelete model (Delete.update deleteMsg deleteModel)

                _ ->
                    ( model, Cmd.none )


toCreate : Model -> ( Create.Model, Cmd Create.Msg ) -> ( Model, Cmd Msg )
toCreate model ( createModel, cmd ) =
    ( { model | page = Create createModel }, Cmd.map GotCreateMsg cmd )


toView : Model -> ( View.Model, Cmd View.Msg ) -> ( Model, Cmd Msg )
toView model ( viewModel, cmd ) =
    ( { model | page = View viewModel }, Cmd.map GotViewMsg cmd )


toDelete : Model -> ( Delete.Model, Cmd Delete.Msg ) -> ( Model, Cmd Msg )
toDelete model ( deleteModel, cmd ) =
    ( { model | page = Delete deleteModel }, Cmd.map GotDeleteMsg cmd )


updateUrl : Url -> Model -> ( Model, Cmd Msg )
updateUrl url model =
    case toRoute url of
        Nothing ->
            ( { model | page = NotFound }, Cmd.none )

        Just CreateRoute ->
            toCreate model (Create.init model.base_url)

        Just (ViewRoute id key) ->
            toView model (View.init id key model.base_url)

        Just (DeleteRoute id) ->
            toDelete model (Delete.init id)
