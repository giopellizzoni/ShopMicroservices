using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Shopping.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Address(),
            new IdentityResources.Email(),
            new IdentityResource("roles", "Your role(s)", new List<string> {"role"})
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("shoppingAPI", "Shopping.API")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "shopping-ms-api",
                ClientName = "Shopping MVC WebAPP",
                ClientSecrets = [new Secret("840C7CDA-1E6F-42E7-A29C-3D12FE965A6F".Sha256())],
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                RequirePkce = false,
                AllowRememberConsent = false,
                RedirectUris = { "https://localhost:5010/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5010/signout-callback-oidc" },
                AllowedScopes = [
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.Email,
                    "shoppingAPI",
                    "roles"
                ]
            }
        };
}
