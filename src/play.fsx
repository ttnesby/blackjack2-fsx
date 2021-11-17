#!/usr/bin/env -S dotnet fsi
#load @"./utils/aLogger.fsx"
#load @"./blackjack.fsx"

open Blackjack
open DeckOfCards
open ALogger

[<Literal>]
let DefaultNoOfGames = 5

type Params = { NoOfGames: int }
type GetParams = unit -> Async<Result<Params,string>>

let getParams : GetParams = fun () ->
    let errMsg e = $"Invalid number for no of games - {e}"
    let maybe i f (sa:string array) = if Array.length sa < i + 1 then None else Some (sa.[i] |> f)
    let tryParams = try 
                        let no = 
                            match maybe 1 System.UInt32.Parse fsi.CommandLineArgs with
                            | Some x -> 
                                if x = 0u 
                                then ALog.wrn "Will play just one game..."; 1
                                else x |> int
                            | None ->
                                ALog.wrn $"Nothing given, will play {DefaultNoOfGames} games"; DefaultNoOfGames

                        { NoOfGames = no } |> Ok
                    with | e -> e.Message |> errMsg |> Error
    async { return tryParams }

let createGames p = 
        seq {for n in 1..p -> Blackjack.play (DeckOfCards.random (System.Random())) [] []}

let showResult (me, magnus) =
    printfn "Winner: you can handle this..."
    printfn $"me:     {Blackjack.score me} | %A{DeckOfCards.show me}"
    printfn $"magnus: {Blackjack.score magnus} | %A{DeckOfCards.show magnus}"

let showSummary (k,a) = printfn $"{k} winning {Array.length a} games"

let resolveWinner (me, magnus) = 
    let me' = "Me"
    let magnus' = "Magnus"
    match (Blackjack.score me, Blackjack.score magnus) with
    | 21,21 -> "Draw"
    | 21, _ -> me'
    | _, 21 -> magnus'
    | x, _ when x > Blackjack.BlackJack -> magnus'
    | _, y when y > Blackjack.BlackJack -> me'
    | _, _ -> magnus'

getParams () 
|> Async.RunSynchronously
|> function 
| Ok p ->        
    createGames p.NoOfGames
    |> ALog.logPassThroughX ALog.inf $"Created {p.NoOfGames} games..."
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ALog.logPassThroughX ALog.inf $"{p.NoOfGames} games completed"
    |> fun ra -> 
        if p.NoOfGames <= DefaultNoOfGames 
        then Array.map showResult ra 
        else Array.map resolveWinner ra |> Array.groupBy id |> Array.sortBy fst |> Array.map showSummary  
    |> ignore
    0
| Error e ->
    ALog.err $"{e}"
    1
|> fun i ->
    ALog.inf $"Exit code: {i}"
    exit i