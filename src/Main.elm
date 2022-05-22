module Main exposing (main)

import Browser exposing (Document, UrlRequest(..))
import Browser.Navigation as Nav
import Html exposing (text)
import Page.Create as Create exposing (view)
import Page.Delete as Delete exposing (Model, init, view)
import Page.Home as Home exposing (Model, Msg, init, update, view)
import Page.NotFound as NotFound exposing (view)
import Page.View as View exposing (Model, init, view)
import Route exposing (Route(..), toRoute)
import Tuple exposing (first)
import Url exposing (Url)


type alias Model =
    { page : Page, key : Nav.Key }


type Page
    = Home Home.Model
    | Create
    | View View.Model
    | Delete Delete.Model
    | NotFound


type Msg
    = ClickedLink UrlRequest
    | ChangedUrl Url
    | GotHomeMsg Home.Msg


main : Program () Model Msg
main =
    Browser.application
        { init = init
        , subscriptions = \_ -> Sub.none
        , update = update
        , view = view
        , onUrlChange = ChangedUrl
        , onUrlRequest = ClickedLink
        }


init : () -> Url -> Nav.Key -> ( Model, Cmd Msg )
init _ url key =
    ( { page = getPage url, key = key }, Cmd.none )


view : Model -> Document Msg
view { page } =
    let
        content =
            case page of
                Home viewModel ->
                    Home.view viewModel |> Html.map GotHomeMsg

                Create ->
                    Create.view

                View viewModel ->
                    View.view viewModel

                Delete viewModel ->
                    Delete.view viewModel

                NotFound ->
                    NotFound.view
    in
    { title = "Push-a-Secret"
    , body = [ content ]
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
            ( { model | page = getPage url }, Cmd.none )

        GotHomeMsg homeMsg ->
            case model.page of
                Home homeModel ->
                    toHome model (Home.update homeMsg homeModel)

                _ ->
                    ( model, Cmd.none )


toHome : Model -> ( Home.Model, Cmd Home.Msg ) -> ( Model, Cmd Msg )
toHome model ( home, cmd ) =
    ( { model | page = Home home }, Cmd.map GotHomeMsg cmd )


getPage : Url -> Page
getPage url =
    case toRoute url of
        Nothing ->
            NotFound

        Just HomeRoute ->
            Home (Home.init () |> first)

        Just CreateRoute ->
            Create

        Just (ViewRoute id) ->
            View (View.init id |> first)

        Just (DeleteRoute id) ->
            Delete (Delete.init id |> first)
