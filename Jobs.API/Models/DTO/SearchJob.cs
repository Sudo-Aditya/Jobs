namespace Jobs.API.Models.DTO
{
    public class SearchJob
    {
        public string? Q { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int? LocationId { get; set; }
        public int? DepartmentId { get; set; }
    }
}
