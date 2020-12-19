using System;

namespace GrandmaAuthLib
{
    public class GrandmaUser
    {
        public Guid KeyOrId { get; set; } = Guid.Empty;
        public string Name { get; set; } = ""; // ID 
        public string Password { get; set; } = ""; //Password
        public string Email { get; set; } = "";
        public DateTime BirthDate { get; set; } = DateTime.MinValue;
    }
}