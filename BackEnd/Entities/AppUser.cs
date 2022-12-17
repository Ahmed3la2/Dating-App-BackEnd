using BackEnd.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BackEnd.Entities
{
    public class AppUser : IdentityUser<int>
    {

        public DateTime DateOfBirth { get; set; } 

        public string KnownAs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public string Gender { get; set; }

        public string Indroduction { get; set; }

        public string LookingFor { get; set; }

        public string Intrest { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }

        public ICollection<UserLike> LikesdByUsers { get; set; }

        public ICollection<UserLike> LikedUsers { get; set; }

        public ICollection<Message> MessageSent { get; set; }

        public ICollection<Message> MessageRecevied { get; set; }

        public ICollection<AppUserRole> UserRoles { get; set; }


        public int GetAge()
        {
            return DateOfBirth.CalcuateAge();
        }



    }
}
