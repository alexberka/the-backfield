using TheBackfield.DTOs.GameStream;

namespace TheBackfield.Interfaces
{
    public interface IWatchClient
    {
        Task SayHello(string greeting);
        Task UpdateGameStream(GameStreamDTO gameStream);
    }
}
