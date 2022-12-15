module Elmish.UrlParserTests

// open FsCheck
// open FsCheck.NUnit
// open System.Collections.Generic
open Swensen.Unquote
open Elmish.UrlParser

type Routes =
  IntAndTwoStringOptions of int * string option * string option

let curry3 f a b c = f (a,b,c)

open NUnit.Framework
[<Test>]
let ``parses subroute and multiple params`` () =
  let parser = s "id" </> i32 <?> stringParam "one" <?> stringParam "two" |> map (curry3 IntAndTwoStringOptions)
  parseUrl parser "id/123?one=&two=" =! Some (IntAndTwoStringOptions (123,Some "",Some ""))