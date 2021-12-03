#load @"./../.paket/load/Suave.fsx"
#load @"./bjPresentation.fsx"

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let noOfBJGames noOfGames =
    context (fun ctx ->
    // get something from ctx.request...

        OK(noOfGames.ToString())

let app = 
    choose [ 
        GET >=> choose [ 
            path "/hello" >=> OK "Hello GET"
            path "/goodbye" >=> OK "Good bye GET" 
            pathScan "/%d/play"  (fun (a) -> OK((a).ToString()))
        ]
        POST >=> choose [ 
            path "/hello" >=> OK "Hello POST"
            path "/goodbye" >=> OK "Good bye POST" 
        ] 
    ]

startWebServer defaultConfig app
