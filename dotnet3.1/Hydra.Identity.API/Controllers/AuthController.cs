using System;
using System.Threading.Tasks;
using FluentValidation.Results;
using Hydra.Core.API.Controllers;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Identity.API.Models;
using Hydra.Identity.Application.Commands.UserLogin;
using Hydra.Identity.Application.Commands.UserRegister;
using Hydra.Identity.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hydra.Identity.API.Controllers
{
    [Route("api/identity")]
    public class AuthController : MainController
    {
        // private readonly IMessageBus _messageBus;
        private readonly IMediatorHandler _mediator;


        public AuthController(IMediatorHandler mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> Register(UserRegisterView userRegister)
        {
            if(!ModelState.IsValid) return CustomResponse(ModelState);

            var command = new CreateNewUserCommand(userRegister.IdentityNumber, userRegister.Name, userRegister.Email, userRegister.Email, userRegister.Password);

            var result = await _mediator.SendCommand<CreateNewUserCommand, ValidationResult>(command).ConfigureAwait(false);

            return CustomResponse(result);
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginView userLogin)
        {
            if(!ModelState.IsValid) return CustomResponse(ModelState);

            var command = new UserLoginCommand(userLogin.Email, userLogin.Password);
            var result = await _mediator.SendCommand<UserLoginCommand, UserLoginResponse>(command).ConfigureAwait(false);
            return CustomResponse(result);
        }

        // [HttpPost("refresh-token")]
        // public async Task<ActionResult> RefreshToken([FromBody] string refreshToken)
        // {
        //   if(string.IsNullOrEmpty(refreshToken))
        //   {
        //       AddErrors("Invalid refresh token");
        //       return CustomResponse();
        //   }

        //   var token = await _authenticationService.GetRefreshToken(Guid.Parse(refreshToken));

        //   if(token is null)
        //   {
        //       AddErrors("Refresh token is expired");
        //       return CustomResponse();
        //   }

        //   return CustomResponse(await _authenticationService.TokenGenerator(token.Username));
        // }
    }
}