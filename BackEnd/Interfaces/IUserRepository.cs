using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<bool> SaveAllAsync();

        Task<PageList<MemberDto>> GetUsers(UserParams param);

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByNameAsync(string name);


    }
}
