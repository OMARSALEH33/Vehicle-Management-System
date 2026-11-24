
using Microsoft.AspNetCore.Mvc;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Services;
namespace Challenge_1_Build_APIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase
    {
       private readonly IFeatureService featureService;

        public FeaturesController(IFeatureService featureService)
        {
            this.featureService = featureService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeatureResource>>> GetFeature()
        {

            return Ok(await featureService.GetAllAsync());
        }

        //  GET {id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FeatureResource>> GetFeature(int id)
        {
            var result = await featureService.GetByIdAsync(id);

            if (result.Status == ServiceStatus.NotFound)
                return NotFound();

            return Ok(result.Data);
        }

        //  Save
        [HttpPost]
        public async Task<ActionResult<FeatureResource>> CreateFeature([FromBody] SaveFeatureResource featureResource)
        {

            var result = await featureService.CreateAsync(featureResource);

            if (result.Status == ServiceStatus.Conflict)
            {
                ModelState.AddModelError(result.Field!, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetFeature), new { id = result.Data!.Id }, result.Data);
        }

        //  Update: PUT 
        [HttpPut("{id:int}")]
        public async Task<ActionResult<FeatureResource>> UpdateFeature(int id, [FromBody] SaveFeatureResource featureResource)
        {
            var result = await featureService.UpdateAsync(id, featureResource);

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
        public async Task<IActionResult> DeleteFeature(int id)
        {
            var result = await featureService.DeleteAsync(id);

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