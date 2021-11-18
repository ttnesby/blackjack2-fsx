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

let showGameResult (winner, me, magnus) =
    printfn $"Winner: {winner}"
    printfn $"me:     {Blackjack.score me} | %A{DeckOfCards.show me}"
    printfn $"magnus: {Blackjack.score magnus} | %A{DeckOfCards.show magnus}"
    printfn "-------------------------------------------------"

let showGamesSummary (s: string * _[]) = printfn $"{fst s} as winner of [{Array.length (snd s)}] games"

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
        then Array.map showGameResult ra 
        else 
            Array.map (fun (w: string, _, _) -> ())
            // |> Array.groupBy   
            // |> Array.map showGamesSummary  
    |> ignore
    0
| Error e ->
    ALog.err $"{e}"
    1
|> fun i ->
    ALog.inf $"Exit code: {i}"
    exit i