using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;
using Hydra.Core.API.Tests;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Core.Mediator.Messages;
using Hydra.Identity.API.Controllers;
using Hydra.Identity.API.Models;
using Hydra.Identity.API.UnitTests.Mappnig;
using Hydra.Identity.Application.Commands;
using Hydra.Identity.Application.Commands.UserRegister;
using Hydra.Tests.Fixtures;
using Hydra.Tests.Fixtures.Builders.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Hydra.Identity.API.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private AuthController _sut;
        private readonly Mock<IMediatorHandler> _mediatorHandler;
        private readonly Mock<UserRegisterCommandHandler> _commandHandler;

        public AuthControllerTests()
        {
           _mediatorHandler = new Mock<IMediatorHandler>();
           _commandHandler = new Mock<UserRegisterCommandHandler>();
        }
       
        [Theory(DisplayName="Register User should fail when the model is Email is null")]
        [Trait("Register", "Register User")]
        [AutoMoqData]
        public async Task RegisterUser_AddNewUser_ValidRequest(AutoMoqDataValidFixtures fixtures, CommandResult<ValidationResult> result)
        {
            //Arrange
            var request = new UserRegisterView();
            request.Email = "test@test.com";
            request.Password = "Qwe!@£123";
            request.PasswordConfirmation = "Qwe!@£123";
            request.Name = "TestUser";
            request.IdentityNumber = "ABC123";

            var command = new CreateNewUserCommand(request.IdentityNumber, request.Name, request.Email, request.Email, request.Password);
            _mediatorHandler.Setup(s => s.SendCommand<CreateNewUserCommand, ValidationResult>(command)).ReturnsAsync(result);
            
            _sut = new AuthController(_mediatorHandler.Object);
            
            //Act
            var response = await _sut.Register(request);

            //Assert
            var statusCode = (response as ObjectResult).StatusCode;
            Assert.Equal(200, statusCode);
        }

        [Theory(DisplayName="Register User should fail when the model is Email is null")]
        [Trait("Register", "Register User")]
        [AutoMoqData]
        public async Task RegisterUser_AddNewUser_InvalidRequest(AutoMoqDataValidFixtures fixtures, CommandResult<ValidationResult> result)
        {
            //Arrange
            var request = new UserRegisterView();
            request.Email = "test@test.com";
            request.Password = "Qwe!@£123";
            request.PasswordConfirmation = "Qwe!@£123";
            request.Name = "TestUser";
            request.IdentityNumber = "ABC123";

            result.Payload = new ValidationResult();
            result.ValidationResult.Errors.Add(new ValidationFailure(string.Empty, "Invalid request"));

            var command = new CreateNewUserCommand(request.IdentityNumber, request.Name, request.Email, request.Email, request.Password);
            _mediatorHandler.Setup(s => s.SendCommand<CreateNewUserCommand, ValidationResult>(It.IsAny<CreateNewUserCommand>())).ReturnsAsync(result);
            
            _sut = new AuthController(_mediatorHandler.Object);
            
            //Act
            var response = await _sut.Register(request);

            //Assert
            var responseAssert = new BadRequestObjectResultMap(response);

            var expectedErrors = new List<string>{
                "Invalid request"
            };

            responseAssert.IsInvalidRequest(expectedErrors);
        }

        [Theory(DisplayName="Register User should fail when the model is Email is null")]
        [Trait("Register", "Register User")]
        [AutoMoqData]
        public async Task RegisterUser_AddNewUser_InvaliEmail(AutoMoqDataValidFixtures fixtures, CommandResult<ValidationResult> result)
        {
            //Arrange
            var request = new UserRegisterView();
            request.Email = null;
            request.Password = "Qwe!@£123";
            request.PasswordConfirmation = "Qwe!@£123";
            request.Name = "TestUser";
            request.IdentityNumber = "ABC123";

            result.Payload = new ValidationResult();
            result.ValidationResult.Errors.Add(new ValidationFailure(string.Empty, "Invalid request"));

            var command = new CreateNewUserCommand(request.IdentityNumber, request.Name, request.Email, request.Email, request.Password);
            _mediatorHandler.Setup(s => s.SendCommand<CreateNewUserCommand, ValidationResult>(It.IsAny<CreateNewUserCommand>())).ReturnsAsync(result);
            
            _sut = new AuthController(_mediatorHandler.Object);
            
            //Act
            var response = await _sut.Register(request);

            //Assert
            var responseAssert = new BadRequestObjectResultMap(response);

            var expectedErrors = new List<string>{
                "Invalid request"
            };

            responseAssert.IsInvalidRequest(expectedErrors);
        }
    }
}