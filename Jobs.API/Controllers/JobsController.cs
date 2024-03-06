﻿using Jobs.API.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobs.API.Controllers
{
    //https://localhost:portnumber/api/v1/Jobs
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        //private readonly TeknorixDbContext _context;

        //public JobsController(TeknorixDbContext context)
        //{
        //    _context = context;
        //}

        //GET: https://localhost:portnumber/api/Jobs
        [HttpPost]
        public async Task<IActionResult> CreateJob(Jobs job)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return bad request if validation fails
            }

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtRoute(
                "GetJob", // Name of the route for retrieving a specific job
                new { id = job.Id }, // Route parameters
                job); // Return the newly created job object
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, Jobs job)
        {
            if (id != job.Id)
            {
                return BadRequest("Job ID in request body and URL do not match.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingJob = await _context.Jobs.FindAsync(id);

            if (existingJob == null)
            {
                return NotFound("Job not found.");
            }

            // Update properties of existingJob
            existingJob.Title = job.Title;
            existingJob.Description = job.Description;
            existingJob.LocationId = job.LocationId;
            existingJob.DepartmentId = job.DepartmentId;
            existingJob.ClosingDate = job.ClosingDate;

            _context.Jobs.Update(existingJob);
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content on successful update
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetJobs([FromBody] JobListRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var query = _context.Jobs.AsQueryable();

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

            // Pagination
            int total = await query.CountAsync();
            int skip = (request.PageNo - 1) * request.PageSize;
            query = query.Skip(skip).Take(request.PageSize);

            var jobs = await query.Select(j => new JobListItem
            {
                Id = j.Id,
                Code = j.Code, // Assuming you have an auto-generated code field
                Title = j.Title,
                Location = j.Location.Title, // Assuming Location has a Title property
                Department = j.Department.Title, // Assuming Department has a Title property
                PostedDate = j.PostedDate,
                ClosingDate = j.ClosingDate
            }).ToListAsync();

            return Ok(new { total, data = jobs });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(int id)
        {
            var job = await _context.Jobs
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
                Code = job.Code, // Assuming you have an auto-generated code field
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
