using BackEnd.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Entities
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

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

        public int GetAge()
        {
            return DateOfBirth.CalcuateAge();
        }



    }
}
