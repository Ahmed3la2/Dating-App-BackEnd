using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Helpers;
using BackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        // Get DataBase => context
        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Get User By Id
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users
                 .Include(p => p.Photos)
                 .Include(l => l.LikedUsers)
                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Get User By UserName
        public async Task<AppUser> GetUserByNameAsync(string Username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == Username);
        }

        // Get All Users
        public  async Task<PageList<MemberDto>> GetUsers(UserParams param)
        {
            /*var query = _context.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)*/
            var miniAge = DateTime.Today.AddYears(-param.MaxAge);
            var maxAge  = DateTime.Today.AddYears(-param.MiniAge - 1);

            IQueryable<MemberDto> query = _context.Users
                        .Select(user => new MemberDto
                        {
                            Id = user.Id,
                            Age = user.GetAge(),
                            City = user.City,
                            Country = user.Country,
                            LastActive = user.LastActive,
                            Created = user.Created,
                            Intrest = user.Intrest,
                            KnownAs = user.KnownAs,
                            LookingFor = user.LookingFor,
                            UserName = user.UserName,
                            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain == true).Url,
                            Gender = user.Gender,
                            Indroduction = user.Indroduction,
                            DateOfBirth = user.DateOfBirth,
                            Photos = (ICollection<PhotoDto>)user.Photos.Select(p => new PhotoDto
                            {
                                IsMain = p.IsMain,
                                Id = p.Id,
                                Url = p.Url,
                            })
                        }).AsQueryable()
                        .Where(u => u.Gender == param.Gender)
                        .Where(u => u.UserName != param.CurrentUserName)
                        .Where(u => u.DateOfBirth >= miniAge && u.DateOfBirth <= maxAge);

            query = param.OrderBy switch
            {
                "created" => query.OrderByDescending(query => query.Created),
                 _ => query.OrderByDescending(query => query.LastActive)
            };
            return await PageList<MemberDto>.CreateAsync(query, param.PageSize, param.PageNumber);
        }

        // Save Change in DataBase
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // Make User Table State Modified
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

    
    }
} 
