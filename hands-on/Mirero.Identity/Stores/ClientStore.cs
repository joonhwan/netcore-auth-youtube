using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Mirero.Identity.Stores
{
    public class ClientStore : IClientStore
    {
        private List<Client> _clients;

        public ClientStore()
        {
            _clients = PopulateDefaults();
        }
        
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var foundClient = _clients.FirstOrDefault(client => client.ClientId == clientId);
            return Task.FromResult<Client>(foundClient);
        }

        private List<Client> PopulateDefaults()
        {
            var clients = new List<Client>
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
                    ClientId = "mirero.secured.mvc.app",
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
                    },

                    // @OfflineAccess
                    AllowOfflineAccess = true,
                },
                // SPA 를 위한 Implicit Flow 
                new Client()
                {
                    ClientId = "mirero.secured.web.app",

                    // 원래 client secret 이 있으면 좋지만, SPA의 경우, javascript 코드에 
                    // 박힐 수 밖에 읍다. 따라서 여기에서는 그냥 client secret 없이 하는 거 같다. ?
                    // --> 말이 되나?  
                    // ClientSecrets = { new Secret("very_secret_key_of_web_app".ToSha256()) },
                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Implicit,
                    // 인증서버가 로그인을 역으로 알려줄 수 있는 웹앱의 사이트주소(잠깐동안 스쳐가는 페이지.)
                    RedirectUris = {"https://localhost:60011/signin"},
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
                    AllowedCorsOrigins =
                    {
                        "https://localhost:60011"
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AccessTokenLifetime = 5, // 테스트를 위해 30초만. (디폴트는 3600초=1시간)
                }
            };
            return clients;
        }
    }
}