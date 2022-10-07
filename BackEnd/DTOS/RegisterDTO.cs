using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOS
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
