using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Mirero.Identity.Validators
{
    internal class NoOpPasswordValidator<T> : IPasswordValidator<T>
        where T : class
    {
        public Task<IdentityResult> ValidateAsync(UserManager<T> manager, T user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }
    }
}