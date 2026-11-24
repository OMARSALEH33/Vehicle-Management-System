using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Mapping;
using Challenge_1_Build_APIS.Persistence;
using Challenge_1_Build_APIS.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMakeService, MakeService>();
builder.Services.AddScoped<IModelService, ModelService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddDbContext<VegaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Vega API v1");
});

}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AngularClient");

app.MapControllers();

app.Run();