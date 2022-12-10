using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Entities
{
    public class UserLike
    {

        public AppUser SourceUser { get; set; }

        public int SourceUserId { get; set; }

        public AppUser LkedUser { get; set; }

        public int LkedUserID { get; set; }
    }
}
