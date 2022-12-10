using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd.Interfaces
{
    public interface ILikeRepository
    {
          Task<UserLike>  GetUserLikAsynce(int SourceUserId, int LikedUserId);

          Task<AppUser> GetUserWithLikesAsync(int UserId);

          Task<PageList<LikeDto>> GetUserLikes(LikeParam likeParams, int UserId);
    }
}
