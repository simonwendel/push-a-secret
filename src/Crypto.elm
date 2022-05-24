port module Crypto exposing
    ( DecryptionRequest
    , EncryptionRequest
    , EncryptionResponse
    , Key
    , receiveDecryption
    , receiveEncryption
    , receiveKey
    , requestDecryption
    , requestEncryption
    , requestKey
    )


type alias Key =
    { algorithm : String
    , key : String
    }


type alias EncryptionRequest =
    { key : Key
    , cleartext : String
    }


type alias EncryptionResponse =
    { iv : String
    , ciphertext : String
    }


type alias DecryptionRequest =
    EncryptionResponse


type alias DecryptionResponse =
    { cleartext : String }


port requestKey : () -> Cmd msg


port receiveKey : (Key -> msg) -> Sub msg


port requestEncryption : EncryptionRequest -> Cmd msg


port receiveEncryption : (EncryptionResponse -> msg) -> Sub msg


port requestDecryption : DecryptionRequest -> Cmd msg


port receiveDecryption : (DecryptionResponse -> msg) -> Sub msg
