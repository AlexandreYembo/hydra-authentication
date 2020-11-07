using System;
using System.Threading.Tasks;
using Hydra.Core.Integration.Messages;
using Hydra.Core.MessageBus;
using Hydra.Identity.API.Models;
using Hydra.Identity.API.Services;
using Hydra.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hydra.Identity.API.Controllers
{
    [Route("api/identity")]
    public class AuthController : MainController
    {
        private readonly AuthenticationService _authenticationService;
        private readonly IMessageBus _messageBus;


        public AuthController(   AuthenticationService authenticationService,
            IMessageBus messageBus)
        {
            _authenticationService = authenticationService;
            _messageBus = messageBus;
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

            var userRegistered = await _authenticationService.UserManager.CreateAsync(user, userRegister.Password);

            if(userRegistered.Succeeded)
            {
               var customerResult = await CreateCustomer(userRegister);

               if(!customerResult.ValidResult.IsValid)
               {
                   await _authenticationService.UserManager.DeleteAsync(user);
                   return CustomResponse(customerResult.ValidResult);
               }

               return CustomResponse(await _authenticationService.TokenGenerator(userRegister.Email));
            }

            foreach (var error in userRegistered.Errors)
            {
                AddErrors(error.Description);
            }

            return CustomResponse();
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginView userLogin)
        {
            if(!ModelState.IsValid) return BadRequest();

            var userLogged = await _authenticationService.SignInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);
            
            if(userLogged.Succeeded) return CustomResponse(await _authenticationService.TokenGenerator(userLogin.Email));

            if(userLogged.IsLockedOut)
            {
                AddErrors("User tempoary locked for many tries");
                return CustomResponse();
            }

            AddErrors("Invalid User or password");
            return CustomResponse();
        }

        private async Task<ResponseMessage> CreateCustomer(UserRegisterView userRegister)
        {
            var user = await _authenticationService.UserManager.FindByEmailAsync(userRegister.Email);
            var userSaved =  new UserSaveIntegrationEvent(Guid.Parse(user.Id), userRegister.Name, user.Email, userRegister.IdentityNumber);
            try
            {
                return await _messageBus.RequestAsync<UserSaveIntegrationEvent, ResponseMessage>(userSaved);
            }
            catch
            {
                await _authenticationService.UserManager.DeleteAsync(user);
                throw;
            }
        }
    }
}