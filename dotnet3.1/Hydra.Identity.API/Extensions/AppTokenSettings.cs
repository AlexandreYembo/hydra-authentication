namespace Hydra.Identity.API.Extensions
{
    public class AppTokenSettings
    {
        public int RefreshTokenExpiration { get; set; }
        public string Issuer { get; set; }
    }
}