namespace TheBackfield.DTOs
{
    public class ResponseDTO
    {
        public bool Unauthorized { get; set; } = false;
        public bool Forbidden { get; set; } = false;
        public bool NotFound { get; set; } = false;
        public string? ErrorMessage { get; set; }
        /// <summary>
        /// Throws error as IResult
        /// Converts any error data stored in DTO
        /// </summary>
        /// <returns>Error as IResult (if no error, returns Results.BadRequest())</returns>
        public IResult ThrowError()
        {
            if (NotFound)
            {
                return Results.NotFound(ErrorMessage);
            }
            if (Unauthorized)
            {
                return Results.Unauthorized();
            }
            if (Forbidden)
            {
                return Results.StatusCode(403);
            }
            return Results.BadRequest();
        }

        public PlayerResponseDTO CastToPlayerResponseDTO()
        {
            return new PlayerResponseDTO
            {
                NotFound = NotFound,
                Unauthorized = Unauthorized,
                Forbidden = Forbidden,
                ErrorMessage = ErrorMessage
            };
        }

        public TeamResponseDTO CastToTeamResponseDTO()
        {
            return new TeamResponseDTO
            {
                NotFound = NotFound,
                Unauthorized = Unauthorized,
                Forbidden = Forbidden,
                ErrorMessage = ErrorMessage
            };
        }
    }
}
