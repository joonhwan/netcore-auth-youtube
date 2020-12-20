using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OAuthServer
{
    public static class Constants
    {
        public const string Audience = "https://localhost:5001"; // TODO : from config or..
        public const string Issuer = Audience;
        public const string Secret = "very_long_secret"; // 너무 짧으면 PII(=personally identifiable information) 오류라는게 생긴다. 

        public static byte[] SecretKey => Encoding.UTF8.GetBytes(Constants.Secret);
        public static SecurityKey IssuerSigningSecurityKey => new SymmetricSecurityKey(SecretKey);
    }
}