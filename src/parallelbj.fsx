#load @"./utils/aLogger.fsx"
#load @"./bjPresentation.fsx"

namespace ParallelBJ

module ParallelBJ =

    open Blackjack
    open DeckOfCards
    open ALogger
    open BJPresentation

    let private createGames p =
        seq {for _ in 1..p -> Blackjack.play (DeckOfCards.random (System.Random())) [] []}

    let play max detailsLimit noOfGames fResult fOutsideRange =
        match noOfGames with
        | x when x >= 1 && x <= max ->
            createGames noOfGames
            |> ALog.logPassThroughX ALog.inf $"Created [{noOfGames}] games"
            |> Async.Parallel
            |> ALog.logPassThroughX ALog.inf $"Playing [{noOfGames}] games in parallel"
            |> Async.RunSynchronously
            |> ALog.logPassThroughX ALog.inf $"[{noOfGames}] games completed"
            |> fun ra ->
                if noOfGames <= detailsLimit
                then BJPresentation.few ra
                else BJPresentation.many ra
            |> fun r -> r |> fResult
        | _ -> fOutsideRange()