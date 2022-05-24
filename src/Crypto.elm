port module Crypto exposing
    ( EncryptedValue
    , EncryptionRequest
    , Key
    , receiveEncrypted
    , receiveKey
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


type alias EncryptedValue =
    { key : Key
    , iv : String
    , ciphertext : String
    }


port requestKey : () -> Cmd msg


port receiveKey : (Key -> msg) -> Sub msg


port requestEncryption : EncryptionRequest -> Cmd msg


port receiveEncrypted : (EncryptedValue -> msg) -> Sub msg
