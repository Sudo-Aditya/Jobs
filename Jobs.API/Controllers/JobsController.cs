using Jobs.API.Data;
using Jobs.API.Models.Domain;
using Jobs.API.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobs.API.Controllers
{
    //https://localhost:portnumber/api/v1/Jobs
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly JobsDBContext dBContext;

        public JobsController(JobsDBContext context)
        {
            this.dBContext = context;
        }



        public static string GenerateRandomCode()
        {
            // Create a Random object seeded with the current time for better randomness
            Random random = new Random(DateTime.Now.Millisecond);

            // Define the character pool for generating the random string
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Generate the random 4-digit alphanumeric string
            char[] randomChars = new char[4];
            for (int i = 0; i < 4; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            // Create the final job ID string
            return "JOB-" + new string(randomChars);
        }

        //GET: https://localhost:portnumber/api/Jobs
        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] AddJob job)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return bad request if validation fails
            }

            var jbMdel = new JobDepartmentLocatinLink
            {
                Title = job.Title,
                Description = job.Description,
                LocationId = job.LocationId,
                DepartmentId = job.DepartmentId,
                ClosingDate = job.ClosingDate,
            };

            dBContext.Jobs.Add(jbMdel);
            await dBContext.SaveChangesAsync();

            var jbDt = new AddJob
            {
                Title = jbMdel.Title,
                Description = jbMdel.Description,
                LocationId = jbMdel.LocationId,
                DepartmentId = jbMdel.DepartmentId,
                ClosingDate = jbMdel.ClosingDate,
            };

            return CreatedAtAction(
                "GetJob", // Name of the route for retrieving a specific job
                new { id = jbMdel.Id }, // Route parameters
                jbDt); // Return the newly created job object
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, JobsController job)
        {
            //if (id != job.Id)
            //{
            //    return BadRequest("Job ID in request body and URL do not match.");
            //}

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var existingJob = await _context.Jobs.FindAsync(id);

            //if (existingJob == null)
            //{
            //    return NotFound("Job not found.");
            //}

            // Update properties of existingJob
            //existingJob.Title = job.Title;
            //existingJob.Description = job.Description;
            //existingJob.LocationId = job.LocationId;
            //existingJob.DepartmentId = job.DepartmentId;
            //existingJob.ClosingDate = job.ClosingDate;

            //_context.Jobs.Update(existingJob);
            //await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content on successful update
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetJobs([FromBody] JobListRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var query = dBContext.Jobs.AsQueryable();

            // Apply filters based on request parameters
            if (!string.IsNullOrEmpty(request.Q))
            {
                query = query.Where(j => j.Title.Contains(request.Q) || j.Description.Contains(request.Q));
            }

            if (request.LocationId.HasValue)
            {
                query = query.Where(j => j.LocationId == request.LocationId.Value);
            }

            if (request.DepartmentId.HasValue)
            {
                query = query.Where(j => j.DepartmentId == request.DepartmentId.Value);
            }

            //// Pagination
            int total = await query.CountAsync();
            int skip = (request.PageNo - 1) * request.PageSize;
            query = query.Skip(skip).Take(request.PageSize);

            var jobs = await query.Select(j => new JobListItem
            {
                Id = j.Id,
                Code = GenerateRandomCode(), // Assuming you have an auto-generated code field
                Title = j.Title,
                Location = j.Location.Title, // Assuming Location has a Title property
                Department = j.Department.Title, // Assuming Department has a Title property
                PostedDate = j.PostedDate,
                ClosingDate = j.ClosingDate
            }).ToListAsync();

            return Ok(new { total, data = jobs
                            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(int id)
        {
            var job = await dBContext.Jobs
                .Include(j => j.Location)
                .Include(j => j.Department)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                return NotFound("Job not found.");
            }

            return Ok(new JobDetailResponse
            {
                Id = job.Id,
                Code = GenerateRandomCode(), // Assuming you have an auto-generated code field
                Title = job.Title,
                Description = job.Description,
                Location = new Location
                {
                    Id = job.Location.Id,
                    Title = job.Location.Title,
                    City = job.Location.City,
                    State = job.Location.State,
                    Country = job.Location.Country,
                   Zip = job.Location.Zip
                },
                Department = new Department
                {
                    Id = job.Department.Id,
                    Title = job.Department.Title
                },
                PostedDate = job.PostedDate,
                ClosingDate = job.ClosingDate
            });
        }
    }
}
