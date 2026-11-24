using AutoMapper;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Core.Models;
using Challenge_1_Build_APIS.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Challenge_1_Build_APIS.Services


{

    public class MakeService : IMakeService
    {
        private readonly VegaDbContext context;
        private readonly IMapper mapper;

        public MakeService(VegaDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        // GET

        public async Task<ServiceResult<PagedResult<MakeResource>>> GetAllAsync(int page = 1, int pageSize = 5)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 5 : Math.Min(pageSize, 100);

            var totalCount = await context.Makes.CountAsync();

            var makes = await context.Makes
                .AsNoTracking()
                .OrderBy(m => m.Id)
                .Skip((page - 1)* pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResult = new PagedResult<MakeResource>
            {
                Items = mapper.Map<List<MakeResource>>(makes),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ServiceResult<PagedResult<MakeResource>>.Ok(pagedResult);
        }

        //  GET{id}
        
        public async Task<ServiceResult<MakeResource>> GetByIdAsync(int id)
        {
            var make = await context.Makes
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);

            if (make is null)
            {
                return ServiceResult<MakeResource>.NotFound();

            }

            return ServiceResult<MakeResource>.Ok(mapper.Map<MakeResource>(make));
        }

        //  Save
        
        public async Task<ServiceResult<MakeResource>> CreateAsync(SaveMakeResource makeResource)
        {
            if (await context.Makes.AnyAsync(m => m.Code == makeResource.Code)) 
            { 
                return ServiceResult<MakeResource>.Conflict(nameof(makeResource.Code), "This code is already used.");
            }
            var make = mapper.Map<Make>(makeResource);

            context.Makes.Add(make);
            await context.SaveChangesAsync();

            return ServiceResult<MakeResource>.Ok(mapper.Map<MakeResource>(make));
        }

        //  Update
        
        public async Task<ServiceResult<MakeResource>> UpdateAsync(int id, SaveMakeResource makeResource)
        {
            var make = await context.Makes.SingleOrDefaultAsync(m => m.Id == id);

            if (make is null)
            {
                return ServiceResult<MakeResource>.NotFound();

            }

            if (await context.Makes.AnyAsync(m => m.Code == makeResource.Code && m.Id != id))
            {
                return ServiceResult<MakeResource>.Conflict(nameof(makeResource.Code), "This code is already used.");
            }

            mapper.Map(makeResource, make);
            await context.SaveChangesAsync();

            return ServiceResult<MakeResource>.Ok(mapper.Map<MakeResource>(make));
        }

        //  Delete:
        
        public async Task<ServiceResult<MakeResource>> DeleteAsync(int id)
        {
            var make = await context.Makes.FindAsync(id);

            if (make is null)
            {
                return ServiceResult<MakeResource>.NotFound();
            }
            if (await context.Models.AnyAsync(m => m.MakeId == id))
            {
                return ServiceResult<MakeResource>.Conflict("Id", "Cannot delete a make that has models.");
            }
            
            context.Makes.Remove(make);
            await context.SaveChangesAsync();

            return ServiceResult<MakeResource>.Ok(mapper.Map<MakeResource>(make));
        }
    }

}

