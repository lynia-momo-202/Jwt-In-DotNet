using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonAPI.Data;
using MonAPI.Services;

namespace MonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(JwtService jwtService, UserManager<IdentityUser> userManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            if (!_jwtService.IsValidAudience(request.Audience))
            {
                return BadRequest("Invalid audience.");
            }

            var userClaims = (await _userManager.GetClaimsAsync(user)).ToList();
            var token = _jwtService.GenerateToken(request.UserName, request.Audience, userClaims);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _jwtService.SaveRefreshToken(request.UserName, refreshToken);

            return Ok(new
            {
                Token = token,
                RefreshToken = refreshToken,
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] Models.RefreshRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserName);
            if (user == null || !await _jwtService.IsValidRefreshToken(request.RefreshToken))
                return Unauthorized();

            if (!_jwtService.IsValidAudience(request.Audience))
                return BadRequest("Audience non valide .");

            var userClaims = await _userManager.GetClaimsAsync(user);
            var token = _jwtService.GenerateToken(request.UserName, request.Audience, userClaims.ToList());
            var refreshToken = _jwtService.GenerateRefreshToken();

            //révoquer l'ancien token et stocker le nouveau
            await _jwtService.RevokeRefreshToken(refreshToken);
            await _jwtService.SaveRefreshToken(request.UserName, refreshToken);

            return Ok(
                new
                {
                    Token = token,
                    RefreshToken = refreshToken
                }
            );

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var user = new ApplicationUser { UserName = request.UserName };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            //Ajouter des claims à l'utilisateur 
            result = await _userManager.AddClaimAsync(user, new Claim(JwtRegisteredClaimNames.Name, request.DisplayName));
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }
        
        public record RegisterRequest (string UserName, string DisplayName , string Password);
    }
}
