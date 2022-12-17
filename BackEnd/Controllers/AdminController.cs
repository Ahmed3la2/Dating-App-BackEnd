using BackEnd.DTOS;
using BackEnd.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public UserManager<AppUser> _userManager { get; }

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public ActionResult GetUsersWithRole()
        {
            var user = _userManager.Users.Include(u => u.UserRoles)
                       .ThenInclude(r => r.Role)
                       .OrderBy(u => u.UserName)
                       .Select(u => new
                       {
                          u.UserName,
                          u.Id,
                          roles = u.UserRoles.Select(r => r.Role.Name)
                       }).ToList();

            return Ok(user);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditUserRoles(string username, [FromQuery] string roles)
        {
            var rolesToArr = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound("Could not find user");

            var userRoles = await _userManager.GetRolesAsync(user);

            var res =  await _userManager.AddToRolesAsync(user, rolesToArr.Except(userRoles));

            if (!res.Succeeded) return BadRequest("Cannot add roles");

            return Ok(await _userManager.GetRolesAsync(user));

        }

        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpGet("photos-to-moderator")]
        public ActionResult GetPhtotsToModerator()
        {
            var user = _userManager.Users.Include(p => p.Photos)
                             .Select(u => new
                             {
                                 u.Id,
                                 u.UserName,
                                 photos = u.Photos.Select(p => new { p.Id, p.Url, p.IsMain, p.AppUserID })
                             })
                            .ToList();

           
            return Ok(user);
        }
    }
}
