using Microsoft.AspNetCore.Identity;

namespace Mirero.Identity.Validators
{
    internal class NoOpPasswordHasher<T> : IPasswordHasher<T>
        where T : class
    {
        public string HashPassword(T user, string password)
        {
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(T user, string hashedPassword,
            string providedPassword)
        {
            if (hashedPassword == providedPassword)
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }
    }
}