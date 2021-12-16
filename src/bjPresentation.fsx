#load @"./../.paket/load/Thoth.Json.Net.fsx"
#load @"./blackjack.fsx"

namespace BJPresentation

module BJPresentation =

    open Thoth.Json.Net
    open Blackjack
    open DeckOfCards

    type PlayerStatus = {
        Score: int
        Hand: string
    }

    let playerStatus hand = {
        Score = Blackjack.score hand
        Hand = DeckOfCards.show hand
    }
    type GameResult = {
        Winner : Blackjack.Winner
        Magnus: PlayerStatus
        Me: PlayerStatus
    }

    let private toGameResult(winner, me, magnus) =
        {
            Winner = winner
            Magnus = playerStatus magnus
            Me = playerStatus me
        }

    type GameSummary = {
        Winner: Blackjack.Winner
        Games : int
    }

    let few ra = ra |> Array.map toGameResult |> fun r -> Encode.Auto.toString(4, r)

    let private manySummary (s: Blackjack.Winner * _[]) = {
        Winner = fst s
        Games = Array.length (snd s)
    }

    let many ra =
        ra
        |> Array.map (fun (w: Blackjack.Winner, _, _) -> w)
        |> Array.groupBy id
        |> Array.sortBy fst
        |> Array.map manySummary
        |> fun r -> Encode.Auto.toString(4, r)