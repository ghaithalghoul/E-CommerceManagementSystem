namespace E_CommerceManagementSystem.Dto
{
    public class ReviewFilterRequest
    {
        public int? MaxRating { get; set; }
        public int? MinRating { get; set; }
        public string? ProductName { get; set; }
        

        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int page { get; set; } = 1;
        public int pagesize { get; set; } = 10;


    }
}
