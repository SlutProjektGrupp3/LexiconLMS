using AutoMapper;
using Domain.Models.Configurations;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.AuthDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LMS.Services;
public class AuthService : IAuthService
{
    private readonly IMapper mapper;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly JwtSettings jwtSettings;
    private ApplicationUser? user;

    public AuthService(
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<JwtSettings> jwtSettings
        )
    {
        this.mapper = mapper;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenDto> CreateTokenAsync(bool addTime)
    {
        SigningCredentials signing = GetSigningCredentials();
        IEnumerable<Claim> claims = await GetClaimsAsync();
        JwtSecurityToken token = GenerateToken(signing, claims);

        ArgumentNullException.ThrowIfNull(user);
        user.RefreshToken = GenerateRefreshToken();

        if (addTime)
            user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(3);

        var res = await userManager.UpdateAsync(user);
        if (!res.Succeeded) throw new Exception(string.Join("/n", res.Errors));

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenDto(jwt, user.RefreshToken!);
    }

    private string? GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private JwtSecurityToken GenerateToken(SigningCredentials signing, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
                                    issuer: jwtSettings.Issuer,
                                    audience: jwtSettings.Audience,
                                    claims: claims,
                                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.Expires)),
                                    signingCredentials: signing);

        return token;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        ArgumentNullException.ThrowIfNull(user);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            
            //Add more claims here if needed
            //These claims disappear after first content paint?
            //new Claim("FirstName", user.FirstName),
            //new Claim("LastName", user.LastName)
            //new Claim("FavoriteColor", "Blue") 
        };

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistrationDto)
    {
        ArgumentNullException.ThrowIfNull(userRegistrationDto);

        var isRoleValid = !string.IsNullOrWhiteSpace(userRegistrationDto.Role);

        if (isRoleValid)
        {
            var roleExists = await roleManager.RoleExistsAsync(userRegistrationDto.Role!);
            if (!roleExists)
                return IdentityResult.Failed(new IdentityError { Description = "Role does not exist" });
        }

        var user = mapper.Map<ApplicationUser>(userRegistrationDto);
        var result = await userManager.CreateAsync(user, userRegistrationDto.Password);

        if (!result.Succeeded) return result;

        if (isRoleValid)
            result = await userManager.AddToRoleAsync(user, userRegistrationDto.Role!);

        return result;
    }

    public async Task<bool> ValidateUserAsync(UserAuthDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto);

        user = await userManager.FindByNameAsync(userDto.UserName);

        return user != null && await userManager.CheckPasswordAsync(user, userDto.Password);
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto token)
    {
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token.AccessToken);
        ApplicationUser? user = await userManager.FindByNameAsync(principal.Identity?.Name!);

        if (user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpireTime <= DateTime.Now)
            throw new TokenValidationException();

        this.user = user;

        return await CreateTokenAsync(addTime: false);

    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
