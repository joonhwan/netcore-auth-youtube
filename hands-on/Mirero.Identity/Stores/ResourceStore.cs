using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Mirero.Identity.Stores
{
    public class ResourceStore : IResourceStore
    {
        private IEnumerable<IdentityResource> _identities;
        private IEnumerable<ApiResource> _apis;
        private IEnumerable<ApiScope> _scopes;

        public ResourceStore()  
        {
            _identities = DefaultResource.IdentityResources;
            _apis = DefaultResource.ApiResources;
            _scopes = DefaultResource.ApiScopes;

            if (_identities.HasDuplicates(identity => identity.Name) ||
                _apis.HasDuplicates(api => api.Name) ||
                _scopes.HasDuplicates(scope => scope.Name))
            {
                // TODO CHECK 나중에  DB 를 설계한다던지 할 때, 참고해야 함.
                throw new InvalidDataException("Name은 identities, apis, scopes에서 각각 모두 고유해야함.");
            }
        }
        
        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            // TODO DB 등 저장소에서 질의해서 가져와야 함. 
            var result = _identities.Where(identity => scopeNames.Contains(identity.Name));
            return Task.FromResult(result);
        }

        public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var result = _scopes.Where(scope => scopeNames.Contains(scope.Name));
            return Task.FromResult(result);
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var result = _apis.Where(api => api.Scopes.Any(scope => scopeNames.Contains(scope)));
            return Task.FromResult(result);
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null) throw new ArgumentNullException(nameof(apiResourceNames));

            var query = from a in _apis
                where apiResourceNames.Contains(a.Name)
                select a;
            return Task.FromResult(query);
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(_identities, _apis, _scopes);
            return Task.FromResult(result);
        }
    }

    internal static class DefaultResource
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
                    "name",
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
    }
}