using AuthenticationApi.Authentication;
using AuthenticationApi.Data;
using AuthenticationApi.Models;
using AuthenticationApi.Models.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly DataDbContext _dataDbContext;

        public AuthenticateController(IConfiguration configuration, UserManager<ApplicationUser> userManager, DataDbContext dataDbContext)
        {
            _configuration = configuration;
            this.userManager = userManager;
            _dataDbContext = dataDbContext;
        }



        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Registration model)
        {

            //checking if the user allready exist in the database.
            
            var userExists = await _dataDbContext.Register.Where(x => x.Username.Contains(model.Username)).FirstOrDefaultAsync();
           
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { status = "Error", message = "User already exists!" });

            model.Id = Guid.NewGuid();
            var result = await _dataDbContext.Register.AddAsync(model);
            //saves the created user to the database.
            await _dataDbContext.SaveChangesAsync();

            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { status = "Error", message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { status = "Success", message = "User created successfully!" });
            
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel logmodel)
        {
            var user =  _dataDbContext.Register.Where(x => x.Username.Equals(logmodel.Username) && x.Password.Equals(logmodel.Password)).FirstOrDefault();
            if (user != null)
            {
            //    var userRoles =  userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                     new Claim("Email",user.Email!=null?user.Email:""),
                    new Claim("Username",user.Username!=null?user.Username:""),
                    new Claim(ClaimTypes.Role,"Admin"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                //foreach (var userRole in userRoles)
                //{
                //    authClaims.Add(new Claim(ClaimTypes.Role, userRole));

                //}

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(5),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                //var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                //return Ok(new AuthenticatedResponse { Token = tokenString });

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}
