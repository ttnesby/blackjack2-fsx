namespace Card

module Card = 
    [<RequireQualifiedAccess>]
    type Suite = Spade | Heart | Club | Diamond

    [<RequireQualifiedAccess>]
    type Value = Two | Three | Four | Five | Six | Seven | Eight | Nine | Ten |Jack | Queen | King | Ace

    let private suitName = 
        function | Suite.Spade -> "S" | Suite.Heart -> "H" | Suite.Club -> "C" | Suite.Diamond -> "D"

    let private valueName = 
        function | Value.Two -> "2" | Value.Three -> "3" | Value.Four -> "4" | Value.Five -> "5" | Value.Six -> "6" | Value.Seven -> "7" | Value.Eight -> "8" | Value.Nine -> "9" | Value.Ten -> "10" | Value.Jack -> "J" | Value.Queen -> "Q" | Value.King -> "K" | Value.Ace -> "A"

    let name (s,v) = suitName s + valueName v

    let suites () = seq { Suite.Spade; Suite.Heart; Suite.Club; Suite.Diamond }
    let values () = seq { 
            Value.Two ;Value.Three ;Value.Four ;Value.Five; Value.Six; 
            Value.Seven; Value.Eight; Value.Nine; Value.Ten; Value.Jack; 
            Value.Queen; Value.King; Value.Ace }





