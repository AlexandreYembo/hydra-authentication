using System.ComponentModel.DataAnnotations;

namespace Hydra.Identity.API.Models
{
    public class UserLoginView
    {
        [Required(ErrorMessage="The field {0} is required")]
        [EmailAddress(ErrorMessage="The field {0} is Invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage="The field {0} is required")]
        [StringLength(100, ErrorMessage="The field {0} may have between {2} and {1} characters", MinimumLength = 6)]
        public string Password { get; set; }
    }
}