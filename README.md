# blackjack2-fsx

Not much to say about the code. Focus on F# scripting - no build.

> - `/paket.dependencies` is the [packet manager](https://fsprojects.github.io/Paket/)
> - `/src/utils` contains a solution for logging [NLog](https://nlog-project.org/)
> - `/src/card.fsx` is card functionality
> - `/src/deckOfCards.fsx` is deck of cards functionality
> - `/src/blackjack.fsx` implements the rules for [NAV blackjack](http://nav-deckofcards.herokuapp.com/#/)
> - `/src/bjPresentation.fsx` transforms results to json with [Thoth.Json.Net](https://thoth-org.github.io/Thoth.Json/)
> - `/src/parallellbj.fsx` plays **x** no of games in parallel
> - `/src/parameters.fsx` manage parameter(s), `noOfGames` for the console game
> - `/src/play.fsx` is console black jack game
> - `/src/wserver.fsx` is a [Suave](https://suave.io/) lightweight web server black jack game
> - `/Dockerfile` build a docker container of the Suave web server part 

