namespace TheBackfield.DTOs
{
    public class ResponseDTO
    {
        public bool Unauthorized { get; set; } = false;
        public bool Forbidden { get; set; } = false;
        public bool NotFound { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public object? Resource { get; set; }
        public int ResourceId { get; set; }
    }
}
