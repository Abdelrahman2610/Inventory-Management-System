using Microsoft.AspNetCore.Identity;

namespace Inventory_Managment_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public int? Location_id { get; set; }
        public bool is_active { get; set; }

        // Add security question and answer
        public string SecurityQuestion { get; set; } // e.g., "What is your mother's maiden name?"
        public string SecurityAnswer { get; set; }   // Case-insensitive answer
    }
}