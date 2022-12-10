namespace BackEnd.Helpers
{
    public class LikeParam
    {
        private const int MaxPageSize = 50;

        public int UserId { get; set; }

        public int PageNumber { get; set; } = 1;

        public int _PageSize = 5;

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

        public string Perdicate { get; set; } = "liked";

    }
}
