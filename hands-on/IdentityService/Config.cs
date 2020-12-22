using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace IdentityService
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            new ApiScope("api.1"),
            new ApiScope("api.2"),
            new ApiScope("api.3")
        };
        
        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
        {
            // @API.Resource : ApiResource.Name 이 Audience 이름 ?!
            new ApiResource("Audience.ApiOne", "ApiOne ApiResource's DisplayName Here")
            {
                Scopes = new HashSet<string>{ "api.1" } // @API.Scope
            },
        };

        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "any_client",
                ClientSecrets =
                {
                    new Secret("very_secret_key_of_api_client".ToSha256()),
                    //new Secret("miflow_very_secret_key_2".ToSha256()), 
                },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // {
                //     GrantType.ClientCredentials,
                // },
                AllowedScopes =
                {
                    "api.1",  // see @API.Scope
                    "api.2"
                },
                RedirectUris = { "https://localhost:44345/home/signin" },
            },
        };
    }
    
}