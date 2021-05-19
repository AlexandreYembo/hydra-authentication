using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Core.Mediator.Integration;
using Hydra.Core.MessageBus;
using Hydra.Identity.Application.Commands;
using Hydra.Identity.Application.Commands.UserRegister;
using Hydra.Identity.Application.Events.UserRegister;
using Hydra.Identity.Application.Providers;
using Hydra.Tests.Fixtures;
using Hydra.Tests.Fixtures.Builders.Models;
using Hydra.User.Integration.Messages;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Hydra.Identity.Application.UnitTests
{
    public class UserRegisterCommandHandlerTests
    {
        private UserRegisterCommandHandler _sut;
        private Mock<IMessageBus> _messageBusMock;
        private Mock<IMediatorHandler> _mediatorHandlerMock;
        private Mock<IUserRegisterProvider> _userRegisterProviderMock;

        public UserRegisterCommandHandlerTests()
        {
            _messageBusMock = new Mock<IMessageBus>();
            _mediatorHandlerMock = new Mock<IMediatorHandler>();
            _userRegisterProviderMock = new Mock<IUserRegisterProvider>();
        }

        [Theory(DisplayName="Register User should fail when the command is invalid")]
        [Trait("Register", "Register User")]
        [AutoMoqData]
        public async Task RegisterUser_AddNewUser_InvalidCommand(AutoMoqDataValidFixtures fixtures)
        {
            //Arrange
            var expectedResult = new ValidationResult();
            expectedResult.Errors.Add(CreateValidationFailure("-1000", "Identity Number is required"));
            expectedResult.Errors.Add(CreateValidationFailure("-1001", "Name is required"));
            expectedResult.Errors.Add(CreateValidationFailure("-1002", "Username is required"));
            expectedResult.Errors.Add(CreateValidationFailure("-1005", "Invalid format of the email"));
            expectedResult.Errors.Add(CreateValidationFailure("-1003", "Email is required"));
            expectedResult.Errors.Add(CreateValidationFailure("-1004", "Password is required"));

            var command = new CreateNewUserCommand(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            _sut = new UserRegisterCommandHandler(_userRegisterProviderMock.Object, _mediatorHandlerMock.Object, _messageBusMock.Object);
            
            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
 
            //Assert
            response.ValidationResult.Errors.Should().HaveCount(7);
            response.ValidationResult.Errors.Select(s => s.ErrorCode).Should().Contain(expectedResult.Errors.Select(s => s.ErrorCode));
            response.ValidationResult.Errors.Select(s => s.ErrorMessage).Should().Contain(expectedResult.Errors.Select(s => s.ErrorMessage));
           
        }


        [Theory(DisplayName="Register User should fail when the user already exists")]
        [Trait("Register", "Register User")]
        [AutoMoqData]
        public async Task RegisterUser_AddNewUser_UserAlreadyExists(AutoMoqDataValidFixtures fixtures)
        {
            //Arrange
            var identityError = new IdentityError
            {
                Code = "1",
                Description = "Username already exists"
            };

            _userRegisterProviderMock.Setup(s => s.CreateUserAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(identityError)).Verifiable();

            var command = new CreateNewUserCommand("123", "Alexandre", "alexandre@test.com", "alexandre@test.com", "A3£fksk@sf324");
            _sut = new UserRegisterCommandHandler(_userRegisterProviderMock.Object, _mediatorHandlerMock.Object, _messageBusMock.Object);
            
            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
 
            //Assert
            response.ValidationResult.Errors.Should().HaveCount(1);
            response.ValidationResult.Errors.Select(s => s.ErrorMessage).Should().Contain(identityError.Description);
        }

        [Theory(DisplayName="Register User should fail when try to create a new customer")]
        [Trait("Register", "Register User")]
        [AutoMoqData]
        public async Task RegisterUser_AddNewUser_CreateCustomer_ShouldFail(AutoMoqDataValidFixtures fixtures, IdentityUser identityUser)
        {
            //Arrange
            var expectedResult = new ValidationResult();
            expectedResult.Errors.Add(CreateValidationFailure("0", "Error to create a new customer, same identity Number defined"));

            identityUser.Id = Guid.NewGuid().ToString();

            var responseMessage = new ResponseMessage(expectedResult);

            _messageBusMock.Setup(s =>s.RequestAsync<UserSaveIntegrationEvent, ResponseMessage>(It.IsAny<UserSaveIntegrationEvent>())).ReturnsAsync(responseMessage);

            _userRegisterProviderMock.Setup(s => s.CreateUserAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            _userRegisterProviderMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);

            _mediatorHandlerMock.Setup(s => s.PublishEvent(It.IsAny<UserCanceledEvent>()))
                                .Verifiable("Notification was not sent");

            var command = new CreateNewUserCommand("123", "Alexandre", "alexandre@test.com", "alexandre@test.com", "A3£fksk@sf324");
            _sut = new UserRegisterCommandHandler(_userRegisterProviderMock.Object, _mediatorHandlerMock.Object, _messageBusMock.Object);
            
            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
 
            //Assert
            _mediatorHandlerMock.Verify(m => m.PublishEvent(It.IsAny<UserCanceledEvent>()), Times.Once());
            response.ValidationResult.Errors.Should().HaveCount(1);
            response.ValidationResult.Errors.Select(s => s.ErrorMessage).Should().Contain(expectedResult.Errors.FirstOrDefault().ErrorMessage);
        }

        [Theory(DisplayName="Register User should create a new customer")]
        [Trait("Register", "Register User")]
        [AutoMoqData]
        public async Task RegisterUser_AddNewUser_CreateCustomer_ShouldCreateCustomer(AutoMoqDataValidFixtures fixtures, IdentityUser identityUser)
        {
            //Arrange
            identityUser.Id = Guid.NewGuid().ToString();
            var expectedResult = new ValidationResult();


            var responseMessage = new ResponseMessage(expectedResult);

            _messageBusMock.Setup(s =>s.RequestAsync<UserSaveIntegrationEvent, ResponseMessage>(It.IsAny<UserSaveIntegrationEvent>())).ReturnsAsync(responseMessage);

            _userRegisterProviderMock.Setup(s => s.CreateUserAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            _userRegisterProviderMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);

           

            var command = new CreateNewUserCommand("123", "Alexandre", "alexandre@test.com", "alexandre@test.com", "A3£fksk@sf324");
            _sut = new UserRegisterCommandHandler(_userRegisterProviderMock.Object, _mediatorHandlerMock.Object, _messageBusMock.Object);
            
            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
 
            //Assert
            response.ValidationResult.IsValid.Should().Be(true);
        }
        
        [Theory(DisplayName="Register User should fail when try to create a new customer when falls in an exception")]
        [Trait("Register", "Register User")]
        [AutoMoqData]
        public async Task RegisterUser_AddNewUser_CreateCustomer_ShouldFailWhenFallsInException(AutoMoqDataValidFixtures fixtures, IdentityUser identityUser)
        {
            //Arrange
            var expectedResult = new ValidationResult();
            expectedResult.Errors.Add(CreateValidationFailure("0", "Timeout exception"));

            identityUser.Id = Guid.NewGuid().ToString();

            var responseMessage = new ResponseMessage(expectedResult);

            _messageBusMock.Setup(s =>s.RequestAsync<UserSaveIntegrationEvent, ResponseMessage>(It.IsAny<UserSaveIntegrationEvent>())).ThrowsAsync(new Exception("Timeout exception"));//.ReturnsAsync(responseMessage);

            _userRegisterProviderMock.Setup(s => s.CreateUserAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            _userRegisterProviderMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(identityUser);

            _mediatorHandlerMock.Setup(s => s.PublishEvent(It.IsAny<UserCanceledEvent>()))
                                .Verifiable("Notification was not sent");

            var command = new CreateNewUserCommand("123", "Alexandre", "alexandre@test.com", "alexandre@test.com", "A3£fksk@sf324");
            _sut = new UserRegisterCommandHandler(_userRegisterProviderMock.Object, _mediatorHandlerMock.Object, _messageBusMock.Object);

            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
 
            //Assert
            _mediatorHandlerMock.Verify(m => m.PublishEvent(It.IsAny<UserCanceledEvent>()), Times.Once());

            response.ValidationResult.Errors.Should().HaveCount(1);
            response.ValidationResult.Errors.Select(s => s.ErrorMessage).Should().Contain(expectedResult.Errors.FirstOrDefault().ErrorMessage);
        }

        private ValidationFailure CreateValidationFailure(string errorCode, string errorMessage) =>
            new ValidationFailure(string.Empty, errorMessage)
            {
                ErrorCode = errorCode
            };   
    }
}