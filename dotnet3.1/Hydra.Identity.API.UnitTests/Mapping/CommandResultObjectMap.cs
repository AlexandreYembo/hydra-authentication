using System.Collections.Generic;
using Hydra.Core.API.Tests;
using Hydra.Core.Mediator.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Hydra.Identity.API.UnitTests.Mapping
{
    public class CommandResultObjectMap<T> : IRequestResultAssertion
    {
        public int ErrorCode { get; set; } // change library to be status code
        public List<string> ErrorMessages { get; set ; }

        public T CommandResult {get; set;}

         public CommandResultObjectMap(IActionResult actionResult)
        {
             var result = (actionResult as ObjectResult);
            ErrorCode = (int)result.StatusCode;
            CommandResult= (T)result.Value;
        }
    }
}