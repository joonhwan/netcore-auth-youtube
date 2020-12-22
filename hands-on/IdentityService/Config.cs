using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            // 'openid' scope : openid connection 시 로그인 하려면, 
            new IdentityResources.OpenId(),
            
            // 'profile' scope : 사용자 Claim(정보, 예: name, family_name, middle_name, website, gender, locale, picture...)들과 관련된 Scope
            new IdentityResources.Profile(), // -->
        };
        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            // @Scope.Names
            new ApiScope("api.1"),
            new ApiScope("api.2"),
            new ApiScope("api.3")
        };
        
        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
        {
            // @API.Resource : ApiResource.Name 이 Audience 이름 ?!
            new ApiResource("Audience.ApiOne", "ApiOne ApiResource's DisplayName Here")
            {
                Scopes = new HashSet<string>{ "api.1" } // @API.Scope --> see @Scope.Names 참고
            },
        };

        public static IEnumerable<Client> Clients => new List<Client>
        {
            // machine to machine 인증을 위한 사례
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
                    // see @Scope.Names 참고
                    "api.1", 
                    "api.2"
                },
            },
            
            // human to machine 인증을 위한 사례 - LogIn 화면이 보여지고, Consent를 처리하고...  
            new Client
            {
                ClientId = "my.secured.mvc.webapp",
                ClientSecrets =
                {
                    // @MVC.Secret
                    new Secret("very_secret_key_of_web_app".ToSha256())
                },
                AllowedGrantTypes = GrantTypes.Code,
                
                AllowedScopes =
                {
                    // see @Scope.Names 참고
                    "api.1",
                    "api.2",
                    IdentityServerConstants.StandardScopes.OpenId, // "openid"
                    IdentityServerConstants.StandardScopes.Profile, // "profile"
                },
                
                // SecuredMebApp 프로젝트의 호스트에는 자동적으로 "/signin-oidc" 라는 endpoint가 생긴다 
                //  --> 이 Endpoint는 WebApp 클라이언트측 OpenID Connection 라이브러리가 미들웨어를 통해 제공하는듯.
                RedirectUris = {"https://localhost:60001/signin-oidc"},  
                // 위의 url 에 hit 하면, 클라이언트는 자동으로 Login Page로 Redirect 되어야 한다(마찬가지로 클라이언트측 
                // OpenID Connect 라이브러리가 해당 작업을 수행한다)
                
                RequireConsent = false, // IdentityServer4 v3 에서는 기본값이 true 였나보다. v4 에서는 false가 기본값
            }
        };
    }
    
}