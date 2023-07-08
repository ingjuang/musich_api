using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Musich_API.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Musich_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IConfiguration _configuration;
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet, Route("Get")]
        public async Task<ActionResult> Get()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validateToken(identity);
            if (!rToken.success) return Unauthorized(rToken);
            User user = rToken.result;
            var petitionResponse = new
            {
                success = false,
                message = "No tienes permisos para listar usuarios",
                result = ""
            };
            if (user.rol != "administrator")
            {
                return Ok(new
                {
                    success = false,
                    message = "No tienes permisos para listar usuarios",
                    result = ""
                });
            }
            User User = new User();

            return Ok(User.GetUsers());
        }

        [HttpPost, Route("Login")]
        public dynamic LogIng([FromBody] Object optData)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());
            string userName = data.User.ToString();
            string password = data.Password.ToString();
            User User = new User();
            User user = User.GetUsers().FirstOrDefault(x => x.UserName == userName && x.Password == password);
            if (user == null)
            {
                return new
                {
                    success = false,
                    message = "Credenciales incorrectas o usuario no existe",
                    result = ""
                };
            }

            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("user", user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: singIn
                    );

            return new
            {
                success= true,
                message= "exito",
                result= new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}
