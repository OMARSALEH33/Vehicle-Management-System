using AutoMapper;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Core.Models;
using Challenge_1_Build_APIS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace Challenge_1_Build_APIS.Services
{
    public class FeatureService : IFeatureService
    {
        private readonly VegaDbContext context;
        private readonly IMapper mapper;


        public FeatureService(VegaDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<FeatureResource>> GetAllAsync()
        {
            var feature = await context.Features
                .AsNoTracking()
                .OrderBy(f => f.Id)
                .ToListAsync();

            return mapper.Map<List<FeatureResource>>(feature);
        }
        public async Task<ServiceResult<FeatureResource>> GetByIdAsync(int id)
        {
            var feature = await context.Features
                .AsNoTracking()
                .SingleOrDefaultAsync(f => f.Id == id);

            if (feature is null)
            {
                return ServiceResult<FeatureResource>.NotFound();

            }

            return ServiceResult<FeatureResource>.Ok(mapper.Map<FeatureResource>(feature));
        }

        public async Task<ServiceResult<FeatureResource>> CreateAsync(SaveFeatureResource featureResource)
        {
            if(await context.Makes.AnyAsync(f =>f.Name == featureResource.Name))
            {
                return ServiceResult<FeatureResource>.Conflict(nameof(featureResource.Name), "This feature name is already used.");
            }
            var feature = mapper.Map<Feature>(featureResource);

            context.Features.Add(feature);
            await context.SaveChangesAsync();

            return ServiceResult<FeatureResource>.Ok(mapper.Map<FeatureResource>(feature));
        }

        public async Task<ServiceResult<FeatureResource>> UpdateAsync(int id, SaveFeatureResource featureResource)
        {
            var feature = await context.Features.SingleOrDefaultAsync(f => f.Id == id);

            if (feature is null)
            {
                return ServiceResult<FeatureResource>.NotFound();

            }

            if (await context.Features.AnyAsync(f => f.Name == featureResource.Name && f.Id != id))
            {
                return ServiceResult<FeatureResource>.Conflict(nameof(featureResource.Name), "This feature name is already used.");
            }

            mapper.Map(featureResource, feature);
            await context.SaveChangesAsync();

            return ServiceResult<FeatureResource>.Ok(mapper.Map<FeatureResource>(feature));
        }
        public async Task<ServiceResult<FeatureResource>> DeleteAsync(int id)
        {
            var feature = await context.Features.FindAsync(id);

            if (feature is null)
            {
                return ServiceResult<FeatureResource>.NotFound();
            }
            

            context.Features.Remove(feature);
            await context.SaveChangesAsync();

            return ServiceResult<FeatureResource>.Ok(mapper.Map<FeatureResource>(feature));
        }

    }
}