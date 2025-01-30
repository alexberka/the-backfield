<h1 align="center" style="font-weight: bold;">The Backfield</h1>
<p>v1.025.0124.1b compatible with UI v1.025.0129.1 or later</p>

<p align="center">
<a href="#tech">Technologies</a>
<a href="#started">Getting Started</a>
<a href="#routes">API Endpoints</a>
<a href="#documentation">Further Documentation</a>
</p>


<p>The Backfield is a database management and live broadcasting API for American football statkeeping.</p>


<h2 id="technologies">Technologies</h2>

- C#/ASP.Net
- Entity Framework Core
- SignalR
- postgreSQL
- Swagger (development only)

<h2>User-Specific Data</h2>

Data in The Backfield is user-specific, to the extent that most calls accessing this data must include the user's session key for access verification. Only users with that session key can access/edit that data. For many calls, the session key is passed as a query parameter. For many others, it is included in the request body. 
Session keys are generated on the creation/login of a user via uid. The uid may be handled by any authentication process integrated into the UI (Google Firebase is used in [The Official Backfield UI](http://github.com/alexberka/the-backfield-ui)) or may be manually specified by the user when creating the data. Once the User is created, the associated uid cannot be altered by calls.

<h2 id="started">Getting Started</h2>

Clone this project to a directory on your machine using

```bash
git clone git@github.com:alexberka/the-backfield.git
```

Open the solution file in an IDE. In order to initialize the database on your machine and populate with seed data, run:

```
ef database update
```

in the terminal.

<h2>Primary Data Structures</h2>

Teams

```
{
  Id: <int>,
  LocationName: <string>,
  Nickname: <string>,
  Players: [List<Player>],
  HomeField: <string>,
  HomeLocation: <string>,
  LogoUrl: <string>,
  ColorPrimaryHex: <string>,
  ColorSecondaryHex: <string>,
  UserId: <int>
}
```

Players

```
{
  Id: <int>,
  FirstName: <string>,
  LastName: <string>,
  ImageUrl: <string>,
  BirthDate: <DateTime>,
  Hometown: <string>
  TeamId: <int>,
  Team: <Team>,
  JerseyNumber: <int>,
  UserId: <int>,
  Positions: [List<Position>]
}
```

Games

```
{
  Id: <int>,
  HomeTeamId: <int>,
  HomeTeam: <Team>,
  HomeTeamScore: <int>,
  HomeTeamStats: <object>, // Accumulated GameStats as anonymous DTO
  AwayTeamId: <int>,
  AwayTeam: <Team>,
  AwayTeamScore: <int>,
  AwayTeamStats: <object>, // Accumulated GameStats as anonymous DTO
  GameStart: <DateTime>,
  GamePeriods: <int>, // Number of time divisions of the game, 4 = quarters, 2 = halves, etc.
  PeriodLength: <int>, // Number of seconds per time division
  UserId: <int>
}
```

Plays

```
	Id: <int>
```

GameStats (Deprecated)

```
{
  Id: <int>,
  GameId: <int>,
  PlayerId: <int>,
  TeamId: <int>,
  RushYards: <int>,
  RushAttempts: <int>,
  PassYards: <int>,
  PassAttempts: <int>,
  PassCompletions: <int>,
  ReceivingYards: <int>,
  Receptions: <int>,
  Touchdowns: <int>,
  Tackles: <int>,
  InterceptionsThrown: <int>,
  InterceptionsReceived: <int>,
  FumblesCommitted: <int>,
  FumblesForced: <int>,
  FumblesRecovered: <int>,
  UserId: <int>
}
```

All relationships must established on creation of each entity.

<h2>Running the Application</h2>

The application can be started in the terminal is Visual Studio using:

```
dotnet run
```

or started in the debugger. If started with the debugger, the app will enter development mode, and Swagger will automatically open in a new window.

To interact with the API via Postman, access the [The Backfield Documentation on Postman](https://documenter.getpostman.com/view/31791227/2sAYBYgWGq) and click "Run in Postman". Choose either the web or desktop app in the popup, select a collection to import into, and click import.

The {{baseUrl}} variable must be adjusted to the proper local port. Find this in your Visual Studio terminal or in the Swagger window if operating in development mode.

<h2 id="routes">API Endpoints</h2>

Here is a list of all endpoints and their functions.
For complete endpoint documentation, visit [The Backfield Documentation on Postman](https://documenter.getpostman.com/view/31791227/2sAYBYgWGq)
​
| route               | description                                          
|----------------------|-----------------------------------------------------
| <kbd>GET /games?sessionKey=\<sessionKey\></kbd>     | retrieve user's games
| <kbd>POST /games</kbd>     | create game
| <kbd>GET /games/{gameId}?sessionKey=\<sessionKey\></kbd>     | retrieve single game with teams and gamestats
| <kbd>PUT /games/{gameId}</kbd>     | update game
| <kbd>DELETE /games/{gameId}?sessionKey=\<sessionKey\></kbd>     | delete game
| <kbd>GET /games/{gameId}/game-stream</kbd>	| retrieve gamestream
| <kbd>POST /game-stats</kbd>     | create game-stat
| <kbd>PUT /game-stats/{gameStatId}</kbd>     | update game-stat
| <kbd>DELETE /game-stats/{gameStatId}?sessionKey=\<sessionKey\></kbd>     | delete game-stat
| <kbd>GET /penalties?sessionKey=\<sessionKey\></kbd>	| retrieve penalties (pre-loaded + user-specific)
| <kbd>GET /plays/{playId}?sessionKey=\<sessionKey\></kbd>	| retrieve single play
| <kbd>POST /plays</kbd>	| create play
| <kbd>PUT /plays/{playId}</kbd>	| update play
| <kbd>DELETE /plays/{playId}?sessionKey=\<sessionKey\></kbd>	| delete play
| <kbd>GET /players?sessionKey=\<sessionKey\></kbd>     | retrieve user's players
| <kbd>POST /players</kbd>     | create player
| <kbd>GET /players/{playerId}?sessionKey=\<sessionKey\></kbd>     | retrieve player with team and positions
| <kbd>PUT /players/{playerId}</kbd>     | update player
| <kbd>DELETE /players/{playerId}?sessionKey=\<sessionKey\></kbd>     | delete player
| <kbd>POST /players/{playerId}/add-positions</kbd>     | add positions to player
| <kbd>POST /players/{playerId}/remove-positions</kbd>     | remove positions from player
| <kbd>GET /positions</kbd>     | retrieve positions list
| <kbd>GET /teams?sessionKey=\<sessionKey\></kbd>     | retrieve user's teams
| <kbd>POST /teams</kbd>     | create team
| <kbd>GET /teams/{teamId}?sessionKey=\<sessionKey\></kbd>     | retrieve single team with players
| <kbd>PUT /teams/{teamId}</kbd>     | update team
| <kbd>DELETE /teams/{teamId}?sessionKey=\<sessionKey\></kbd>     | delete team
| <kbd>GET /users/{uid}</kbd>     | retrieve user by uid
| <kbd>POST /users</kbd>     | create user
|
| <kbd>WEBSOCKET /watch?gameId=\<gameId\></kbd>	| establish gamestream websocket connection


<h3 id="documentation">Further Documentation</h3>

[The Backfield repository on GitHub](http://github.com/alexberka/the-backfield)
[The Backfield UI repository on GitHub](http://github.com/alexberka/the-backfield-ui)
[ERD on dbdiagram](https://dbdiagram.io/d/The-Backfield-6732a4d0e9daa85aca1861f6)
[Operational Walk-Through on YouTube](https://www.youtube.com/watch?v=bem_ITHckJQ)
