module Hash

open Hashids

type Hash() =
    static let config = 
        HashidConfiguration.create 
                    { HashidConfiguration.defaultOptions with Salt = System.Guid.NewGuid().ToString() }
    
    static member GenerateNewHash () =
        let encode = Hashid.encode64 config
        let hashId = encode [| 73L; 88L |]
        hashId