using System;
using System.Collections.Generic;

namespace BackEnd.DTOS
{
    public class MemberDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public int Age { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhotoUrl { get; set; }

        public string KnownAs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; } = DateTime.Now;

        public string Gender { get; set; }

        public string Indroduction { get; set; }

        public string LookingFor { get; set; }

        public string Intrest { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public ICollection<PhotoDto> Photos { get; set; }
    }
}
