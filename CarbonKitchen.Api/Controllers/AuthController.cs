namespace CarbonKitchen.Api.Controllers
{
    using CarbonKitchen.Api.Data.Auth;
    using CarbonKitchen.Api.Services.Auth;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] Authentication authentication)
        {
            var token = _authService.Authenticate(authentication.Key);

            if (token == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(token);
        }

        //[HttpPost, Route("login")]
        //public IActionResult Login([FromBody] LoginModel user)
        //{
        //    if (user == null)
        //    {
        //        return BadRequest("Invalid client request");
        //    }

        //    if (user.UserName == "johndoe" && user.Password == "def@123")
        //    {
        //        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
        //        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        //        var tokeOptions = new JwtSecurityToken(
        //            issuer: "http://localhost:5000",
        //            audience: "http://localhost:5000",
        //            claims: new List<Claim>(),
        //            expires: DateTime.Now.AddMinutes(5),
        //            signingCredentials: signinCredentials
        //        );

        //        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        //        return Ok(new { Token = tokenString });
        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }
        //}
    }
}
