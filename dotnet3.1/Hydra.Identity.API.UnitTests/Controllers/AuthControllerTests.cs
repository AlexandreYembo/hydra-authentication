using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;
using Hydra.Core.API.Tests;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Core.Mediator.Messages;
using Hydra.Identity.API.Controllers;
using Hydra.Identity.API.Models;
using Hydra.Identity.API.UnitTests.Mapping;
using Hydra.Identity.Application.Commands;
using Hydra.Identity.Application.Commands.UserRegister;
using Hydra.Tests.Fixtures;
using Hydra.Tests.Fixtures.Builders.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using Hydra.Identity.Application.Commands.UserLogin;
using Hydra.Identity.Application.Models;

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
       
        [Theory(DisplayName="Register User should Register")]
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

            _mediatorHandler.Setup(s => s.SendCommand<CreateNewUserCommand, ValidationResult>(It.IsAny<CreateNewUserCommand>())).ReturnsAsync(result);
            
            _sut = new AuthController(_mediatorHandler.Object);
            
            //Act
            var response = await _sut.Register(request);

            //Assert
            var statusCode = (response as ObjectResult).StatusCode;
            statusCode.Should().Be(200);
        }

        [Theory(DisplayName="Register User should fail when the request is invalid")]
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


        [Theory(DisplayName="Login user should return success")]
        [Trait("Login", "Login User")]
        [AutoMoqData]
        public async Task LoginUser_ShouldLogingTheUser(AutoMoqDataValidFixtures fixtures, CommandResult<UserLoginResponse> commandResultExpected)
        {
            //Arrange
            var request = new UserLoginView();
            request.Email = "test@test.com";
            request.Password = "Qwe!@£123";

            _mediatorHandler.Setup(s => s.SendCommand<UserLoginCommand, UserLoginResponse>(It.IsAny<UserLoginCommand>())).ReturnsAsync(commandResultExpected);
            
            _sut = new AuthController(_mediatorHandler.Object);
            
            //Act
            var response = await _sut.Login(request);
            var result = new CommandResultObjectMap<UserLoginResponse>(response);
            //Assert
            result.ErrorCode.Should().Be(200);
            result.CommandResult.Should().NotBeNull();
            result.CommandResult.Should().BeEquivalentTo(commandResultExpected.Payload);
        }

        [Theory(DisplayName="Login user should return fail")]
        [Trait("Login", "Login User")]
        [AutoMoqData]
        public async Task LoginUser_ShouldRetunLoginFail(AutoMoqDataValidFixtures fixtures, CommandResult<UserLoginResponse> result)
        {
            //Arrange
            var request = new UserLoginView();
            request.Email = "test@test.com";
            request.Password = "Qwe!@£123";

            result.Payload = new UserLoginResponse();
            result.ValidationResult.Errors.Add(new ValidationFailure(string.Empty, "Invalid request"));

            _mediatorHandler.Setup(s => s.SendCommand<UserLoginCommand, UserLoginResponse>(It.IsAny<UserLoginCommand>())).ReturnsAsync(result);
            
            _sut = new AuthController(_mediatorHandler.Object);
            
            //Act
            var response = await _sut.Login(request);

            //Assert
            var responseAssert = new BadRequestObjectResultMap(response);

            var expectedErrors = new List<string>{
                "Invalid request"
            };

            responseAssert.IsInvalidRequest(expectedErrors);
        }
    }
}