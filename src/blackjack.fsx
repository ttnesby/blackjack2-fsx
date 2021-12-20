#load @"./deckOfCards.fsx"

namespace Blackjack

module Blackjack =

    open Card
    open DeckOfCards
     // a comment - mapping from card value to blackjack score
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

    type Winner =
    | Draw
    | Me
    | Magnus

    let play doc me magnus = async {

        let rec loop doc me magnus =

            let getTwoCards = DeckOfCards.giveReceiveX 2 doc
            let getACard = DeckOfCards.giveReceiveX 1 doc
            let me' f = f me |> fun (doc',me') -> loop doc' me' magnus
            let magnus' f = f magnus |> fun (doc',magnus') -> loop doc' me magnus'
            let result winner = winner, me, magnus

            match (me, score me, magnus,score magnus) with
            | [],_, _,_ -> me' getTwoCards
            | _,_, [],_ -> magnus' getTwoCards
            | [_;_],BlackJack, [_;_],BlackJack -> result Draw
            | [_;_],BlackJack, _,_ -> result Me
            | _,_, [_;_],BlackJack -> result Magnus
            | _::_,myScore, _,_ when myScore < 17 -> me' getACard
            | _::_,myScore, _,_ when myScore > BlackJack -> result Magnus
            | _,myScore, _::_,magnusScore when magnusScore <= myScore -> magnus' getACard
            | _,_, _::_,magnusScore when magnusScore > BlackJack -> result Me
            | _,_, _,_ -> result Magnus

        return (loop doc me magnus)
    }