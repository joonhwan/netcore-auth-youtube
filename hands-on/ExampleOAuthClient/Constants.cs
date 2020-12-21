using System.Text;

namespace ExampleOAuthClient
{
    public static class Constants
    {
        public const string SchemeName = "example.client.cookie";
        public const string AuthServerScheme = "example.oauth.server";
        public const string Secret = "very_long_client_secret"; // 너무 짧으면 PII(=personally identifiable information) 오류라는게 생긴다.
        public const string OAuthServerUrl = "https://localhost:5001";
        public static readonly string OAuthServerAuthorizeUrl = $"{OAuthServerUrl}/oauth/authorize";
        public static readonly string OAuthServerTokenUrl     = $"{OAuthServerUrl}/oauth/token";
    }
}