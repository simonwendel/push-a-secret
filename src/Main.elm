module Main exposing (main)

import Browser exposing (Document, UrlRequest(..))
import Browser.Navigation as Nav
import Html exposing (text)
import Page.Home as Home exposing (..)
import Route exposing (Route(..), toRoute)
import Tuple exposing (first)
import Url exposing (Url)


type alias Model =
    { page : Page, key : Nav.Key }


type Page
    = Home Home.Model
    | Create
    | View String
    | Delete String
    | NotFound


type Msg
    = ClickedLink UrlRequest
    | ChangedUrl Url
    | GotHomeMsg Home.Msg


init : () -> Url -> Nav.Key -> ( Model, Cmd Msg )
init _ url key =
    ( { page = getPage url, key = key }, Cmd.none )


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
            View id

        Just (DeleteRoute id) ->
            Delete id


view : Model -> Document Msg
view { page } =
    let
        content =
            case page of
                Home model ->
                    Home.view model |> Html.map GotHomeMsg

                Create ->
                    text "Create a new secret!"

                View id ->
                    "Viewing secret with key: " ++ id |> text

                Delete id ->
                    "Deleting secret with key: " ++ id |> text

                NotFound ->
                    text "Oops, nothing here!"
    in
    { title = "Push a Secret"
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
