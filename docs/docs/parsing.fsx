(**
---
layout: standard
title: Parsing routes
toc: false
---
**)

(*** hide ***)

#load "./../import.fsx"

(**

The library defines building blocks for processing URLs in a way that assigns types to the values extracted.
The idea is to define a parser in type-safe fashion, using combinators:

- `s` combinator for a static string we expect to find in the URL,
- `</>` combinator to capture a slash in the URL,
- `str` combinator to extract a string,
- `i32` combinator to attempt to parse an `int`.
- `top` combinator that takes nothing.
- `remaining` combinator to get the remaining parts of the URL

Some examples:
*)

open Elmish.UrlParser

parse (s "blog" </> i32) "blog/42" // Some 42

parse (s "blog" </> i32) "blog/13" //  Some 13

parse (s "blog" </> i32) "blog/hello" // None

parse (s "search" </> str) "search/dogs" // Some "dogs"

parse (s "search" </> str) "search/13" // Some "13"

parse (s "search" </> remaining) "search/cats/dogs" // Some "cats/dogs"

parse (s "search" </> str) "search" // None

(**

Normally you want to put many of these parsers together to handle all possible routes. The following parser works on URLs like `/blog/42` and `/search/badger`:

*)

type Route =
    | Blog of int
    | Search of string

let route =
    oneOf
        [ map Blog (s "blog" </> i32)
          map Search (s "search" </> str) ]

(**

Here we are turning URLs into nice union types, so we can use case expressions to work with them in a nice way.
`oneOf` will try the listed parsers one by one until it finds one that returns `Some`. The `map` function in this example passes the outputs from the parser into the the case constructors.

*)

parse route "blog/58"    // Some (Blog 58)
parse route "search/cat" // Some (Search "cat")
parse route "search/31"  // Some (Search "31")
parse route "blog/cat"   // None
parse route "blog"       // None

(**

Note that F# case constructors take all of the arguments as a tuple, while the parser will apply the arguments individually, so we may need to adapt the signature:

*)


type Route2 = BlogDouble of int * string // needs arguments in tupled form

let curry f x y = f (x,y) // convert tupled form function of two arguments into curried form

let route2 state =
    map (curry BlogDouble) (s "blog" </> i32 </> str) state

(**

Now the compiler is happy for the two arguments to be passed individually to the curried case constructor.
*)
