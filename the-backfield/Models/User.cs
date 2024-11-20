namespace TheBackfield.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedOn { get; set; }
    public string SessionKey { get; set; } = "";
    public DateTime SessionStart { get; set; } = DateTime.MinValue;
    public string Uid { get; set; }
}