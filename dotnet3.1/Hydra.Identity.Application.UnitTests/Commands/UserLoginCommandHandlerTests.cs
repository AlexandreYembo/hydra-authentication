using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentValidation.Results;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Core.Tests.Extensions;
using Hydra.Identity.Application.Commands;
using Hydra.Identity.Application.Commands.UserLogin;
using Hydra.Identity.Application.Models;
using Hydra.Identity.Application.Providers;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Hydra.Identity.Application.UnitTests.Commands
{
    public class UserLoginCommandHandlerTests
    {
        private UserLoginCommandHandler _sut;
        private Mock<IMediatorHandler> _mediatorHandlerMock;
        private Mock<IUserProvider> _userProviderMock;
        private Mock<IUserLoginProvider> _userLoginProviderMock;

        public UserLoginCommandHandlerTests()
        {
            _mediatorHandlerMock = new Mock<IMediatorHandler>();
            _userProviderMock = new Mock<IUserProvider>();
            _userLoginProviderMock = new Mock<IUserLoginProvider>();
        }

        [Fact(DisplayName="Login User should fail when the command is invalid")]
        [Trait("Login", "Login User")]
        public async Task LoginUser_InvalidCommand()
        {
            //Arrange
            var expectedResult = new ValidationResult();
            expectedResult.Errors.Add(CreateValidationFailure("-1005", "Invalid format of the email"));
            expectedResult.Errors.Add(CreateValidationFailure("-1003", "Email is required"));
            expectedResult.Errors.Add(CreateValidationFailure("-1004", "Password is required"));

            var command = new UserLoginCommand(string.Empty, string.Empty);
            _sut = new UserLoginCommandHandler(_userLoginProviderMock.Object, _mediatorHandlerMock.Object, _userProviderMock.Object);
            
            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
 
            //Assert
            response.Payload.Should().BeNull();
            response.ValidationResult.Errors.Should().HaveCount(3);
            response.ValidationResult.Errors.Select(s => s.ErrorCode).Should().Contain(expectedResult.Errors.Select(s => s.ErrorCode));
            response.ValidationResult.Errors.Select(s => s.ErrorMessage).Should().Contain(expectedResult.Errors.Select(s => s.ErrorMessage));
        }

        [Theory(DisplayName="Login User should fail when the user is invalid")]
        [Trait("Login", "Login User")]
        [AutoData]
        public async Task LoginUser_InvalidUserOrPassword(SignInResult expectedResult)
        {
            //Arrange
            expectedResult.SetProperty(s => s.Succeeded, false);
            var validationResult = new ValidationResult();
            validationResult.Errors.Add(CreateValidationFailure(null, "User or password invalid!"));
            _userLoginProviderMock.Setup(s => s.UserSignInAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResult);

            var command = new UserLoginCommand("myemail@example.com", "password123");
            _sut = new UserLoginCommandHandler(_userLoginProviderMock.Object, _mediatorHandlerMock.Object, _userProviderMock.Object);
            
            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
           
            //Assert
            response.Payload.Should().BeNull();
            response.ValidationResult.Errors.Should().HaveCount(1);
            response.ValidationResult.Errors.Select(s => s.ErrorMessage).Should().Contain(validationResult.Errors.Select(s => s.ErrorMessage));
        }

        [Theory(DisplayName="Login User should pass when the user is valid")]
        [Trait("Login", "Login User")]
        [AutoData]
        public async Task LoginUser_ValidUser(SignInResult expectedResult, UserLoginResponse expectedUserLoginResponse)
        {
            //Arrange
            expectedResult.SetProperty(s => s.Succeeded, true);
            _userLoginProviderMock.Setup(s => s.UserSignInAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedResult);
            _userLoginProviderMock.Setup(s => s.TokenGenerator(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>())).ReturnsAsync(expectedUserLoginResponse);

            var command = new UserLoginCommand("myemail@example.com", "password123");
            _sut = new UserLoginCommandHandler(_userLoginProviderMock.Object, _mediatorHandlerMock.Object, _userProviderMock.Object);
            
            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
           
            //Assert
            response.Payload.Should().NotBeNull();
            response.ValidationResult.Should().BeNull();
            response.Payload.UserToken.Should().BeEquivalentTo(expectedUserLoginResponse.UserToken);
        }

        private ValidationFailure CreateValidationFailure(string errorCode, string errorMessage) =>
            new ValidationFailure(string.Empty, errorMessage)
            {
                ErrorCode = errorCode
            };   
    }
}