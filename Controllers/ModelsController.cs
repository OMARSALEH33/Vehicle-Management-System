using AutoMapper;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Core.Models;
using Challenge_1_Build_APIS.Persistence;
using Challenge_1_Build_APIS.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Challenge_1_Build_APIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelsController : ControllerBase
    {
        private readonly IModelService modelService;
        

        public ModelsController(IModelService modelService)
        {
           this.modelService = modelService;
        }

        // 1) Get
        [HttpGet]
        public async Task<ActionResult<ServiceResult<PagedResult<ModelResource>>>> GetModels([FromQuery] int? makeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {

            var result = await modelService.GetAllAsync(makeId, page, pageSize);
            return Ok(result.Data);
        }

        // 2) Get {id]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ModelResource>> GetModel(int id)
        {
            var result = await modelService.GetByIdAsync(id);

            if (result.Status == ServiceStatus.NotFound)
            {
                return NotFound();
            }
           
            return Ok(result.Data);
        }

        // 3) Save:
        [HttpPost]
        public async Task<ActionResult<ModelResource>> CreateModel([FromBody] SaveModelResource modelResource)
        {
            var reuslt = await modelService.CreateAsync(modelResource);

            if(reuslt.Status == ServiceStatus.Conflict)
            {
                ModelState.AddModelError(reuslt.Field!, reuslt.Error!);
                return ValidationProblem(ModelState);
            }
            return CreatedAtAction(nameof(GetModel), new { id = reuslt.Data!.Id }, reuslt.Data);
        }

        // 4) Update
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ModelResource>> UpdateModel(int id, [FromBody] UpdateModelResource modelResource)
        {
            var result = await modelService.UpdateAsync(id, modelResource);

            if(result.Status == ServiceStatus.NotFound)
            {
                return NotFound();
            }
            if (result.Status == ServiceStatus.Conflict)
            {
                ModelState.AddModelError(result.Field!, result.Error!);
                return ValidationProblem(ModelState);
            }
            return Ok(result.Data);

        }

        // 5) Delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteModel(int id)
        {
            var result = await modelService.DeleteAsync(id);

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