namespace IdentityService.Models
{
    public class ExternalRegisterViewModel
    {
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string ReturnUrl { get; set; }
    }
}