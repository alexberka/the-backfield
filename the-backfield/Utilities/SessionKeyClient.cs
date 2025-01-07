using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Repositories;

namespace TheBackfield.Utilities
{
    public class SessionKeyClient
    {
        /// <summary>
        /// Generate a new random sessionKey for a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>sessionKey as string (5-13 characters with appended userId)</returns>
        public static string Generate(int userId)
        {
            string sessionKey = "";
            string characters = "zAyBxCwDvEuFtGsHrIqJpKoLnMmNlOkPjQiRhSgTfUeVdWcXbYaZ0192837465";

            Random selector = new();
            int keyLength = selector.Next(5, 13);

            for (int i = 1; i <= keyLength; i++)
            {
                sessionKey += characters[selector.Next(characters.Length)];
            }

            return $"{sessionKey}_{userId}";
        }
        /// <summary>
        /// For given 
        /// </summary>
        /// <param name="sessionKey">SessionKey parameter received from client</param>
        /// <param name="user">User object retrieved with SessionKey (may be null)</param>
        /// <param name="team"></param>
        /// <returns>ResponseDTO<Team> with Team = Team if access granted, Error = IResult if access denied </returns>
        public static ResponseDTO<Team> VerifyAccess(string sessionKey, User? user, Team? team)
        {
            if (team == null)
            {
                return new ResponseDTO<Team> { NotFound = true, ErrorMessage = "Invalid team id" };
            }
            if (user == null || user.SessionKey != sessionKey)
            {
                return new ResponseDTO<Team> { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }
            if (team.UserId != user.Id)
            {
                return new ResponseDTO<Team> { Forbidden = true, ErrorMessage = "User does not have access" };
            }
            return new ResponseDTO<Team> { Resource = team };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey">SessionKey parameter received from client</param>
        /// <param name="user">User object retrieved with SessionKey (may be null)</param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static ResponseDTO<Player> VerifyAccess(string sessionKey, User? user, Player? player)
        {
            if (player == null)
            {
                return new ResponseDTO<Player> { NotFound = true, ErrorMessage = "Invalid player id" };
            }
            if (user == null || user.SessionKey != sessionKey)
            {
                return new ResponseDTO<Player> { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }
            if (player.UserId != user.Id)
            {
                return new ResponseDTO<Player> { Forbidden = true, ErrorMessage = "User does not have access" };
            }
            return new ResponseDTO<Player> { Resource = player };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey">SessionKey parameter received from client</param>
        /// <param name="user">User object retrieved with SessionKey (may be null)</param>
        /// <param name="game">Game object retrieved with request (may be null)</param>
        /// <returns></returns>
        public static ResponseDTO<Game> VerifyAccess(string sessionKey, User? user, Game? game)
        {
            if (game == null)
            {
                return new ResponseDTO<Game> { NotFound = true, ErrorMessage = "Invalid game id" };
            }
            if (user == null || user.SessionKey != sessionKey)
            {
                return new ResponseDTO<Game> { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }
            if (game.UserId != user.Id)
            {
                return new ResponseDTO<Game> { Forbidden = true, ErrorMessage = "User does not have access" };
            }
            return new ResponseDTO<Game> { Resource = game };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey">SessionKey parameter received from client</param>
        /// <param name="user">User object retrieved with SessionKey (may be null)</param>
        /// <param name="gameStat">GameStat object retrieved with request (may be null)</param>
        /// <returns></returns>
        public static ResponseDTO<GameStat> VerifyAccess(string sessionKey, User? user, GameStat? gameStat)
        {
            if (gameStat == null)
            {
                return new ResponseDTO<GameStat> { NotFound = true, ErrorMessage = "Invalid gameStat id" };
            }
            if (user == null || user.SessionKey != sessionKey)
            {
                return new ResponseDTO<GameStat> { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }
            if (gameStat.UserId != user.Id)
            {
                return new ResponseDTO<GameStat> { Forbidden = true, ErrorMessage = "User does not have access" };
            }
            return new ResponseDTO<GameStat> { Resource = gameStat };
        }

        public static ResponseDTO<Play> VerifyAccess(string sessionKey, User? user, Play? play)
        {
            if (play == null)
            {
                return new ResponseDTO<Play> { NotFound = true, ErrorMessage = "Invalid play id" };
            }
            if (user == null || user.SessionKey != sessionKey)
            {
                return new ResponseDTO<Play> { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }
            if (play.Game == null || play.Game.UserId != user.Id)
            {
                return new ResponseDTO<Play> { Forbidden = true, ErrorMessage = "User lacks permissions to access this game" };
            }
            return new ResponseDTO<Play> { Resource = play };
        }
    }
}
