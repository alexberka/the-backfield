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
        /// <param name="sessionKey"></param>
        /// <param name="user"></param>
        /// <param name="team"></param>
        /// <returns>ResponseDTO with Response = Team if access granted, Error = IResult if access denied </returns>
        public static ResponseDTO VerifyAccess(string sessionKey, User? user, Team? team)
        {
            if (team == null)
            {
                return new ResponseDTO { NotFound = true, ErrorMessage = "Invalid team id" };
            }
            if (user == null || user.SessionKey != sessionKey)
            {
                return new ResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
            }
            if (team.UserId != user.Id)
            {
                return new ResponseDTO { Forbidden = true, ErrorMessage = "User does not have access" };
            }
            return new ResponseDTO { Resource = team, ResourceId = team.Id };
        }
    }
}
