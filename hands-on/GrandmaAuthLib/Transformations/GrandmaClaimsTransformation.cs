using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace GrandmaAuthLib.Transformations
{
    // 할머니는 특정사람에게만 특정 쿠키통을 허락한다.
    public class GrandmaClaimsTransformation : IClaimsTransformation
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
            if (claimsId.HasClaim(ClaimTypes.Name, "bob"))
            {
                claimsId.AddClaim(new Claim("granted.cookiejar.id", "choco.cookie.01"));
            }

            return Task.FromResult(principal);
        }
    }
}