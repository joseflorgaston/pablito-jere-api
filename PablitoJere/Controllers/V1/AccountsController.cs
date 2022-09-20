using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PablitoJere.DTOs;
using PablitoJere.Services;
using PablitoJere.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PablitoJere.Controllers.V1
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;
        public AccountsController(
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration, 
            SignInManager<IdentityUser> signInManager, 
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService
            )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            this.dataProtector = dataProtectionProvider.CreateProtector("valor_unico");
        }

        [HttpGet("encrypt")]
        public ActionResult Encrypt()
        {
            var textoPlano = "Jose";
            var cifferedText = dataProtector.Protect(textoPlano);
            var textoDesencryptado = dataProtector.Unprotect(cifferedText);

            return Ok(new
            {
                textoDesencryptado,
                textoPlano,
                cifferedText
            });
        }

        [HttpGet("{text}")]
        public ActionResult Hash(string text)
        {
            var hash1 = hashService.Hash(text);
            var hash2 = hashService.Hash(text);
            return Ok(new
            {
                text,
                hash1,
                hash2
            });
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<IdentityUser>>> Users()
        {
            var users = await userManager.GetUsersInRoleAsync("Admin");
            return Ok(new { users = users, count = users.Count});
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponse>> Register(UsersCredential usersCredential) 
        {
            var usuario = new IdentityUser { UserName = usersCredential.UserName, Email = usersCredential.Email, };
            var result = await userManager.CreateAsync(usuario, usersCredential.Password);
            if (result.Succeeded)
            {
                var currentUser = await userManager.FindByNameAsync(usuario.UserName);

                var roleresult = await userManager.AddToRoleAsync(currentUser, "Admin");

                return await TokenConstruction(usersCredential);
            } else
            {
                return BadRequest(result.Errors);
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(LoginDTO usersCredential)
        {
            var user = new IdentityUser();
            var userCredentials = new UsersCredential()
            {
                UserName = usersCredential.UserName,
                Password = usersCredential.Password,
            };

            if (SharedFunctions.IsValidEmail(userCredentials.UserName))
            {
                user = await userManager.FindByEmailAsync(userCredentials.UserName.ToUpper());
            } else
            {
                user = await userManager.FindByNameAsync(usersCredential.UserName);
            }

            if(user != null)
            {
                userCredentials.Email = user.Email;
                userCredentials.UserName = user.UserName;
            }
            
            var result = await signInManager.PasswordSignInAsync(userCredentials.UserName, usersCredential.Password, isPersistent: false, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                return await TokenConstruction(userCredentials);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("RefreshToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticationResponse>> Refresh()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var userClaim = HttpContext.User.Claims.Where(claim => claim.Type == "userName").FirstOrDefault();
            var email = emailClaim.Value;
            var userName = userClaim.Value;
            var userCredentials = new UsersCredential()
            {
                Email = email,
                UserName = userClaim.Value
            };

            return await TokenConstruction(userCredentials);
        }
        private async Task<AuthenticationResponse> TokenConstruction (UsersCredential usersCredential)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", usersCredential.Email),
                new Claim("userName", usersCredential.UserName),
            };

            var user = await userManager.FindByEmailAsync(usersCredential.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDB);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var expiration = DateTime.UtcNow.AddMinutes(30);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials:creds);
            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }

        [HttpPost("SetAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> SetAdmin(EditAdminDTO editAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("isAdmin", "1"));

            return NoContent();
        }

        [HttpPost("RemoveAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> RemoveAdmin(EditAdminDTO editAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("isAdmin", "1"));

            return NoContent();
        }

        [HttpDelete("RemoveUser/{userName}")]
        public async Task<ActionResult> RemoveUser(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if(user == null)
            {
                return BadRequest("Usuario no encontrado");
            }
            await userManager.DeleteAsync(user);
            return Ok("Usuario eliminado exitosamente");
        }
    }
}
