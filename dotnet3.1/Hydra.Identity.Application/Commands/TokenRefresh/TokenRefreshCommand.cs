using System;
using FluentValidation.Results;
using Hydra.Core.Mediator.Messages;
using Hydra.Identity.Application.Models;

namespace Hydra.Identity.Application.Commands.TokenRefresh {
    public class TokenRefreshCommand : Command<UserLoginResponse> {
        public TokenRefreshCommand (Guid tokenId) {
            this.TokenId = tokenId;

        }
        public Guid TokenId { get; set; }
    }
}