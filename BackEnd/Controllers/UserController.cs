using AutoMapper;
using BackEnd.Data;
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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        // Get DataBase And UserRepository And PhotoRepository
        public UserController(DataContext context, IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        // Get All Users
        [Authorize(Roles = "Admin")]
        [HttpGet("allusers")]
        public async Task< ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams param)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            var user =  await _userRepository.GetUserByNameAsync(userName);

            param.CurrentUserName =  user.UserName;
          
            var users =  await _userRepository.GetUsers(param);

            Response.AddPaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);

            return Ok(users);      
        }

        // Get User By Id 
        [HttpGet("{id}")]
        public async Task<MemberDto> GetUser(int id)
        {
             var user =  await _userRepository.GetUserByIdAsync(id);
             var userToReturn = _mapper.Map<MemberDto>(user);
             return userToReturn;
        }
        
        // Get User By UserName
        [HttpGet("user{username}", Name = "GetUser")]
        public async Task<MemberDto> GetUser(string username)
        {
            var user = await _userRepository.GetUserByNameAsync(username);
            var userToReturn = _mapper.Map<MemberDto>(user);
            return userToReturn;
        }

        // Add User
        [HttpPost("adduser")]
        public async Task<AppUser> AddUser(AppUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        //Update user Data
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto MemberUpdateDto)
        {
            // Get The UserName From The Token 
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _userRepository.GetUserByNameAsync(userName);


            // this is instead to ==>
                                  _mapper.Map(MemberUpdateDto, user);

           // to do this ==>
                            // user.Country = MemberUpdateDto.Country;
                            // user.City = MemberUpdateDto.City;
                            // user.Intrest = MemberUpdateDto.Intrest;
                            // user.LookingFor = MemberUpdateDto.LookingFor;



            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("failed To Update User");

        }

        // Add Photo To User
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByNameAsync(userName);    
            var Result = await _photoService.AddImageAsync(file);
            if (Result.Error != null) return BadRequest();
            var photo = new Photo
            {
                Url = Result.SecureUrl.AbsoluteUri,
                PublicId = Result.PublicId
            }; 
            if(user.Photos.Count == 0)
            {
               photo.IsMain = true;
            } 
            user.Photos.Add(photo);
            if(await _userRepository.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Prolem Adding Photo");
        }

        // Set Main Photo 
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByNameAsync(userName);

            var mainPhoto =  user.Photos.FirstOrDefault(x => x.IsMain);
            mainPhoto.IsMain = false;
            
            var NewMainPhoto = user.Photos.FirstOrDefault(x => x.Id == photoId);
            NewMainPhoto.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed To Set Main Photo");

        }

        // Remove Image from DataBase And cloudinary
        [HttpDelete("delete-photo/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByNameAsync(userName);

            var Photo = user.Photos.FirstOrDefault(x => x.Id == id);

            if (Photo == null) return NotFound();

            if (Photo.IsMain) return BadRequest("You Cnnot Remove Your Main Photo");

            var result = await _photoService.DeleteImageAsync(Photo.PublicId); 

            if(result.Error != null) return BadRequest(result.Error.Message);

            user.Photos.Remove(Photo);

            await _userRepository.SaveAllAsync();

            return NoContent();
        }

    } 
}
