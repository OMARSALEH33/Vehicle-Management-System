using AutoMapper;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Core.Models;
using Challenge_1_Build_APIS.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Challenge_1_Build_APIS.Services
{
    public class ModelService : IModelService
    {
        private readonly VegaDbContext context;
        private readonly IMapper mapper;


        public ModelService(VegaDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ServiceResult<PagedResult<ModelResource>>> GetAllAsync(int? makeId, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 5 : Math.Min(pageSize, 100);

            var query = context.Models.AsNoTracking().AsQueryable();

            if (makeId.HasValue)    
                query = query.Where(m => m.MakeId == makeId.Value);
                query = query.OrderBy(m => m.Id);

            var totalCount = await query.CountAsync();

            var models = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
          
            var pagedResult = new PagedResult<ModelResource>
            {
                Items = mapper.Map<List<ModelResource>>(models),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ServiceResult<PagedResult<ModelResource>>.Ok(pagedResult);
        }
        public async Task<ServiceResult<ModelResource>> GetByIdAsync(int id)
        {
            var model = await context.Models
                 .AsNoTracking()
                 .SingleOrDefaultAsync(m => m.Id == id);

            if (model is null)
                return ServiceResult<ModelResource>.NotFound();

            return ServiceResult<ModelResource>.Ok(mapper.Map<ModelResource>(model));
        }
        public async Task<ServiceResult<ModelResource>> CreateAsync(SaveModelResource modelResource)
        {
            if (!await context.Makes.AnyAsync(m => m.Id == modelResource.MakeId))
            {
                return ServiceResult<ModelResource>.Conflict(nameof(modelResource.MakeId), "Invalid MakeId.");
            }

            if (await context.Models.AnyAsync(m => m.Name == modelResource.Name))
            {
                return ServiceResult<ModelResource>.Conflict(nameof(modelResource.Name), "This model name already exists.");

            }

            var model = mapper.Map<Model>(modelResource);

            context.Models.Add(model);
            await context.SaveChangesAsync();

            return ServiceResult<ModelResource>.Ok(mapper.Map<ModelResource>(model));
        }

        public async Task<ServiceResult<ModelResource>> UpdateAsync(int id, UpdateModelResource modelResource)
        {
            var model = await context.Models.SingleOrDefaultAsync(m => m.Id == id);

            if (model is null)
                return ServiceResult<ModelResource>.NotFound();

            if (await context.Models.AnyAsync(m => m.Name == modelResource.Name && m.Id != id))
            {
                return ServiceResult<ModelResource>.Conflict(nameof(modelResource.Name), "This model name already exists.");
            }

            mapper.Map(modelResource, model);
            await context.SaveChangesAsync();

            return ServiceResult<ModelResource>.Ok(mapper.Map<ModelResource>(model));
        }
        public async Task<ServiceResult<ModelResource>> DeleteAsync(int id)
        {
            var model = await context.Models.FindAsync(id);

            if (model is null)
                return ServiceResult<ModelResource>.NotFound();

            context.Models.Remove(model);
            await context.SaveChangesAsync();

            return ServiceResult<ModelResource>.Ok(mapper.Map<ModelResource>(model));
        }

      
    }
}
