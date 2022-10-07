using BackEnd.Data;
using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private DataContext _context;
        private ItokenService _token;

        public UserController(DataContext context, ItokenService token)
        {
            _context = context;
            _token = token;
        }

        // Get All Users
        [HttpGet("allusers")]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            List<UserDto> user = new List<UserDto>();

           for(int i = 0; i < users.Count; i++)
            {
                user.Add(new UserDto
                {
                    UserName = users[i].UserName,
                    Token = _token.CreateToken(users[i])
                });
            }

           return user;
            
        }

        // Get User By Id 
        [Authorize]
        [HttpGet("{id}")]
        public async Task<AppUser> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        // Add User
        [HttpPost("adduser")]
        public async Task<AppUser> AddUser(AppUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    } 
}
