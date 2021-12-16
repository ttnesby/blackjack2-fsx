#load @"./card.fsx"

namespace DeckOfCards

module DeckOfCards =

    open Card

    // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    let private shuffleArray (r:System.Random) (a:_[]) =
        for i in (a.Length - 1) .. -1 .. 0 do
            let j = r.Next(0,i)
            let tmp = a[i]
            a[i] <- a[j]
            a[j] <- tmp
        a
    let private create () =
        [|for s in (Card.suites ()) do
            for v in (Card.values ()) do
                s,v |]

    let random (r: System.Random) = create () |> shuffleArray r |> List.ofArray

    let show =
        let folder (str: string) c = if str.Length = 0 then c else str + $" {c}"
        List.map Card.name >> List.fold folder ""

    let private give = function
        | [] -> [],None
        | [h] -> [],Some h
        | h::t -> t,Some h

    let private receive = function
        | [],None -> []
        | [], Some c -> [c]
        | h::t, None -> (h::t)
        | h::t, Some c -> (h::t)@[c]

    let private giveReceive (g, r) = give g |> fun (g',c) -> g', (receive (r,c))

    let rec giveReceiveX x g r  =
        match (g,x) with
        | [], _ -> g, r
        | _, x when x <= 0 -> g, r
        | _,_ ->
            let g',r' = giveReceive (g,r)
            giveReceiveX (x - 1) g' r'