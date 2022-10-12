using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiCourse.DTOs;

namespace WebApiCourse.Controllers.V1
{
    [ApiController]
    [Route("api/v1/counts")]
    public class CountUsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public CountUsersController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("Register", Name = "registerUserv1")] // api/counts/Register
        public async Task<ActionResult<ResponseAuthentication>> Register(CredentialsUser credentialsUser)
        {
            var user = new IdentityUser { UserName = credentialsUser.Email, Email = credentialsUser.Email };
            var result = await userManager.CreateAsync(user, credentialsUser.Password);

            if (result.Succeeded)
            {
                return await BuildToken(credentialsUser);
            }

            return BadRequest(result.Errors);

        }

        [HttpPost("login", Name = "loginUserv1")] // api/counts/login
        public async Task<ActionResult<ResponseAuthentication>> Login(CredentialsUser credentialsUser)
        {
            var result = await signInManager.PasswordSignInAsync(credentialsUser.Email, credentialsUser.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(credentialsUser);
            }

            return BadRequest("Incorrect login");
        }

        [HttpPost("RenewToken", Name = "renewTokenv1")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ResponseAuthentication>> RenewToken()
        {
            var emailClam = HttpContext.User.Claims.Where(clam => clam.Type == "email").FirstOrDefault();
            var email = emailClam.Value;

            var credentialsUser = new CredentialsUser()
            {
                Email = email
            };

            return await BuildToken(credentialsUser);
        }


        [NonAction]
        public async Task<ActionResult<ResponseAuthentication>> BuildToken(CredentialsUser credentialsUser)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credentialsUser.Email),
                new Claim("words", "rddrrddrrddr")
            };

            var user = await userManager.FindByEmailAsync(credentialsUser.Email);
            var claimDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["KeyJWT"]));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(2);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: cred);

            return new ResponseAuthentication()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }

        [HttpPost("addAdmin", Name = "addAdminv1")]
        public async Task<ActionResult> AddAdmin(EditAdmin editAdmin)
        {
            var user = await userManager.FindByEmailAsync(editAdmin.Email);
            await userManager.AddClaimAsync(user, new Claim("IsAdmin", "true"));
            return NoContent();
        }

        [HttpPost("removeAdmin", Name = "removeAdminv1")]
        public async Task<ActionResult> RemoveAdmin(EditAdmin editAdmin)
        {
            var user = await userManager.FindByEmailAsync(editAdmin.Email);
            await userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "true"));
            return NoContent();
        }
    }
}
