using System.Threading.Tasks;
using IdentityServer4.Services;

namespace Mirero.Identity.Stores
{
    public class CorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            // TODO origin 에 따라 API 사용을 허용할 것인지 ...를 구현.
            
            /* 참고로 이것과 관련한 데이터 모델은 IdentityServer4 의 경우 아래를 사용.
             public class ClientCorsOrigin
            {
                public int Id { get; set; }
                public string Origin { get; set; }

                public int ClientId { get; set; }
                public Client Client { get; set; }
            }
            
            --> 위에서 ClientId 는  Client 테이블의 FK 같은 것.
             */
            
            // 일단 모두 허용.
            return Task.FromResult(true); 
        }
    }
}