using Microsoft.AspNetCore.Identity;

namespace Mirero.Identity.Models
{
    public class MireroUserLogin : UserLoginInfo
    {
        public string KeyOrId { get; set; } // link to MireroUser

        public MireroUserLogin(MireroUser user, UserLoginInfo userLogin) 
            : base(userLogin.LoginProvider, userLogin.ProviderKey, userLogin.ProviderDisplayName)
        {
            KeyOrId = user.KeyOrId;
        }
    }
}