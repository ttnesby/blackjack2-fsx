#load @"./card.fsx"

namespace DeckOfCards

module DeckOfCards =

    open Card

    // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    let private shuffleArray (a:_[],r:System.Random) =
        for i in (a.Length - 1) .. -1 .. 0 do
            let j = r.Next(0,i)
            let tmp = a.[i]
            a.[i] <- a.[j]
            a.[j] <- tmp
        a

    let random (r:System.Random) =
        ([|for s in (Card.suites ()) do
            for v in (Card.values ()) do
                s,v
        |],r)
        |> shuffleArray |> Seq.ofArray

    let show doc = doc |> Seq.map Card.name 


