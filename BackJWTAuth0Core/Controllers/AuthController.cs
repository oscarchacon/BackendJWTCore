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
using BackJWTAuth0Core.Security;

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
        /// <summary>
        /// Login the specified _user.
        /// </summary>
        /// <returns>The login.</returns>
        /// <param name="_user">User.</param>
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
                var tokenString = TokenGenerator.GenerateTokenJwt(user.UserName, user.RoleName, _configuration);
                return Ok(new { Token = tokenString });
            }
            /*else
            {
                return Unauthorized();
            }*/
        }
    }
}
