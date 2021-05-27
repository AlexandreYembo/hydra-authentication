using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentValidation.Results;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Identity.Application.Commands;
using Hydra.Identity.Application.Commands.TokenRefresh;
using Hydra.Identity.Application.Events.TokenRefresh;
using Hydra.Identity.Application.Models;
using Hydra.Identity.Application.Providers;
using Moq;
using Xunit;

namespace Hydra.Identity.Application.UnitTests.Commands
{
    public class TokenRefreshCommandHandlerTests
    {
        private TokenRefreshCommandHandler _sut;
        private Mock<IMediatorHandler> _mediatorHandlerMock;
        private Mock<IUserProvider> _userProviderMock;
        private Mock<IUserLoginProvider> _userLoginProviderMock;

        public TokenRefreshCommandHandlerTests()
        {
            _mediatorHandlerMock = new Mock<IMediatorHandler>();
            _userProviderMock = new Mock<IUserProvider>();
            _userLoginProviderMock = new Mock<IUserLoginProvider>();
        }

        [Fact(DisplayName="Refresh Token should fail when the refresh token is invalid")]
        [Trait("Refresh Token", "Refresh User Token")]
        [AutoData]
        public async Task RefreshUserToken_InvalidRefrehToken()
        {
            //Arrange
            var validationResult = new ValidationResult();
            validationResult.Errors.Add(CreateValidationFailure(null, "Invalid token refresh"));
            
            var refreshToken = Guid.NewGuid();

            _mediatorHandlerMock.Setup(s => s.PublishEvent(It.IsAny<TokenRefreshInvalidEvent>()))
                                .Verifiable("Notification was sent");

            var command = new TokenRefreshCommand(refreshToken);
            _sut = new TokenRefreshCommandHandler(_userLoginProviderMock.Object, _mediatorHandlerMock.Object, _userProviderMock.Object);
            
            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
           
            //Assert
            response.Payload.Should().BeNull();
            response.ValidationResult.Errors.Should().HaveCount(1);
            response.ValidationResult.Errors.Select(s => s.ErrorMessage).Should().Contain(validationResult.Errors.Select(s => s.ErrorMessage));
        }

        [Theory(DisplayName="Refresh Token should pass when the user is valid")]
        [Trait("Refresh Token", "Refresh User Token")]
        [AutoData]
        public async Task RefreshUserToken_ValidUser(RefreshToken expectedResult,  UserLoginResponse expectedUserLoginResponse)
        {
            //Arrange
            _userLoginProviderMock.Setup(s => s.GetRefreshToken(It.IsAny<Guid>())).ReturnsAsync(expectedResult);
            _userLoginProviderMock.Setup(s => s.TokenGenerator(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>())).ReturnsAsync(expectedUserLoginResponse);

            var refreshToken = Guid.NewGuid();

            var command = new TokenRefreshCommand(refreshToken);
            _sut = new TokenRefreshCommandHandler(_userLoginProviderMock.Object, _mediatorHandlerMock.Object, _userProviderMock.Object);
            
            //Act
            var response = await _sut.Handle(command, CancellationToken.None);
           
            //Assert
            response.Payload.Should().NotBeNull();
            response.ValidationResult.Should().BeNull();
           // response.Payload.RefreshToken.Should().BeEquivalentTo(expectedResult.Token);
        }

        private ValidationFailure CreateValidationFailure(string errorCode, string errorMessage) =>
            new ValidationFailure(string.Empty, errorMessage)
            {
                ErrorCode = errorCode
            };   
    }
}