using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Inventory_Managment_System.Models;

namespace Inventory_Managment_System.Data
{
    public class SecurityDbContext : IdentityDbContext<ApplicationUser>
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
            : base(options) { }
    }

}