using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GrandmaAuthLib.Store
{
    internal class AuthModel
    {
        public AuthModel(GrandmaUser user, params GrandmaRole[] roles)
        {
            User = user;
            Roles = roles != null ? new List<GrandmaRole>(roles) : new List<GrandmaRole>();
        }
        public GrandmaUser User { get; set; }
        public List<GrandmaRole> Roles { get; set; }
    }
    public class InMemoryGrandmaAuthStore
    {
        private readonly List<AuthModel> _data = new List<AuthModel>();
        private readonly ILogger<InMemoryGrandmaAuthStore> _logger;

        public InMemoryGrandmaAuthStore(ILogger<InMemoryGrandmaAuthStore> logger)
        {
            _logger = logger;
            _logger.LogInformation("Initializing InMemory Grandma Auth Store...");
            Populate();
        }

        private void Populate()
        {
            AddOrUpdate(new GrandmaUser
            {
                Name = "bob",
                Password = "mirero",
                Email = "bob@mirero.co.kr",
                BirthDate = new DateTime(1971, 12, 11)
            });
        }

        public IdentityResult AddOrUpdate(GrandmaUser user)
        {
            // try update first
            var existingAuth = _data.FirstOrDefault(model => model.User.KeyOrId == user.KeyOrId);
            if (existingAuth != null)
            {
                _logger.LogInformation("Updating User : {0}", user.Name);
                existingAuth.User.Email = user.Email;
                existingAuth.User.Name = user.Name;
                existingAuth.User.Password = user.Password;
                existingAuth.User.BirthDate = user.BirthDate;
                return IdentityResult.Success;
            }
            
            // try add
            if (_data.Any(model => model.User.Name == user.Name))
            {
                _logger.LogError("Cannot Add User : {0} => Name Conflict!", user.Name);
                return IdentityResult.Failed(new[]
                {
                    new IdentityError {Code = "DUP_NAME"},
                });
            }
            
            _logger.LogInformation("Adding New User : {0}/{1}", user.Name, user.Password);

            user.KeyOrId = Guid.NewGuid();
            var userInStore = new GrandmaUser()
            {
                Email = user.Email,
                Name = user.Name,
                Password = user.Password,
                BirthDate = user.BirthDate,
                KeyOrId = user.KeyOrId
            };
            _data.Add(new AuthModel(userInStore));

            _logger.LogInformation("---- Auth Model List ----");
            foreach (var item in _data)
            {
                _logger.LogInformation("{0}/{1}/{2}", item.User.Name, item.User.Password, item.User.Email);
            }
            return IdentityResult.Success;
        }

        public GrandmaUser FindUserByKeyOrId(string userId)
        {
            return _data.FirstOrDefault(model => model.User.KeyOrId.ToString() == userId)?.User;
        }   

        public GrandmaUser FindUserByName(string normalizedUserName)
        {
            return _data.FirstOrDefault(model => model.User.Name == normalizedUserName)?.User;
        }

        public GrandmaUser FindUserByEmail(string normalizedEmail)
        {
            return _data.FirstOrDefault(model => model.User.Email == normalizedEmail)?.User;
        }
    }
}