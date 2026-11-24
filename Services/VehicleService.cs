using AutoMapper;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Core.Models;
using Challenge_1_Build_APIS.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Challenge_1_Build_APIS.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly VegaDbContext context;
        private readonly IMapper mapper;

        public VehicleService(VegaDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ServiceResult<PagedResult<VehicleResource>>> GetAllAsync(int page = 1, int pageSize = 5)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 5 : Math.Min(pageSize, 100);

            var query = context.Vehicles
                .Include(v => v.Model)
                    .ThenInclude(m => m.Make)
                .Include(v => v.Features)
                    .ThenInclude(vf => vf.Feature)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var vehicles = await query
                .OrderBy(v => v.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResult = new PagedResult<VehicleResource>
            {
                Items = mapper.Map<List<VehicleResource>>(vehicles),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ServiceResult<PagedResult<VehicleResource>>.Ok(pagedResult);
        }

        public async Task<ServiceResult<VehicleResource>> GetByIdAsync(int id)
        {
            var vehicle = await context.Vehicles
                .Include(v => v.Model)
                    .ThenInclude(m => m.Make)
                .Include(v => v.Features)
                    .ThenInclude(vf => vf.Feature)
                .AsNoTracking()
                .SingleOrDefaultAsync(v => v.Id == id);

            if (vehicle is null)
            {
                return ServiceResult<VehicleResource>.NotFound();
            }

            return ServiceResult<VehicleResource>.Ok(mapper.Map<VehicleResource>(vehicle));
        }

        public async Task<ServiceResult<VehicleResource>> CreateAsync(SaveVehicleResource resource)
        {
            var makeName = resource.MakeName?.Trim();
            if (string.IsNullOrEmpty(makeName))
            {
                return ServiceResult<VehicleResource>.Conflict(nameof(resource.MakeName), "Make name is required.");
            }

            var make = await context.Makes
                .SingleOrDefaultAsync(m => m.Name.ToLower() == makeName.ToLower());

            if (!string.IsNullOrWhiteSpace(resource.Code))
            {
                var existingMakeWithCode = resource.Code.Trim();
                    bool isCodeUsedByOthers = await context.Makes
                    .AnyAsync(m => m.Code == existingMakeWithCode && m.Name.ToLower() != makeName.ToLower());
                if (isCodeUsedByOthers)
                {
                    return ServiceResult<VehicleResource>.Conflict(nameof(resource.Code), "This code is already taken by another make. Try a different one!");
                }
            }

            if (make == null)
            {
                make = new Make
                {
                    Name = makeName,
                    Code = string.IsNullOrWhiteSpace(resource.Code)
                        ? (makeName.Length >= 3 ? makeName.Substring(0, 3).ToUpper() : makeName.ToUpper())
                        : resource.Code.Trim()
                };
                context.Makes.Add(make);
                await context.SaveChangesAsync();
            }

            var modelName = resource.ModelName?.Trim();
            if (string.IsNullOrEmpty(modelName))
            {
                return ServiceResult<VehicleResource>.Conflict(nameof(resource.ModelName), "Model name is required.");
            }

            var model = await context.Models
                .SingleOrDefaultAsync(m => m.Name.ToLower() == modelName.ToLower() && m.MakeId == make.Id);

            if (!string.IsNullOrWhiteSpace(resource.ModelName))
            {

                var modelNameToCheck = resource.ModelName.Trim();

            }

            if (model == null)
            {
                model = new Model { Name = modelName, MakeId = make.Id };
                context.Models.Add(model);
                await context.SaveChangesAsync(); 
            }

            var vehicle = mapper.Map<Vehicle>(resource);
            vehicle.ModelId = model.Id;
            vehicle.LastUpdate = DateTime.Now;

            if (resource.Features != null)
            {
                vehicle.Features = resource.Features.Select(id => new VehicleFeature
                {
                    FeatureId = id
                }).ToList();
            }

            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();

            return await GetByIdAsync(vehicle.Id);
        }

        public async Task<ServiceResult<VehicleResource>> UpdateAsync(int id, SaveVehicleResource resource)
        {
            var vehicle = await context.Vehicles
                .Include(v => v.Features)
                .SingleOrDefaultAsync(v => v.Id == id);

            if (vehicle is null)
            {
                return ServiceResult<VehicleResource>.NotFound();
            }

            var makeName = resource.MakeName?.Trim();
            if (string.IsNullOrEmpty(makeName))
            {
                return ServiceResult<VehicleResource>.Conflict(nameof(resource.MakeName), "Make name is required.");
            }

            var make = await context.Makes
                .SingleOrDefaultAsync(m => m.Name.ToLower() == makeName.ToLower());

            if (make == null)
            {
                make = new Make
                {
                    Name = makeName,
                    Code = string.IsNullOrWhiteSpace(resource.Code)
                        ? (makeName.Length >= 3 ? makeName.Substring(0, 3).ToUpper() : makeName.ToUpper())
                        : resource.Code.Trim()
                };
                context.Makes.Add(make);
                await context.SaveChangesAsync();
            }

            var modelName = resource.ModelName?.Trim();
            if (string.IsNullOrEmpty(modelName))
            {
                return ServiceResult<VehicleResource>.Conflict(nameof(resource.ModelName), "Model name is required.");
            }

            var model = await context.Models
                .SingleOrDefaultAsync(m => m.Name.ToLower() == modelName.ToLower() && m.MakeId == make.Id);

            if (model == null)
            {
                model = new Model { Name = modelName, MakeId = make.Id };
                context.Models.Add(model);
                await context.SaveChangesAsync();
            }

            mapper.Map(resource, vehicle);

            vehicle.ModelId = model.Id;
            vehicle.LastUpdate = DateTime.Now;

            vehicle.Features.Clear();
            if (resource.Features != null)
            {
                foreach (var featureId in resource.Features)
                {
                    vehicle.Features.Add(new VehicleFeature
                    {
                        VehicleId = vehicle.Id,
                        FeatureId = featureId
                    });
                }
            }

            await context.SaveChangesAsync();

            return await GetByIdAsync(vehicle.Id);
        }

        public async Task<ServiceResult<VehicleResource>> DeleteAsync(int id)
        {
            var vehicle = await context.Vehicles
                .Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
                .SingleOrDefaultAsync(v => v.Id == id);

            if (vehicle is null)
            {
                return ServiceResult<VehicleResource>.NotFound();
            }

            context.Vehicles.Remove(vehicle);
            await context.SaveChangesAsync();

            return ServiceResult<VehicleResource>.Ok(mapper.Map<VehicleResource>(vehicle));
        }
    }
}