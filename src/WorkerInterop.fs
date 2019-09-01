// Common JS Interop code and utility functions

module WorkersInterop
open System
open System.Text.RegularExpressions

open Fable.Core
open Fable.Import.Browser
open Fable.Import.JS



[<Emit("addEventListener('fetch', $0)")>]
let addEventListener (e:FetchEvent->Promise<Response>) : unit = jsNative

[<Emit("new Response($0, {status: $1})")>]
let newResponse (a:string) (b:string) : Response = jsNative

type CFWRequest =
    inherit Request
    abstract member cf : CFDetails
and CFDetails = {
    tlsVersion: string
    tlsCipher: string
    country: string
    colo: string
}

let wrap x = promise {return x}

let path (r:CFWRequest)=
    match Regex.Split((Uri r.url).AbsolutePath.ToLower(), "\/") with
    | [|"";""|] -> [||]   // this is for paths http://somthing.com/ and http://something.com
    | p -> p.[1..]        // becuase the first path element is always ""
    |> List.ofArray

let textResponse txt =
  newResponse txt "200" |> wrap


type Verb =
    | GET
    | POST
    | PUT
    | PATCH
    | DELETE
    | OPTION
    | HEAD
    | TRACE
    | CONNECT
    | UNDEFINED

let verb (r:CFWRequest) =
    match r.method.ToUpper() with
    | "GET" -> GET
    | "POST" -> POST
    | "PUT" -> PUT
    | "PATCH" -> PATCH
    | "DELETE" -> DELETE
    | "OPTION" -> OPTION
    | "HEAD" -> HEAD
    | "TRACE" -> TRACE
    | "CONNECT" -> CONNECT
    | _ -> UNDEFINED

type KeyQuery = {
  prefix: string
  cursor: string option
}

type Key = {
  name: string
  expiration: int option
}

type KeyQueryResponse1 = {
  keys: Key list
  list_complete: bool
  cursor: string option
}

type ResultInfo = {
  count: int
  cursor: string option
}


type KeyQueryResponse = {
  result: Key list
  success: bool
  errors: string list
  messages: string list
  result_info: ResultInfo
}


[<Emit("KVBinding.list()")>]
let kvKeyListing() : Promise<KeyQueryResponse> = jsNative

[<Emit("KVBinding.list($0)")>]
let kvKeyQuery() : Promise<string> = jsNative

[<Emit("KVBinding.get($0)")>]
let kvGet(key:string) : Promise<string option> = jsNative

[<Emit("KVBinding.put($0,$1)")>]
let kvPut(key:string) (value:string) : Promise<unit> = jsNative

[<Emit("KVBinding.delete($0)")>]
let kvDelete(key:string) : Promise<unit> = jsNative

