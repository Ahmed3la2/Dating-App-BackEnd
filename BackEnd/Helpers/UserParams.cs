using System;

namespace BackEnd.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        public int _PageSize = 10;

        public int PageSize
        {
            get { return _PageSize; }
            set
            {
               _PageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }

        public string CurrentUserName { get; set; }

        public string Gender { get; set; } = "male";

        public int MiniAge { get; set; } = 0;

        public int MaxAge { get; set; } = 150;

        public string OrderBy { get; set; } = "created";
    }
}
