using System;

namespace Hydra.Identity.Application.Models {
    public class RefreshToken {
        public RefreshToken () {
            this.Id = Guid.NewGuid();
            this.Token = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        /// <summary>
        /// this is not Json web token, this is a guid, unique
        /// </summary>
        /// <value></value>
        public Guid Token { get; set; }
        /// <summary>
        /// Email or userlogin of the user logged
        /// </summary>
        /// <value></value>
        public string Username { get; set; }

        /// <summary>
        /// When the token will expire Datetime + hours foward defined
        /// </summary>
        /// <value></value>
        public DateTime ExpirationDate { get; set; }
    }
}