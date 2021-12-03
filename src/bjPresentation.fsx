#load @"./blackjack.fsx"

namespace BJPresentation

module BJPresentation = 

    open Blackjack
    open DeckOfCards
    open System.Text

    let private fewResults (winner, me, magnus) =
        [
            $"Winner: {winner}"
            $"me:     {Blackjack.score me} | {DeckOfCards.show me}"
            $"magnus: {Blackjack.score magnus} | {DeckOfCards.show magnus}"
            "-------------------------------------------------"
        ] |> List.fold (fun (r:StringBuilder) s -> r.AppendLine(s)) (StringBuilder())

    let few ra = Array.map fewResults ra

    let private manySummary (s: string * _[]) = StringBuilder($"{fst s} as winner of [{Array.length (snd s)}] games")

    let many ra = 
        ra
        |> Array.map (fun (w: string, _, _) -> w)
        |> Array.groupBy id
        |> Array.sortBy fst
        |> Array.map manySummary