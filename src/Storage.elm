port module Storage exposing
    ( CheckRequest
    , CheckResponse
    , DeletionRequest
    , DeletionResponse
    , LookupRequest
    , LookupResponse
    , StorageRequest
    , StorageResponse
    , receiveCheck
    , receiveDeletion
    , receiveLookup
    , receiveStorage
    , requestCheck
    , requestDeletion
    , requestLookup
    , requestStorage
    )


type alias StorageRequest =
    { algorithm : String
    , iv : String
    , ciphertext : String
    }


type alias StorageResponse =
    { id : String }


type alias LookupRequest =
    StorageResponse


type alias LookupResponse =
    StorageRequest


type alias DeletionRequest =
    { id : String }


type alias DeletionResponse =
    { success : Bool }


type alias CheckRequest =
    DeletionRequest


type alias CheckResponse =
    { exists : Bool }


port requestStorage : StorageRequest -> Cmd msg


port receiveStorage : (StorageResponse -> msg) -> Sub msg


port requestLookup : LookupRequest -> Cmd msg


port receiveLookup : (LookupResponse -> msg) -> Sub msg


port requestDeletion : DeletionRequest -> Cmd msg


port receiveDeletion : (DeletionResponse -> msg) -> Sub msg


port requestCheck : CheckRequest -> Cmd msg


port receiveCheck : (CheckResponse -> msg) -> Sub msg
