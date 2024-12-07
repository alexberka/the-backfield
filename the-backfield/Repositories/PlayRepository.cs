using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Repositories
{
    public class PlayRepository : IPlayRepository
    {
        public Task<Play?> CreatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }

        public Task<string?> DeletePlayAsync(int playId)
        {
            throw new NotImplementedException();
        }

        public Task<Play?> GetSinglePlayAsync(int playId)
        {
            throw new NotImplementedException();
        }

        public Task<Play?> UpdatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }
    }
}
