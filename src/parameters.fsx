#load @"./utils/aLogger.fsx"

namespace Parameter

module Parameter =

    open ALogger

    [<Literal>]
    let DefaultNoOfGames = 5

    type Params = { NoOfGames: int }
    type Get = unit -> Async<Result<Params,string>>

    let get : Get = fun () ->
        let errMsg e = $"Invalid number for no of games - {e}"
        let maybe i f (sa:string array) = if Array.length sa < i + 1 then None else Some (sa.[i] |> f)
        let toInt x = if x = 0u then ALog.wrn "Will play just one game..."; 1 else x |> int
        let tryParams = try
                            let no =
                                match maybe 1 System.UInt32.Parse fsi.CommandLineArgs with
                                | Some x -> toInt x
                                | None -> ALog.wrn $"Nothing given, will play {DefaultNoOfGames} games"; DefaultNoOfGames
                            { NoOfGames = no } |> Ok
                        with | e -> e.Message |> errMsg |> Error
        async { return tryParams }