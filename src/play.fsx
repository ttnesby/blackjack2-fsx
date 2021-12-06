#!/usr/bin/env -S dotnet fsi
#load @"./parameters.fsx"
#load @"./parallellbj.fsx"

open ALogger
open Parameter
open ParallellBJ

ALog.inf "Active"

[<Literal>]
let MaxNoOfGames = 1000000

Parameter.get ()
|> Async.RunSynchronously
|> function
| Ok p ->
    ParallellBJ.play 
        MaxNoOfGames
        Parameter.DefaultNoOfGames 
        p.NoOfGames 
        (fun r -> printfn $"{r}"; 0)
        (fun () -> ALog.wrn "Number of games must be in range [1, 1 000 000]"; 1)
| Error e -> ALog.err $"{e}"; 1
|> fun i ->
    ALog.inf $"Exit code: {i}"
    exit i