using AutoMapper;
using BackEnd.Data;
using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Helpers;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{   [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
     
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ItokenService _tokenservice;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager ,ItokenService tokenservice, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenservice = tokenservice;
            _mapper = mapper;
        }

        // Method To Make User Register 
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDTO)
        {
            if (await UserExist(registerDTO.UserName)) return BadRequest("UserName  Is Taken");

            var user = _mapper.Map<AppUser>(registerDTO);

            user.UserName = registerDTO.UserName.ToLower();

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var Roleresult = await _userManager.AddToRoleAsync(user, "Member");
            if (!Roleresult.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenservice.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        // Method To Make User Login 
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDTO) 
        {
            var user = await _userManager.Users
                          .Include(p => p.Photos)
                          .SingleOrDefaultAsync(u => u.UserName == loginDTO.UserName.ToLower());

            if (user == null) return Unauthorized("require UserName");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if (!result.Succeeded) return Unauthorized();

            return new UserDto
            {
                UserName = loginDTO.UserName,
                Token = await _tokenservice.CreateToken(user),
                photo = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };  
        }

        // Method To Delete User 
        [HttpDelete("deleteuser")]
        public async Task<ActionResult<AppUser>> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest("cannot delete User");
            return user;
        }


        // Method Check If User Exist Or Not
        private async Task<bool> UserExist(string username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}
