port module Crypto exposing (KeyModel, receiveKey, requestKey)


type alias KeyModel =
    { algorithm : String, key : String }


port requestKey : () -> Cmd msg


port receiveKey : (KeyModel -> msg) -> Sub msg
