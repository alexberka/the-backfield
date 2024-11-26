namespace TheBackfield.DTOs
{
    public class ResponseDTO
    {
        public bool Unauthorized { get; set; } = false;
        public bool Forbidden { get; set; } = false;
        public bool NotFound { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public bool Error
        {
            get
            { 
                return Unauthorized == true || Forbidden == true || NotFound == true || ErrorMessage != null;
            }
        }
        /// <summary>
        /// Throws error as IResult
        /// <br/>Converts any error data stored in DTO
        /// </summary>
        /// <returns>Error as IResult (if no error tags marked, returns Results.BadRequest(ErrorMessage))</returns>
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
            return Results.BadRequest(ErrorMessage ?? "");
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

        public GameResponseDTO CastToGameResponseDTO()
        {
            return new GameResponseDTO
            {
                NotFound = NotFound,
                Unauthorized = Unauthorized,
                Forbidden = Forbidden,
                ErrorMessage = ErrorMessage
            };
        }
        /// <summary>
        /// Convert instance of ResponseDTO or any class that extends ResponseDTO to instance of type T. Only base class properties are transferred.
        /// </summary>
        /// <typeparam name="T">Class that extends base class ResponseDTO</typeparam>
        /// <returns></returns>
        public T CastTo<T>() where T : ResponseDTO
        {
            T cast = Activator.CreateInstance<T>();

            cast.Unauthorized = Unauthorized;
            cast.NotFound = NotFound;
            cast.Forbidden = Forbidden;
            cast.ErrorMessage = ErrorMessage;

            return cast;
        }
    }
}
