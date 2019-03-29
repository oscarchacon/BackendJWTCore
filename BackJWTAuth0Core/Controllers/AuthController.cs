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
            else
            {
                var claims = new Claim[]
                {
                    new Claim("UserName", user.UserName),
                    new Claim(ClaimTypes.Role, user.RoleName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiAuth:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    issuer: _configuration["ApiAuth:Issuer"],
                    audience: _configuration["ApiAuth:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Int32.Parse(_configuration["ApiAuth:expiresAt"])),
                    notBefore: DateTime.Now,
                    signingCredentials: creds
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }
            /*else
            {
                return Unauthorized();
            }*/
        }
    }
}
