using BackEnd.Entities;
using System.Threading.Tasks;

namespace BackEnd.Interfaces
{
    public interface ItokenService
    {
        Task<string> CreateToken(AppUser appUser);

    }
}
