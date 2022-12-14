module Elmish.UrlParserTests

open Swensen.Unquote
open Elmish.UrlParser

type Routes =
  | Empty
  | Backlash
  | IntAndOneStringOptions of int * string option
  | IntAndTwoStringOptions of int * string option * string option

let curry2 f a b = f (a,b)
let curry3 f a b c = f (a,b,c)

open NUnit.Framework
[<Test>]
let ``parses subroute and multiple params`` () =
  let parser = s "id" </> i32 <?> stringParam "one" <?> stringParam "two" |> map (curry3 IntAndTwoStringOptions)
  parseUrl parser "id/123?one=&two=" =! Some (IntAndTwoStringOptions (123,Some "",Some ""))
  Assert.Pass()

[<Test>]
let ``parses subroute and one params`` () =
  let parser = s "id" </> i32 <?> stringParam "one" |> map (curry2 IntAndOneStringOptions)
  parseUrl parser "id/123?one=" =! Some (IntAndOneStringOptions (123,Some ""))
  Assert.Pass()


//TODO: Implement failing test
[<Test>]
let ``parses backslash as subroute`` () =
  let parser = s "/" |> map Backlash
  parseUrl parser "/" =! Some Backlash
  Assert.Pass()

[<Test>]
let ``parses empty string as subroute`` () =
  let parser = s "" |> map Empty
  parseUrl parser "" =! Some Empty
  Assert.Pass()