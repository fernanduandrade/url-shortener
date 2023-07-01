module Hash

open Hashids

let generateNewHash() =
    let config = 
            HashidConfiguration.create 
                        { HashidConfiguration.defaultOptions with Salt = System.Guid.NewGuid().ToString() }
    let encode = Hashid.encode64 config
    let hashId = encode [| 73L; 88L |]
    hashId