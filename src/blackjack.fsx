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
    let private isLEMe (magnus, me) = score magnus <= score me

    let rec private play' doc me magnus =

        let getTwoCards = DeckOfCards.giveReceiveX 2 doc
        let getACard = DeckOfCards.giveReceiveX 1 doc
        let me' f = f me |> fun (doc',me') -> play' doc' me' magnus
        let magnus' f = f magnus |> fun (doc',magnus') -> play' doc' me magnus'  
        let result = me, magnus, doc 

        match (me, magnus) with
        | [], _ -> me' getTwoCards
        | _, [] -> magnus' getTwoCards
        | [e1;e2], [m1;m2] when isBJ [e1;e2] || isBJ [m1;m2] -> result
        | e::et, _ when isLT17 (e::et) -> me' getACard
        | e::et, _ when isBJ (e::et) || isGTBJ (e::et) -> result
        | _, m::mt when isLEMe (m::mt, me) -> magnus' getACard
        | _, _ -> result

    let play = play' (DeckOfCards.random (System.Random()))

