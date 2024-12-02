<h1 align="center" style="font-weight: bold;">The Backfield</h1>

<p align="center">
<a href="#tech">Technologies</a>
<a href="#started">Getting Started</a>
<a href="#routes">API Endpoints</a>
<a href="#documentation">Further Documentation</a>
</p>


<p>The Backfield is a database management system for American football statkeeping.</p>


<h2 id="technologies">Technologies</h2>

- C#/.Net
- Entity Framework Core
- Visual Studio
- postgres/pgAdmin
- Swagger (development only)

<h2>User-Specific Data</h2>

Data in The Backfield is user-specific, to the extent that all calls accessing this data must include the user's session key for access verification. Only users with that session key can access/edit that data. For many calls, the session key is passed as a query parameter. For the remainder, it is included in the request body. 
Session keys are generated on the creation/login of a user via uid. The uid may be handled by any authentication process integrated into the UI (Google Firebase, etc.) or may be manually specified by the user when creating the data. Once the User is created, the associated uid cannot be altered.

<h2 id="started">Getting Started</h2>

Clone this project to a directory on your machine using

```bash
git clone git@github.com:alexberka/the-backfield.git
```

Open the solution file in Visual Studio. In order to initialize the database on your machine and populate with seed data, run:

```
ef database update
```

in the terminal.

<h2>Data Structures</h2>
Data is stored as one of four primary entities:

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

GameStats

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
| <kbd>POST /game-stats</kbd>     | create game-stat
| <kbd>PATCH /game-stats/{gameStatId}</kbd>     | update game-stat
| <kbd>DELETE /game-stats/{gameStatId}?sessionKey=\<sessionKey\></kbd>     | delete game-stat
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


<h3 id="documentation">Further Documentation</h3>

[The Backfield repository on GitHub](github.com/alexberka/the-backfield)
[ERD on dbdiagram](https://dbdiagram.io/d/The-Backfield-6732a4d0e9daa85aca1861f6)
[Operational Walk-Through on YouTube]()
