using Microsoft.EntityFrameworkCore;
using Jobs.API.Models.Domain;

namespace Jobs.API.Data
{
    public class JobsDBContext: DbContext
    {
        public JobsDBContext(DbContextOptions dbContextOptions): base(dbContextOptions)
        {
                
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<JobDetailResponse> JobDetailResponses { get; set; }
        public DbSet<JobListItem> JobListItems { get; set; }
        public DbSet<JobListRequest> JobListRequests { get; set; }
        public DbSet<Location> Locations { get; set; }
        //public DbSet<Jobs> Jobs { get; set; }

    }
}
