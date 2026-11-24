using Microsoft.AspNetCore.Mvc;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Services;

namespace Challenge_1_Build_APIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            this.vehicleService = vehicleService;
        }

        // GET: 
        [HttpGet]
        public async Task<ActionResult<ServiceResult<PagedResult<VehicleResource>>>> GetVehicles(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            return Ok(await vehicleService.GetAllAsync(page, pageSize));
        }

        // GET{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehicleResource>> GetVehicle(int id)
        {
            var result = await vehicleService.GetByIdAsync(id);

            if (result.Status == ServiceStatus.NotFound)
                return NotFound();

            return Ok(result.Data);
        }

        // POST
        [HttpPost]
        public async Task<ActionResult<VehicleResource>> CreateVehicle([FromBody] SaveVehicleResource resource)
        {
           

            var result = await vehicleService.CreateAsync(resource);

            if (result.Status == ServiceStatus.Conflict)
            {
                ModelState.AddModelError(result.Field!, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetVehicle), new { id = result.Data!.Id }, result.Data);
        }

        // PUT:id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<VehicleResource>> UpdateVehicle(int id, [FromBody] SaveVehicleResource resource)
        {
            var result = await vehicleService.UpdateAsync(id, resource);

            if (result.Status == ServiceStatus.NotFound)
                return NotFound();

            if (result.Status == ServiceStatus.Conflict)
            {
                ModelState.AddModelError(result.Field!, result.Error!);
                return ValidationProblem(ModelState);
            }

            return Ok(result.Data);
        }

        // DELETE:{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var result = await vehicleService.DeleteAsync(id);

            if (result.Status == ServiceStatus.NotFound)
                return NotFound();

            return NoContent();
        }
    }
}