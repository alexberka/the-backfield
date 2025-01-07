namespace TheBackfield.DTOs
{
    public class ResponseDTO<T>
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
        public T? Resource { get; set; }
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
        /// <summary>
        /// Convert instance of ResponseDTO or any class that extends ResponseDTO to instance of type K. Only base class properties are transferred.
        /// </summary>
        /// <typeparam name="K">New generic type</typeparam>
        /// <returns>ResponseDTO&lt;<typeparamref name="K"/>&gt;</returns>
        public ResponseDTO<K> ToType<K>()
        {
            ResponseDTO<K> cast = Activator.CreateInstance<ResponseDTO<K>>();

            cast.Unauthorized = Unauthorized;
            cast.NotFound = NotFound;
            cast.Forbidden = Forbidden;
            cast.ErrorMessage = ErrorMessage;

            return cast;
        }
    }
}
