module Main exposing (main)

import Browser exposing (Document, UrlRequest(..))
import Browser.Navigation as Nav
import Html exposing (text)
import Url exposing (Url)
import Url.Parser as Parser exposing ((</>), Parser, s)


type alias Model =
    { page : Page, key : Nav.Key }


type Msg
    = ClickedLink UrlRequest
    | ChangedUrl Url


type Page
    = Home
    | Create
    | View String
    | Delete String
    | NotFound


route : Parser (Page -> a) a
route =
    Parser.oneOf
        [ Parser.map Home Parser.top
        , Parser.map Create (s "create")
        , Parser.map View (s "view" </> Parser.string)
        , Parser.map Delete (s "delete" </> Parser.string)
        ]


toRoute : Url -> Page
toRoute url =
    Parser.parse route url |> Maybe.withDefault NotFound


init : () -> Url -> Nav.Key -> ( Model, Cmd Msg )
init _ url key =
    ( { page = toRoute url, key = key }, Cmd.none )


view : Model -> Document Msg
view { page } =
    case page of
        Home ->
            { title = "Decerno Secret Pusher"
            , body = [ text "Welcome!" ]
            }

        Create ->
            { title = "Push a Secret"
            , body = [ text "Create a new secret!" ]
            }

        View key ->
            { title = "View a Secret"
            , body = [ "Viewing secret with key: " ++ key |> text ]
            }

        Delete key ->
            { title = "Delete a Secret"
            , body = [ "Deleting secret with key: " ++ key |> text ]
            }

        NotFound ->
            { title = "Not Found"
            , body = [ text "Oops, nothing here!" ]
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
            ( { model | page = toRoute url }, Cmd.none )


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
