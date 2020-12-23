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
            
            // 아래 주석처리 --> see @UserInfoEndpoint 
            // // 'profile' scope : 사용자 Claim(정보, 예: name, family_name, middle_name, website, gender, locale, picture...)들과 관련된 Scope
            // new IdentityResources.Profile(), // -->
            
            // @AddClaimToIdToken 
            //  사용자 정의 scope. --> 이 scope 에 여러가지 사용자 관련 정보(=Claim) 들을 정의할 수 있다. 
            new IdentityResource
            {
                Name = "scope.mirero.profile",
                Description = "Mirero Co. LTD specific Data",
                UserClaims =
                {
                    // identity 에 포함되어야할 사용자 정의 claim  
                    "mirero.role", 
                }
            }
        };
        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            // @Scope.Names
            new ApiScope("scope.mirero.api.type.secret", new []
            {
                // 이 scope 이 요청되면 전송되어야할 "추가" claim 목록 ?!
                "mirero.role",
            }),
            new ApiScope("scope.mirero.api.type.gateway", new []
            {
                // 이 scope 이 요청되면 전송되어야할 "추가" claim 목록 ?!
                "mirero.role",
            }),
            new ApiScope("scope.mirero.api.type.blablah"),
        };
        
        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
        {
            // @API.Resource : ApiResource.Name 이 Audience 이름 ?!
            new ApiResource("audience.mirero.secret.api", "ApiOne ApiResource's DisplayName Here")
            {
                Scopes = new HashSet<string>{ "scope.mirero.api.type.secret" } // @API.Scope --> see @Scope.Names 참고
            },
            new ApiResource("audience.mirero.gateway.api")
            {
                Scopes = new HashSet<string>{ "scope.mirero.api.type.gateway" }
            }
        };

        public static IEnumerable<Client> Clients => new List<Client>
        {
            // machine to machine 인증을 위한 사례
            // authentication 은 하지 않음. 
            new Client
            {
                ClientId = "client.mirero.worker.service",
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
                    "scope.mirero.api.type.secret", 
                    "scope.mirero.api.type.gateway"
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
                    IdentityServerConstants.StandardScopes.OpenId, // "openid"
                    // 아래 주석처리 --> see @UserInfoEndpoint 
                    //IdentityServerConstants.StandardScopes.Profile, // "profile"
                    "scope.mirero.api.type.secret",
                    "scope.mirero.api.type.gateway",
                    "scope.mirero.profile", // @AddClaimToIdToken 사용자 정의된 scope 를 추가
                },
                
                // SecuredMebApp 프로젝트의 호스트에는 자동적으로 "/signin-oidc" 라는 endpoint가 생긴다 
                //  --> 이 Endpoint는 WebApp 클라이언트측 OpenID Connection 라이브러리가 미들웨어를 통해 제공하는듯.
                RedirectUris = {"https://localhost:60001/signin-oidc"},  
                // 위의 url 에 hit 하면, 클라이언트는 자동으로 Login Page로 Redirect 되어야 한다(마찬가지로 클라이언트측 
                // OpenID Connect 라이브러리가 해당 작업을 수행한다)
                
                RequireConsent = false, // IdentityServer4 v3 에서는 기본값이 true 였나보다. v4 에서는 false가 기본값
                
                // @AddClaimToIdToken
                //
                // 아래 옵션은 원래 false. 하지만, true 로 설정하면, id_token 값에 사용자 정보(=Claim)이 포함된다.
                // 
                // 이게 false면, 사용자 정보를 얻기 위해 "https://{host}/userinfo" 의 endpoint를 통해 얻어가는 수고를 해야 한다.
                //(--> backchannel 로 userinfo 를 얻어가므로 frontend 에는 사용자 정보가 날아가지 않는 장점이 있는듯?)
                // AlwaysIncludeUserClaimsInIdToken = true,
                
                // @UserInfoEndpoint 
                //    Front End 쪽으로 모든 정보를 보내지 않으려면, 아래 처럼 false가 되어 야 함.
                //   true 로 하는 경우는 Single Page App 등 MVC가 아닌 App 의 경우여야 할 것 같음. 
                // 
                //  또한, 위  
                AlwaysIncludeUserClaimsInIdToken = false, // NOTE: 디폴트가 false 임. 
                
                Claims = new List<ClientClaim>
                {
                    new ClientClaim("mirero.id.server", "v1.0")
                }
            }
        };
    }
    
}