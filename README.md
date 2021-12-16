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
> - `/Dockerfile` build a docker container for Suave web server 

```zsh
# build docker container
docker build --pull --rm -f "Dockerfile" -t blackjack2fsx:latest "."

# running docker container
docker run -d -p 900:8080 blackjack2fsx:latest
```
```powershell
# test the web server - root
Invoke-RestMethod -Uri 'http://localhost:900/'

# output - info
/blackjack/{1 <= no_of_games <= 100 000}/games

# test negative no of games
Invoke-RestMethod -Uri 'http://localhost:900/blackjack/-1/games'

# output
Invoke-RestMethod: Number of games must be in range - [1, 100000]

# <= 5 games (parameters.fsx defaultGames) gives full output
Invoke-RestMethod -Uri 'http://localhost:900/blackjack/3/games'

# output
Winner Magnus                     Me
------ ------                     --
Me     @{Score=23; Hand=HK S3 CQ} @{Score=17; Hand=C7 CK}
Magnus @{Score=15; Hand=S10 D5}   @{Score=24; Hand=S3 DK SA}
Magnus @{Score=20; Hand=H6 S9 C5} @{Score=19; Hand=H3 H9 D7}

# > 5 games gives a summary
Invoke-RestMethod -Uri 'http://localhost:900/blackjack/100000/games'

# output
Winner Games
------ -----
Draw     188
Magnus 53415
Me     46397
```