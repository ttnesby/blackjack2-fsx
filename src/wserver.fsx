#load @"./../.paket/load/Suave.fsx"
#load @"./bjParallel.fsx"

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open ALogger
open ParallelBJ

ALog.inf "Active"

[<Literal>]
let MaxNoOfGames = 100000

[<Literal>]
let DetailsLimit = 20

let play = ParallelBJ.play
            MaxNoOfGames
            DetailsLimit
            (fun r -> OK($"%s{r}"))
            (fun () -> RequestErrors.BAD_REQUEST($"Number of games must be in range - [1, {MaxNoOfGames}]"))

let playBJ noOfGames = context (fun _ -> play noOfGames)

let app =
    choose [
        GET >=> choose [
            path "/" >=> OK "/blackjack/{1 <= no_of_games <= 100 000}/games"
            pathScan "/blackjack/%d/games" playBJ
        ]
    ]

// https://pythonspeed.com/articles/docker-connection-refused/
let cfg = { defaultConfig with bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 8080 ] }

startWebServer cfg app
