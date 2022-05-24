port module Storage exposing
    ( LookupRequest
    , LookupResponse
    , StorageRequest
    , StorageResponse
    , receiveStorage
    , requestStorage
    , requestLookup
    , receiveLookup
    )


type alias StorageRequest =
    { iv : String
    , ciphertext : String
    }


type alias StorageResponse =
    { id : String }


type alias LookupRequest =
    StorageResponse


type alias LookupResponse =
    StorageRequest


port requestStorage : StorageRequest -> Cmd msg


port receiveStorage : (StorageResponse -> msg) -> Sub msg


port requestLookup : LookupRequest -> Cmd msg


port receiveLookup : (LookupResponse -> msg) -> Sub msg
