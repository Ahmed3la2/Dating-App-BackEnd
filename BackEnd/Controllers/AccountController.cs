using AutoMapper;
using BackEnd.Data;
using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Helpers;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Http;
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
        private readonly DataContext _context;
        private readonly ItokenService _tokenservice;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ItokenService tokenservice, IMapper mapper)
        {
            _context = context;
            _tokenservice = tokenservice;
            _mapper = mapper;
        }

        // Method To Make User Register 
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDTO)
        {
            if (await UserExist(registerDTO.UserName)) return BadRequest("UserName  Is Taken");

            var user = _mapper.Map<AppUser>(registerDTO);

            var hmac = new HMACSHA512();

                user.UserName = registerDTO.UserName;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
                user.PasswordSalt = hmac.Key;
       

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenservice.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        // Method To Make User Login 
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDTO) 
        {
            var user = await _context.Users
                          .Include(p => p.Photos)
                          .SingleOrDefaultAsync(u => u.UserName == loginDTO.UserName);

            if (user == null) return Unauthorized("require UserName");

            var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                UserName = loginDTO.UserName,
                Token = _tokenservice.CreateToken(user),
                photo = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };  
        }

        // Method To Delete User 
        [HttpDelete("deleteuser")]
        public async Task<ActionResult<AppUser>> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }


        // Method Check If User Exist Or Not
        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}
