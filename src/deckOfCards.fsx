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
        |],r) |> shuffleArray |> List.ofArray

    let show doc = List.map Card.name doc

    let private give  = function | [] -> None,[] | [h] -> Some h,[] | h::t -> Some h,t

    let private receive doc c =
        match (doc,c) with | [],None -> doc | [], Some c -> [c] | h::t, Some c -> doc@[c] | h::t, None -> doc

    let private giveReceive (g, r) = give g |> fun (c,g') -> g', (receive r c)

    let rec giveReceiveX x g r  =
        match (g,x) with
        | [], _ -> g, r
        | _, 0 -> g, r
        | _,_ -> 
            let g',r' = giveReceive (g,r)
            giveReceiveX (x - 1) g' r'


