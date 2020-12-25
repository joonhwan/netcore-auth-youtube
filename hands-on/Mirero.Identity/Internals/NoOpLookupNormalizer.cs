using Microsoft.AspNetCore.Identity;

namespace Mirero.Identity
{
    internal class NoOpLookupNormalizer : ILookupNormalizer
    {
        public string Normalize(string key)
        {
            return key;
        }

        public string NormalizeName(string name)
        {
            return name;
        }

        public string NormalizeEmail(string email)
        {
            return email;
        }
    }
}