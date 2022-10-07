using BackEnd.Data;
using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ItokenService _tokenservice;

        public AccountController(DataContext context, ItokenService tokenservice)
        {
            _context = context;
            _tokenservice = tokenservice;
        }

        // Method To Make User Register 
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDTO)
        {
            if (await UserExist(registerDTO.UserName)) return BadRequest("UserName Is Taken");

            var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDTO.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenservice.CreateToken(user)
            };
        }

        // Method To Delete User 
        [HttpDelete("deleteuser")]
        public async Task<ActionResult<AppUser>> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);

            return user;
        }

        // Method To Make User Login 
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDTO) 
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDTO.UserName);

            if (user == null) return Unauthorized("require UserName");

            var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                UserName=loginDTO.UserName,
                Token = _tokenservice.CreateToken(user)
            };
        }

        // Method Check If User Exist Or Not
        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}
