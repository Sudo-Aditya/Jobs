namespace Jobs.API.Models.Domain
{
    public class JobDetailResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Location Location { get; set; }
        public Department Department { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime ClosingDate { get; set; }
    }
}
