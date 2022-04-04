using BackEnd.Data;
using BackEnd.Entities;
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

        public UserController(DataContext context)
        {
            _context = context;
        }

        // Get All Users
        [HttpGet]
        public async Task<IEnumerable<AppUser>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        // Get User By Id
        [HttpGet("{id}")]
        public async Task<AppUser> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        // Add User
        [HttpPost]
        public async Task<AppUser> AddUser(AppUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    } 
}
