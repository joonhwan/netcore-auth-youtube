using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Mirero.Identity.Models;

namespace Mirero.Identity.Repositories
{
    internal class TestInMemoryMireroUserRepository : IMireroUserRepository
    {
        private static List<MireroUser> _users =
            new List<MireroUser>()
            {
                new MireroUser
                {
                    KeyOrId = "123456789-123456789", //Guid.NewGuid().ToString(),
                    Email = "bob@mirero.co.kr",
                    Name = "bob",
                    Password = "mirero",
                    Roles =
                    {
                        "admin",
                        "manager"
                    }
                },
            };

        public TestInMemoryMireroUserRepository()
        {
        }

        public IdentityResult AddOrUpdate(MireroUser user)
        {
            lock (_users)
            {
                var existing = _users.FirstOrDefault(record => record.KeyOrId == user.KeyOrId);
                if (existing != null)
                {
                    existing.Email = user.Email;
                    //TODO Make Sense to change name?
                    //existing.Name = user.Name;
                    existing.Password = user.Password;
                    existing.Roles = user.Roles.ToArray().ToList(); // clone list.
                }
                else
                {
                    _users.Add(user);
                }
            }

            return IdentityResult.Success;
        }

        public MireroUser FindUserByKeyOrId(string userId)
        {
            lock (_users)
            {
                return _users.FirstOrDefault(record => record.KeyOrId == userId);
            }
        }

        public MireroUser FindUserByName(string name)
        {
            lock (_users)
            {
                // throw new System.NotImplementedException();
                return _users.FirstOrDefault(record => record.Name == name);
            }
        }

        public MireroUser FindUserByEmail(string email)
        {
            lock (_users)
            {
                return _users.FirstOrDefault(record => record.Email == email);
            }
        }

        public IList<MireroUser> FindUserByRole(string roleName)
        {
            lock (_users)
            {
                return _users.Where(user => user.Roles.Contains(roleName)).ToList();
            }
        }
    }
}