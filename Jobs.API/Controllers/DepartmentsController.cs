using Jobs.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly JobsDBContext dBContext;

        public DepartmentsController(JobsDBContext context)
        {
            this.dBContext = context;
        }

    }
}
