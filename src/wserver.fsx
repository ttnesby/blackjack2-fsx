#load @"./../.paket/load/Suave.fsx"
#load @"./parallellbj.fsx"
#load @"./parameters.fsx"

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open Parameter
open ALogger
open ParallellBJ

ALog.inf "Active"

[<Literal>]
let MaxNoOfGames = 100000


let playBJ noOfGames = context (fun _ ->
    // get something from ctx.request...
    ParallellBJ.play 
        MaxNoOfGames
        Parameter.DefaultNoOfGames 
        noOfGames 
        (fun r -> OK($"%s{r}"))
        (fun () -> RequestErrors.BAD_REQUEST($"Number of games must be in range - [1, {MaxNoOfGames}]"))    
)

let app = 
    choose [ 
        GET >=> choose [ 
            path "/" >=> OK "/blackjack/{1 <= no_of_games <= 100 000}/games"
            pathScan "/blackjack/%d/games" playBJ
        ]
        POST >=> choose [ 
            path "/hello" >=> OK "Hello POST"
            path "/goodbye" >=> OK "Good bye POST" 
        ] 
    ]

// https://pythonspeed.com/articles/docker-connection-refused/
let cfg = { defaultConfig with bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 8080 ] }

startWebServer cfg app
