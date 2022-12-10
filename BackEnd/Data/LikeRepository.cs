using BackEnd.Data;
using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Helpers;
using BackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext _context;
        private IQueryable<AppUser> _users;

        //Get DataBase => _context
        public LikeRepository(DataContext context)
        {
            _context = context;
        }

        //Get User Like
        public async Task<UserLike> GetUserLikAsynce(int SourceUserId, int LkedUserID)
        {
            var like = await _context.likes.FirstOrDefaultAsync(s => s.SourceUserId == SourceUserId && s.LkedUserID == LkedUserID);
            return like;
        }

        //Get List Of Users That The User Like Them OR Get List Of Users That liked The Main User 
        public  async Task<PageList<LikeDto>> GetUserLikes(LikeParam likeParams,  int UserId)
        {
            var like =  _context.likes.Include(l=>l.SourceUser).Include(l=>l.LkedUser).AsQueryable();

            if(likeParams.Perdicate == "liked")  _users = like.Where(l => l.SourceUserId == UserId).Select(l => l.LkedUser);
            
            if(likeParams.Perdicate == "LikedByUsers")  _users = like.Where(l => l.LkedUserID == UserId).Select(l => l.SourceUser);
          
            var pageListOfUsers =  _users.Select(u => new LikeDto
            {
                Age=u.GetAge(),
                City=u.City,
                Id=u.Id,
                UserName=u.UserName,
                KnownAs=u.KnownAs,
                PhotoUrl=u.Photos.FirstOrDefault(u => u.IsMain == true).Url

            }).AsQueryable();

            return await PageList<LikeDto>.CreateAsync(pageListOfUsers, likeParams.PageSize, likeParams.PageNumber);
        }

        //Get User Include Liked Users
        public async Task<AppUser> GetUserWithLikesAsync(int UserId)
        {
            return await _context.Users.Include(p => p.LikedUsers).FirstOrDefaultAsync(u => u.Id == UserId);
        }
    }
}
