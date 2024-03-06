namespace Jobs.API.Models.Domain
{
    public class Jobs
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public DateTime ClosingDate { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
