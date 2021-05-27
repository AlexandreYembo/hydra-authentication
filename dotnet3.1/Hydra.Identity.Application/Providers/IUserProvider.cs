namespace Hydra.Identity.Application.Providers
{
    public interface IUserProvider
    {
        string Issuer { get; }
        double RefreshTokenExpiration { get; }
    }
}