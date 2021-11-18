#load @"./deckOfCards.fsx"

namespace Blackjack

module Blackjack =

    open Card
    open DeckOfCards

    let private cardScore (_,v) =
        match v with
        | Card.Value.Two -> 2
        | Card.Value.Three -> 3
        | Card.Value.Four -> 4
        | Card.Value.Five -> 5
        | Card.Value.Six -> 6
        | Card.Value.Seven -> 7
        | Card.Value.Eight ->8
        | Card.Value.Nine -> 9
        | Card.Value.Ten -> 10
        | Card.Value.Jack -> 10
        | Card.Value.Queen -> 10
        | Card.Value.King -> 10
        | Card.Value.Ace -> 11

    let score doc = List.fold (fun score c -> score + (cardScore c)) 0 doc

    [<Literal>]
    let BlackJack = 21

    let private isBJ doc = score doc = BlackJack
    let private isLT17 doc = score doc < 17
    let private isGTBJ doc = score doc > BlackJack

    let play doc me magnus = async {

        let rec loop doc me magnus =

            let getTwoCards = DeckOfCards.giveReceiveX 2 doc
            let getACard = DeckOfCards.giveReceiveX 1 doc
            let me' f = f me |> fun (doc',me') -> loop doc' me' magnus
            let magnus' f = f magnus |> fun (doc',magnus') -> loop doc' me magnus'
            let isLEMe magnus = score magnus <= score me
            let result winner = winner, me, magnus
            let wMe = result "Me    "
            let wMagnus = result "Magnus"

            match (me, magnus) with
            | [], _ -> me' getTwoCards
            | _, [] -> magnus' getTwoCards
            | [e1;e2], [m1;m2] when isBJ [e1;e2] && isBJ [m1;m2] -> result "Draw  "
            | [e1;e2], _ when isBJ [e1;e2] -> wMe
            | _, [m1;m2] when isBJ [m1;m2] -> wMagnus
            | e::et, _ when isLT17 (e::et) -> me' getACard
            | e::et, _ when isBJ (e::et) -> wMe 
            | e::et, _ when isGTBJ (e::et) -> wMagnus            
            | _, m::mt when isLEMe (m::mt) -> magnus' getACard
            | _, m::mt when isGTBJ (m::mt) -> wMe
            | _, _ -> wMagnus

        return loop doc me magnus
    }