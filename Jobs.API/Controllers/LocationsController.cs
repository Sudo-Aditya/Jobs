using Jobs.API.Data;
using Jobs.API.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Jobs.API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Jobs.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly JobsDBContext dBContext;

        public LocationsController(JobsDBContext context)
        {
            this.dBContext = context;
        }

        //POST /api/v1/locations
        [HttpPost]
        public async Task<IActionResult> AddNewLocation(AddLocation location)
        {
            var locationData = new Location()
            {
                Title = location.Title,
                City = location.City,   
                State = location.State, 
                Country = location.Country, 
                Zip = location.Zip,
            };
            await dBContext.Locations.AddAsync(locationData);
            await dBContext.SaveChangesAsync();

            return Ok(locationData);
        }

        //PUT /api/v1/locations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, AddLocation addLocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingLocation = await dBContext.Locations.FindAsync(id);

            if (existingLocation == null)
            {
                return NotFound("Location not found.");
            }

            //Update properties of existingLocation
            existingLocation.Title = addLocation.Title;
            existingLocation.City = addLocation.City;
            existingLocation.State = addLocation.State;
            existingLocation.Country = addLocation.Country;
            existingLocation.Zip = addLocation.Zip;

            dBContext.Locations.Update(existingLocation);
            await dBContext.SaveChangesAsync();

            return Ok(); 
        }

        //GET: /api/v1/locations
        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            return Ok(await dBContext.Locations.ToListAsync());
        }
    }
}
