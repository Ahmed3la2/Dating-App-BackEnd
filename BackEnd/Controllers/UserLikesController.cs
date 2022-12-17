using AutoMapper;
using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Extensions;
using BackEnd.Helpers;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]


    public class UserLikesController : ControllerBase
    {
        private readonly ILikeRepository _like;
        private readonly IUserRepository _userRepository;

        public UserLikesController(ILikeRepository like, IUserRepository userRepository)
        {
            _like = like;
            _userRepository = userRepository;
        }

        //Add User Like
        [HttpPost("{likedId}")]
        public async Task<ActionResult> AddLike(int likedId)
        {
            //Get Current User
            var Id = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userRepository.GetUserByIdAsync(Id);

            //Get User Like, Check If Current User = LikedUser && If User Like LikedUser Before
            var userLike = await _like.GetUserLikAsynce(Id, likedId);
            if (userLike != null) return BadRequest("Cannot Like User Twice");
            if (user.Id == likedId) return BadRequest("You Cannot Like YourSelf");
                    
            user.LikedUsers.Add(new UserLike
            {
                LkedUserID = likedId,
                SourceUserId = user.Id
            });

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Faild To Add Like");

        }

        //Get Current User And See The Users Who Like And The Users Liked Him
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserLike>>> GetUserLike([FromQuery] LikeParam like, int UserId)
        {
            var UseriD  = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (like.Perdicate != "liked" && like.Perdicate != "LikedByUsers") return BadRequest("likes Perdicate UnKnown");

            var userlike = await _like.GetUserLikes(like, UseriD);

            Response.AddPaginationHeader(userlike.CurrentPage,userlike.PageSize, userlike.Count,userlike.TotalCount);

            return Ok(userlike);
        }
    }

}
