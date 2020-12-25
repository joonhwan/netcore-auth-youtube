using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Mirero.Identity.Internals
{
    public class MireroClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // ClaimsIdentity 인 경우에만 claim의 추가 삭제가 가능하다. 
            var claimsId = principal.Identity as ClaimsIdentity;
            if (claimsId == null)
            {
                return Task.FromResult(principal);
            }
            
            // bob 아저씨라면, 초코칩 쿠키를 먹으 자격이 있어요. 
            if (claimsId.HasClaim("name", "bob"))
            {
                claimsId.AddClaim(new Claim("granted.cookiejar.id", "choco.cookie.01"));
            }

            return Task.FromResult(principal);
        }
    }
}