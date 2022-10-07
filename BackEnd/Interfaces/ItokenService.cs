using BackEnd.Entities;

namespace BackEnd.Interfaces
{
    public interface ItokenService
    {
        string CreateToken(AppUser appUser);
    }
}
