#load @"./blackjack.fsx"

namespace BJPresentation

module BJPresentation = 

    open Blackjack
    open DeckOfCards

    let private fewResults (winner, me, magnus) =
        printfn $"Winner: {winner}"
        printfn $"me:     {Blackjack.score me} | {DeckOfCards.show me}"
        printfn $"magnus: {Blackjack.score magnus} | {DeckOfCards.show magnus}"
        printfn "-------------------------------------------------"

    let few ra = Array.map fewResults ra

    let private manySummary (s: string * _[]) = printfn $"{fst s} as winner of [{Array.length (snd s)}] games"

    let many ra = 
        ra
        |> Array.map (fun (w: string, _, _) -> w)
        |> Array.groupBy id
        |> Array.sortBy fst
        |> Array.map manySummary