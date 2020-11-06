using System.Threading.Tasks;
using Hydra.Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hydra.Identity.API.Controllers
{
    [Route("api/identity")]
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager; //manage login
        private readonly UserManager<IdentityUser> _userManager; // manage how I will manage the user

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> Register(UserRegisterView userRegister)
        {
            if(!ModelState.IsValid) return BadRequest();

            var user = new IdentityUser
            {
                UserName = userRegister.Email,
                Email = userRegister.Email,
                EmailConfirmed = false // Will send email to confirm the account
            };

            var userRegistered = await _userManager.CreateAsync(user, userRegister.Password);

            if(userRegistered.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginView userLogin)
        {
            if(!ModelState.IsValid) return BadRequest();

            var userLogged = await _signInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);
            
            if(userLogged.Succeeded) return Ok();

            return BadRequest();
        }
    }
}