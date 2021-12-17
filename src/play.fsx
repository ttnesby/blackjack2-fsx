#!/usr/bin/env -S dotnet fsi
#load @"./parameters.fsx"
#load @"./bjParallel.fsx"

open ALogger
open Parameter
open ParallelBJ

ALog.inf "Active"

[<Literal>]
let MaxNoOfGames = 1000000

[<Literal>]
let DetailsLimit = 10

let play = ParallelBJ.play
                    MaxNoOfGames
                    DetailsLimit
                    (fun r -> ALog.inf $"{r}"; 0)
                    (fun () -> ALog.wrn $"Number of games must be in range [1, {MaxNoOfGames}]"; 1)

Parameter.get ()
|> Async.RunSynchronously
|> function
| Ok p -> play p.NoOfGames
| Error e -> ALog.err $"{e}"; 1
|> fun i ->
    ALog.inf $"Exit code: {i}"
    exit i