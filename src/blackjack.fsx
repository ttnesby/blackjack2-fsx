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
            | [_;_], [_;_] when isBJ me && isBJ magnus -> result "Draw  "
            | [_;_], _ when isBJ me -> wMe
            | _, [_;_] when isBJ magnus -> wMagnus
            | _::_, _ when isLT17 me -> me' getACard
            | _::_, _ when isBJ me -> wMe 
            | _::_, _ when isGTBJ me -> wMagnus            
            | _, _::_ when isLEMe magnus -> magnus' getACard
            | _, _::_ when isGTBJ magnus -> wMe
            | _, _ -> wMagnus

        return loop doc me magnus
    }