module Hash

open Hashids

let config = 
        HashidConfiguration.create 
                    { HashidConfiguration.defaultOptions with Salt = System.Guid.NewGuid().ToString() }

let generateNewHash =
    let encode = Hashid.encode64 config
    let hashId = encode [| 73L; 88L |]
    hashId