
using Microsoft.AspNetCore.Mvc;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Challenge_1_Build_APIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MakesController : ControllerBase
    {
        private readonly IMakeService makeService;

        public MakesController(IMakeService makeService)
        {
            this.makeService = makeService;
        }

        // GET 
        [HttpGet]
        public async Task<ActionResult<PagedResult<MakeResource>>> GetMakes([FromQuery] int page = 1,[FromQuery] int pageSize = 5)
        {
            var result = await makeService.GetAllAsync(page, pageSize);
            return Ok(result.Data);
        }

        //  GET {id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MakeResource>> GetMake(int id)
        {
            var result = await makeService.GetByIdAsync(id);

            if (result.Status == ServiceStatus.NotFound)
                return NotFound();

            return Ok(result.Data);
        }

        //  Save
        [HttpPost]
        public async Task<ActionResult<MakeResource>> CreateMake([FromBody] SaveMakeResource makeResource)
        {
           
            var result = await makeService.CreateAsync(makeResource);

            if (result.Status == ServiceStatus.Conflict)
            {
                ModelState.AddModelError(result.Field!, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetMake), new { id = result.Data!.Id }, result.Data);
        }

        //  Update: PUT api/makes/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<MakeResource>> UpdateMake(int id, [FromBody] SaveMakeResource makeResource)
        {
            var result = await makeService.UpdateAsync(id, makeResource);

            if (result.Status == ServiceStatus.NotFound)
                return NotFound();

            if (result.Status == ServiceStatus.Conflict)
            {
                ModelState.AddModelError(result.Field!, result.Error!);
                return ValidationProblem(ModelState);
            }


            return Ok(result.Data);
        }

        //  Delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMake(int id)
        {
            var result = await makeService.DeleteAsync(id);

            if (result.Status == ServiceStatus.NotFound)
                return NotFound();

            if (result.Status == ServiceStatus.Conflict)
                return Conflict(new ProblemDetails
                {
                    Title = result.Error,
                    Status = StatusCodes.Status409Conflict
                });

            return NoContent();
        }
    }
}