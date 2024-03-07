using Jobs.API.Models.Domain;

namespace Jobs.API.Models.DTO
{
    public class AddJob
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime ClosingDate { get; set; }
    }
}
