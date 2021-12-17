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

# Test docker image
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
# Push image to Docker Hub

```zsh
# login to docker
docker login

# output
Authenticating with existing credentials...
Login Succeeded

Logging in with your password grants your terminal complete access to your account.
For better security, log in with a limited-privilege personal access token. Learn more at https://docs.docker.com/go/access-tokens/

# see docker images
docker images

# output
REPOSITORY                     TAG          IMAGE ID       CREATED          SIZE
blackjack2fsx                  latest       f5e25c629a16   45 minutes ago   666MB
mcr.microsoft.com/dotnet/sdk   6.0-alpine   9d2c47a10a43   2 days ago       579MB
blackjack2fsx                  1.0          fda98ab9e5f6   10 days ago      665MB
ttnesby/blackjack2fsx          1.0          fda98ab9e5f6   10 days ago      665MB
ttnesby/blackjack2fsx          latest       fda98ab9e5f6   10 days ago      665MB
dockerfsx                      latest       d20dc352896c   2 weeks ago      578MB

# re-tag latest image with repo prefix
docker tag blackjack2fsx:latest ttnesby/blackjack2fsx:latest

# output

# push repo prefixed image to docker hub
docker push ttnesby/blackjack2fsx:latest

# output
The push refers to repository [docker.io/ttnesby/blackjack2fsx]
7d70e7700ff3: Pushed
6daad2d19466: Pushed
2bb1bcfa94ce: Pushed
056338cf753c: Pushed
6c8b52ff5f7e: Pushed
a443873b44c8: Pushed
d50e19f37caf: Pushed
146c4f3f0117: Pushed
f73c9c5cb77b: Pushed
11458683b1a7: Pushed
2fc5396cf731: Pushed
11c15b90ebce: Pushed
98bc857b3aed: Layer already exists
1a058d5342cc: Layer already exists
latest: digest: sha256:01da91ac551dadee272c1e19661add52765b49d5410c8e6b4dce9dda258e0e6b size: 3260
```