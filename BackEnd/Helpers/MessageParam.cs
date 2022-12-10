namespace BackEnd.Helpers
{
    public class MessageParam
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
      
        public string UserName { get; set; }

        public string Container { get; set; } = "Unread";
    }
}
