using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mirero.Identity.Models;
using Mirero.Identity.Repositories;

namespace Mirero.Identity.Stores
{
    public class MireroUserStore : IUserStore<MireroUser>
        , IUserEmailStore<MireroUser>
        , IUserPasswordStore<MireroUser>
        , IUserRoleStore<MireroUser>
        , IUserClaimStore<MireroUser>
        , IUserLoginStore<MireroUser>
    {
        private readonly IMireroUserRepository _repository;
        private readonly IMireroUserLoginRepository _userLoginRepository;

        public MireroUserStore(IMireroUserRepository repository, IMireroUserLoginRepository userLoginRepository)
        {
            _repository = repository;
            _userLoginRepository = userLoginRepository;
            var userManagerType = typeof(UserManager<MireroUser>);
            var signInManagerType = typeof(SignInManager<MireroUser>);
            
        }
        
        public void Dispose()
        {
            // no-op
        }

        public Task<string> GetUserIdAsync(MireroUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.KeyOrId.ToString());
        }

        public Task<string> GetUserNameAsync(MireroUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Name);
        }

        public Task SetUserNameAsync(MireroUser user, string userName, CancellationToken cancellationToken)
        {
            user.Name = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(MireroUser user, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer!
            return Task.FromResult(user.Name); 
        }

        public Task SetNormalizedUserNameAsync(MireroUser user, string normalizedName, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer!
            if (user.Name != normalizedName)
            {
                throw new Exception("Please use NoOpLookupNormalizer! "); 
            }
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(MireroUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.AddOrUpdate(user));
        }

        public Task<IdentityResult> UpdateAsync(MireroUser user, CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            return Task.FromResult(_repository.AddOrUpdate(user));
        }

        public Task<IdentityResult> DeleteAsync(MireroUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<MireroUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.FindUserByKeyOrId(userId));
        }

        public Task<MireroUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.FindUserByName(normalizedUserName));
        }

        public Task SetEmailAsync(MireroUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(MireroUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(MireroUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(MireroUser user, bool confirmed, CancellationToken cancellationToken)
        {
            // no-op
            return Task.CompletedTask;
        }

        public Task<MireroUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer!
            return Task.FromResult(_repository.FindUserByEmail(normalizedEmail));
        }

        public Task<string> GetNormalizedEmailAsync(MireroUser user, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer!
            return Task.FromResult(user.Email);
        }

        public Task SetNormalizedEmailAsync(MireroUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            //@ CHECKME: NoOpLookupNormalizer<T> !
            if (user.Email != normalizedEmail)
            {
                throw new Exception("Please use NoOpLookupNormalizer! "); 
            }
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(MireroUser user, string passwordHash, CancellationToken cancellationToken)
        {
            //@CHECKME: NoOpPasswordHasher<T> !
            
            // CreateAsync(TUser, password,..) 를 사용하는 경우에는 여기에서 password가 갱신된다. 
            user.Password = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(MireroUser user, CancellationToken cancellationToken)
        {
            //@CHECKME: NoOpPasswordHasher<T> !
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(MireroUser user, CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            return Task.FromResult(!string.IsNullOrEmpty(user.Password));
        }

        public Task AddToRoleAsync(MireroUser user, string roleName, CancellationToken cancellationToken)
        {
            user.Roles.Add(roleName);
            _repository.AddOrUpdate(user);
            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(MireroUser user, string roleName, CancellationToken cancellationToken)
        {
            user.Roles.Remove(roleName);
            _repository.AddOrUpdate(user);
            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(MireroUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult((IList<string>) user.Roles);
        }

        public async Task<bool> IsInRoleAsync(MireroUser user, string roleName, CancellationToken cancellationToken)
        {
            var roles = await GetRolesAsync(user, cancellationToken);
            return roles.Any(s => s == roleName);
        }

        public Task<IList<MireroUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var users = _repository.FindUserByRole(roleName);
            return Task.FromResult(users);
        }

        public Task<IList<Claim>> GetClaimsAsync(MireroUser user, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>();
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim("mirero.role", role));
            }
            return Task.FromResult((IList<Claim>)claims);
        }

        public Task AddClaimsAsync(MireroUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceClaimAsync(MireroUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimsAsync(MireroUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<MireroUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddLoginAsync(MireroUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            var mireroUserLogin = new MireroUserLogin(user, login);
            _userLoginRepository.Add(mireroUserLogin);
            return Task.CompletedTask;
        }

        public Task<MireroUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var userLogin = _userLoginRepository.FindByProviderKey(loginProvider, providerKey);
            if (userLogin != null)
            {
                return Task.FromResult(_repository.FindUserByKeyOrId(userLogin.KeyOrId));
            }

            return Task.FromResult<MireroUser>(null);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(MireroUser user, CancellationToken cancellationToken)
        {
            var result = _userLoginRepository.FindByKey(user.KeyOrId).Cast<UserLoginInfo>().ToList() as IList<UserLoginInfo>;
            return Task.FromResult(result);
        }

        public Task RemoveLoginAsync(MireroUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            _userLoginRepository.RemoveBy(user.KeyOrId, loginProvider, providerKey);
            return Task.CompletedTask;
        }
    }
}