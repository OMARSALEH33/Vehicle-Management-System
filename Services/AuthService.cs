using AutoMapper;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Persistence;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Challenge_1_Build_APIS.Services
{
    public class AuthService : IAuthService
    {
        private readonly VegaDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration; 

        public AuthService(VegaDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public async Task<ServiceResult<string>> ForgotPasswordAsync(ForgotPasswordRsource resource)
        {
            var user = await context.users.SingleOrDefaultAsync(u => u.Email == resource.Email);
            if (user == null)
                return ServiceResult<string>.NotFound();

            return ServiceResult<string>.Ok("Email verified.");
        }

        public async Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordResources resource)
        {
            var user = await context.users.SingleOrDefaultAsync(u => u.Email == resource.Email);
            if (user == null)
                return ServiceResult<bool>.NotFound();

            user.PasswordHash = resource.Password;
            await context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
        public async Task<ServiceResult<string>> LoginAsync(LoginResource loginResource)
        {
            var user = await context.users
                .SingleOrDefaultAsync(u => u.Username == loginResource.Username && u.PasswordHash == loginResource.Password);

            if (user == null)
                return ServiceResult<string>.NotFound();

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(configuration["JwtSettings:Secret"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(configuration["JwtSettings:DurationInMinutes"]!)),
                Issuer = configuration["JwtSettings:Issuer"],
                Audience = configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            return ServiceResult<string>.Ok(token);
        }
    }
}