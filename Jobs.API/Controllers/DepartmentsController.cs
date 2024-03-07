using Jobs.API.Data;
using Jobs.API.Models.Domain;
using Jobs.API.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobs.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly JobsDBContext dBContext;

        public DepartmentsController(JobsDBContext context)
        {
            this.dBContext = context;
        }

        //POST /api/v1/departments
        [HttpPost]
        public async Task<IActionResult> AddNewDepartment(AddDepartment department)
        {
            var departmentData = new Department()
            {
                Title = department.Title,
            };
            await dBContext.Departments.AddAsync(departmentData);
            await dBContext.SaveChangesAsync();

            return Ok(departmentData);
        }

        //PUT /api/v1/departments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, AddDepartment addDepartment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDepartment = await dBContext.Departments.FindAsync(id);

            if (existingDepartment == null)
            {
                return NotFound("Department not found.");
            }

            //Update properties of existingDepartment
            existingDepartment.Title = addDepartment.Title;

            dBContext.Departments.Update(existingDepartment);
            await dBContext.SaveChangesAsync();

            return Ok();
        }

        //GET: /api/v1/departments
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            return Ok(await dBContext.Departments.ToListAsync());
        }
    }
}
