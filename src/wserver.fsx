#load @"./../.paket/load/Suave.fsx"
#load @"./parameters.fsx"
#load @"./bjPresentation.fsx"

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open Blackjack
open DeckOfCards
open ALogger
open Parameter
open BJPresentation

let createGames p =
        seq {for _ in 1..p -> Blackjack.play (DeckOfCards.random (System.Random())) [] []}

ALog.inf "Active"

let playBJ noOfGames = context (fun _ ->
    // get something from ctx.request...
    ALog.inf "Active"
    createGames noOfGames
    |> ALog.logPassThroughX ALog.inf $"Created [{noOfGames}] games"
    |> Async.Parallel
    |> ALog.logPassThroughX ALog.inf $"Playing [{noOfGames}] games in parallell"
    |> Async.RunSynchronously
    |> ALog.logPassThroughX ALog.inf $"[{noOfGames}] games completed"
    |> fun ra -> if noOfGames <= Parameter.DefaultNoOfGames then BJPresentation.few ra else BJPresentation.many ra
    |> fun ra -> OK($"%A{ra}")
)

let app = 
    choose [ 
        GET >=> choose [ 
            path "/hello" >=> OK "Hello GET"
            path "/goodbye" >=> OK "Good bye GET" 
            pathScan "/blackjack/%d/games" playBJ
        ]
        POST >=> choose [ 
            path "/hello" >=> OK "Hello POST"
            path "/goodbye" >=> OK "Good bye POST" 
        ] 
    ]

startWebServer defaultConfig app
