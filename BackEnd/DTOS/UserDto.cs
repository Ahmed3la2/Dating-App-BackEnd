using BackEnd.Entities;

namespace BackEnd.DTOS
{
    public class UserDto
    {
        public string UserName { get; set; }

        public string Token { get; set; }

        public string photo { get; set; } 

        public string KnownAs { get; set; } 

        public string Gender { get; set; } 
    }
}
