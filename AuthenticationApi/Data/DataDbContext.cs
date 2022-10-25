using AuthenticationApi.Authentication;
using AuthenticationApi.Models.Accounts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Data
{
    public class DataDbContext : IdentityDbContext<ApplicationUser>
    {
        public DataDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Registration> Register { get; set; }
    }
}
