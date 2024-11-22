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
        /// <returns>TeamResponseDTO with Team = Team if access granted, Error = IResult if access denied </returns>
        public static TeamResponseDTO VerifyAccess(string sessionKey, User? user, Team? team)
        {
            if (team == null)
            {
                return new TeamResponseDTO { NotFound = true, ErrorMessage = "Invalid team id" };
            }
            if (user == null || user.SessionKey != sessionKey)
            {
                return new TeamResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }
            if (team.UserId != user.Id)
            {
                return new TeamResponseDTO { Forbidden = true, ErrorMessage = "User does not have access" };
            }
            return new TeamResponseDTO { Team = team };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey">SessionKey parameter received from client</param>
        /// <param name="user">User object retrieved with SessionKey (may be null)</param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static PlayerResponseDTO VerifyAccess(string sessionKey, User? user, Player? player)
        {
            if (player == null)
            {
                return new PlayerResponseDTO { NotFound = true, ErrorMessage = "Invalid player id" };
            }
            if (user == null || user.SessionKey != sessionKey)
            {
                return new PlayerResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }
            if (player.UserId != user.Id)
            {
                return new PlayerResponseDTO { Forbidden = true, ErrorMessage = "User does not have access" };
            }
            return new PlayerResponseDTO { Player = player };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionKey">SessionKey parameter received from client</param>
        /// <param name="user">User object retrieved with SessionKey (may be null)</param>
        /// <param name="game">Game object retrieved with request (may be null)</param>
        /// <returns></returns>
        public static GameResponseDTO VerifyAccess(string sessionKey, User? user, Game? game)
        {
            if (game == null)
            {
                return new GameResponseDTO { NotFound = true, ErrorMessage = "Invalid game id" };
            }
            if (user == null || user.SessionKey != sessionKey)
            {
                return new GameResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }
            if (game.UserId != user.Id)
            {
                return new GameResponseDTO { Forbidden = true, ErrorMessage = "User does not have access" };
            }
            return new GameResponseDTO { Game = game };
        }
    }
}
