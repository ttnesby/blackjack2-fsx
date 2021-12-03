//#!/usr/bin/env -S dotnet fsi
#load @"./parameters.fsx"
#load @"./bjPresentation.fsx"

open Blackjack
open DeckOfCards
open ALogger
open Parameter
open BJPresentation

let createGames p =
        seq {for _ in 1..p -> Blackjack.play (DeckOfCards.random (System.Random())) [] []}

ALog.inf "Active"

Parameter.get ()
|> Async.RunSynchronously
|> function
| Ok p ->
    createGames p.NoOfGames
    |> ALog.logPassThroughX ALog.inf $"Created [{p.NoOfGames}] games"
    |> Async.Parallel
    |> ALog.logPassThroughX ALog.inf $"Playing [{p.NoOfGames}] games in parallell"
    |> Async.RunSynchronously
    |> ALog.logPassThroughX ALog.inf $"[{p.NoOfGames}] games completed"
    |> fun ra -> 
        if p.NoOfGames <= Parameter.DefaultNoOfGames 
        then BJPresentation.few ra 
        else BJPresentation.many ra
    |> fun ra -> 
        ra |> Array.iter (fun e -> printfn $"{e}") |> ignore
        0    
| Error e ->
    ALog.err $"{e}"
    1
|> fun i ->
    ALog.inf $"Exit code: {i}"
    exit i