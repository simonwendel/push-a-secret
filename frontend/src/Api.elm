module Api exposing (Client, Headers, client)

import Dict exposing (Dict)
import Http exposing (Expect)
import Json.Decode as D exposing (Decoder)
import Json.Encode as E


type alias Headers =
    Dict String String


type alias RequestExpectEmpty msg =
    (Result Http.Error ( (), Headers ) -> msg) -> Cmd msg


type alias RequestExpectJson msg a =
    Decoder a -> (Result Http.Error ( a, Headers ) -> msg) -> Cmd msg


type alias Client msg a =
    { head : String -> RequestExpectEmpty msg
    , get : String -> RequestExpectJson msg a
    , post : E.Value -> RequestExpectJson msg a
    , delete : String -> RequestExpectEmpty msg
    }


client : Client msg a
client =
    buildClient "http://localhost:5000"


buildClient : String -> Client msg a
buildClient baseUrl =
    let
        makeUrl =
            (++) <| baseUrl ++ "/secret/"
    in
    { head = \id msg -> headRequest (makeUrl id) msg
    , get = \id decoder msg -> getRequest (makeUrl id) decoder msg
    , post = \secret decoder msg -> postRequest (makeUrl "") secret decoder msg
    , delete = \id msg -> deleteRequest (makeUrl id) msg
    }


headRequest : String -> RequestExpectEmpty msg
headRequest url msg =
    makeRequest
        { url = url
        , method = "HEAD"
        , body = Http.emptyBody
        , expect = responseWithMetadata msg ignoreBody
        }


getRequest : String -> RequestExpectJson msg a
getRequest url decoder msg =
    Http.get
        { url = url
        , expect = responseWithMetadata msg (parseJsonBody decoder)
        }


postRequest : String -> E.Value -> RequestExpectJson msg a
postRequest url value decoder msg =
    Http.post
        { url = url
        , body = Http.jsonBody value
        , expect = responseWithMetadata msg (parseJsonBody decoder)
        }


deleteRequest : String -> RequestExpectEmpty msg
deleteRequest url msg =
    makeRequest
        { url = url
        , method = "DELETE"
        , body = Http.emptyBody
        , expect = responseWithMetadata msg ignoreBody
        }


makeRequest : { url : String, expect : Expect msg, method : String, body : Http.Body } -> Cmd msg
makeRequest params =
    Http.request
        { url = params.url
        , expect = params.expect
        , method = params.method
        , body = params.body
        , headers = []
        , timeout = Nothing
        , tracker = Nothing
        }


ignoreBody : Http.Metadata -> String -> Result error ( (), Headers )
ignoreBody metadata _ =
    Ok ( (), metadata.headers )


parseJsonBody : Decoder a -> Http.Metadata -> String -> Result Http.Error ( a, Headers )
parseJsonBody decoder metadata body =
    case D.decodeString decoder body of
        Ok decoded ->
            Ok ( decoded, metadata.headers )

        Err err ->
            Err (Http.BadBody (D.errorToString err))


responseWithMetadata : (Result Http.Error ( a, Headers ) -> msg) -> (Http.Metadata -> String -> Result Http.Error ( a, Headers )) -> Expect msg
responseWithMetadata msg parse =
    Http.expectStringResponse msg <|
        \response ->
            case response of
                Http.BadUrl_ url ->
                    Err (Http.BadUrl url)

                Http.Timeout_ ->
                    Err Http.Timeout

                Http.NetworkError_ ->
                    Err Http.NetworkError

                Http.BadStatus_ metadata _ ->
                    Err (Http.BadStatus metadata.statusCode)

                Http.GoodStatus_ metadata body ->
                    parse metadata body
