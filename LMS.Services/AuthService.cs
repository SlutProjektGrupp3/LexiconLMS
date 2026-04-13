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
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtSettings _jwtSettings;
    private ApplicationUser? _user;

    public AuthService(
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<JwtSettings> jwtSettings
        )
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenDto> CreateTokenAsync(bool addTime)
    {
        SigningCredentials signing = GetSigningCredentials();
        IEnumerable<Claim> claims = await GetClaimsAsync();
        JwtSecurityToken token = GenerateToken(signing, claims);

        ArgumentNullException.ThrowIfNull(_user);
        _user.RefreshToken = GenerateRefreshToken();

        if (addTime)
            _user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(3);

        var res = await _userManager.UpdateAsync(_user);
        if (!res.Succeeded) throw new Exception(string.Join("/n", res.Errors));

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenDto(jwt, _user.RefreshToken!);
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
                                    issuer: _jwtSettings.Issuer,
                                    audience: _jwtSettings.Audience,
                                    claims: claims,
                                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.Expires)),
                                    signingCredentials: signing);

        return token;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        ArgumentNullException.ThrowIfNull(_user);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, $"{_user.FirstName} {_user.LastName}"),
            new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString()),
            
            //Add more claims here if needed
            //These claims disappear after first content paint?
            new Claim(ClaimTypes.GivenName, _user.FirstName),
            new Claim(ClaimTypes.Surname, _user.LastName),
            
        };

        var roles = await _userManager.GetRolesAsync(_user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistrationDto)
    {
        ArgumentNullException.ThrowIfNull(userRegistrationDto);

        var isRoleValid = !string.IsNullOrWhiteSpace(userRegistrationDto.Role);

        if (isRoleValid)
        {
            var roleExists = await _roleManager.RoleExistsAsync(userRegistrationDto.Role!);
            if (!roleExists)
                return IdentityResult.Failed(new IdentityError { Description = "Role does not exist" });
        }

        var user = _mapper.Map<ApplicationUser>(userRegistrationDto);
        var result = await _userManager.CreateAsync(user, userRegistrationDto.Password);

        if (!result.Succeeded) return result;

        if (isRoleValid)
            result = await _userManager.AddToRoleAsync(user, userRegistrationDto.Role!);

        return result;
    }

    public async Task<bool> ValidateUserAsync(UserAuthDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto);

        _user = await _userManager.FindByNameAsync(userDto.UserName);

        return _user != null && await _userManager.CheckPasswordAsync(_user, userDto.Password);
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto token)
    {
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token.AccessToken);
        ApplicationUser? user = await _userManager.FindByNameAsync(principal.Identity?.Name!);

        if (user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpireTime <= DateTime.Now)
            throw new TokenValidationException();

        this._user = user;

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
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
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
