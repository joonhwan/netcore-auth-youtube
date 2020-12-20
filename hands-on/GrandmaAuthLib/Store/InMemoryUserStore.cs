using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GrandmaAuthLib.Store
{
    public class InMemoryUserStore 
        : IUserStore<GrandmaUser>
        , IUserEmailStore<GrandmaUser>
        , IUserPasswordStore<GrandmaUser>
        //, IUserRoleStore<GrandmaUser>
        , IUserClaimStore<GrandmaUser>
    {
        private InMemoryGrandmaAuthStore _store;

        public InMemoryUserStore(InMemoryGrandmaAuthStore store)
        {
            var userManagerType = typeof(UserManager<GrandmaUser>);
            var signInManagerType = typeof(SignInManager<GrandmaUser>);
            _store = store;
        }
        
        public void Dispose()
        {
            // no-op
        }

        public Task<string> GetUserIdAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.KeyOrId.ToString());
        }

        public Task<string> GetUserNameAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Name);
        }

        public Task SetUserNameAsync(GrandmaUser user, string userName, CancellationToken cancellationToken)
        {
            user.Name = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer!
            return Task.FromResult(user.Name); 
        }

        public Task SetNormalizedUserNameAsync(GrandmaUser user, string normalizedName, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer!
            if (user.Name != normalizedName)
            {
                throw new Exception("Please use NoOpLookupNormalizer! "); 
            }
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(_store.AddOrUpdate(user));
        }

        public Task<IdentityResult> UpdateAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            return Task.FromResult(_store.AddOrUpdate(user));
        }

        public Task<IdentityResult> DeleteAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<GrandmaUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_store.FindUserByKeyOrId(userId));
        }

        public Task<GrandmaUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_store.FindUserByName(normalizedUserName));
        }

        public Task SetEmailAsync(GrandmaUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(GrandmaUser user, bool confirmed, CancellationToken cancellationToken)
        {
            // no-op
            return Task.CompletedTask;
        }

        public Task<GrandmaUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer!
            return Task.FromResult(_store.FindUserByEmail(normalizedEmail));
        }

        public Task<string> GetNormalizedEmailAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer!
            return Task.FromResult(user.Email);
        }

        public Task SetNormalizedEmailAsync(GrandmaUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer<T> !
            if (user.Email != normalizedEmail)
            {
                throw new Exception("Please use NoOpLookupNormalizer! "); 
            }
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(GrandmaUser user, string passwordHash, CancellationToken cancellationToken)
        {
            //@CHECKME: NoOpPasswordHasher<T> !
            
            // CreateAsync(TUser, password,..) 를 사용하는 경우에는 여기에서 password가 갱신된다. 
            user.Password = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            //@CHECKME: NoOpPasswordHasher<T> !
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            return Task.FromResult(!string.IsNullOrEmpty(user.Password));
        }

        public Task AddToRoleAsync(GrandmaUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveFromRoleAsync(GrandmaUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            var roles = new List<string>();
            switch (user.Name.ToLower())
            {
                case "admin":
                    roles.Add("admin");
                    break;
                case "bob":
                    roles.Add("poweruser");
                    break;
                default:
                    roles.Add("user");
                    break;
            }
            return Task.FromResult((IList<string>) roles);
        }

        public async Task<bool> IsInRoleAsync(GrandmaUser user, string roleName, CancellationToken cancellationToken)
        {
            var roles = await GetRolesAsync(user, cancellationToken);
            return roles.Any(s => s == roleName);
        }

        public Task<IList<GrandmaUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(GrandmaUser user, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>();
            switch (user.Name.ToLower())
            {
                case "admin":
                    claims.Add(new Claim(ClaimTypes.Role, "admin"));
                    break;
                case "bob":
                    claims.Add(new Claim(ClaimTypes.Role, "power-user"));
                    break;
                default:
                    claims.Add(new Claim(ClaimTypes.Role, "user"));
                    break;
            }
            return Task.FromResult((IList<Claim>)claims);
        }

        public Task AddClaimsAsync(GrandmaUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceClaimAsync(GrandmaUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimsAsync(GrandmaUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<GrandmaUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}