using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackJWTAuth0Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace BackJWTAuth0Core.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        IList<UserModel> users = new List<UserModel>
        {
            new UserModel { UserName = "user", Password = "123456", RoleName = "Manager"},
            new UserModel { UserName = "test", Password = "123456", RoleName = "Operator"}
        };

        public AuthController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // GET api/values
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody]UserModel _user)
        {
            var user = users.SingleOrDefault(x => x.UserName == _user.UserName && x.Password == _user.Password);

            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            else if (user.UserName == "user" && user.Password == "123456")
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, "Manager")
                };

                var tokeOptions = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
