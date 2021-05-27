using System.Collections.Generic;
using System.Linq;
using Hydra.Core.API.Tests;
using Microsoft.AspNetCore.Mvc;

namespace Hydra.Identity.API.UnitTests.Mapping
{
     public class BadRequestObjectResultMap : IRequestResultAssertion
     {
        public int ErrorCode { get; set; }
        public List<string> ErrorMessages { get; set ; }

        public BadRequestObjectResultMap(IActionResult actionResult)
        {
            var badRequestResult = actionResult as BadRequestObjectResult;
            var validationProblemDetails = badRequestResult.Value as ValidationProblemDetails;
            ErrorMessages =  validationProblemDetails.Errors.SelectMany(a => a.Value).ToList();
            ErrorCode = (int)badRequestResult.StatusCode;
        }
    }
}