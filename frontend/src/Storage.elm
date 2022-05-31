port module Storage exposing
    ( PeekRequest
    , PeekResponse
    , DeleteRequest
    , DeleteResponse
    , ReadRequest
    , ReadResponse
    , CreateRequest
    , CreateResponse
    , receivePeek
    , receiveDelete
    , receiveRead
    , receiveCreate
    , requestPeek
    , requestDelete
    , requestRead
    , requestCreate
    )


type alias CreateRequest =
    { algorithm : String
    , iv : String
    , ciphertext : String
    }


type alias CreateResponse =
    { id : String }


type alias ReadRequest =
    CreateResponse


type alias ReadResponse =
    CreateRequest


type alias DeleteRequest =
    { id : String }


type alias DeleteResponse =
    { success : Bool }


type alias PeekRequest =
    CreateResponse


type alias PeekResponse =
    { exists : Bool }


port requestCreate : CreateRequest -> Cmd msg


port receiveCreate : (CreateResponse -> msg) -> Sub msg


port requestRead : ReadRequest -> Cmd msg


port receiveRead : (ReadResponse -> msg) -> Sub msg


port requestDelete : DeleteRequest -> Cmd msg


port receiveDelete : (DeleteResponse -> msg) -> Sub msg


port requestPeek : PeekRequest -> Cmd msg


port receivePeek : (PeekResponse -> msg) -> Sub msg
