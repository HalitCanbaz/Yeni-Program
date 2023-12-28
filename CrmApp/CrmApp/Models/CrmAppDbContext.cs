using CrmApp.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrmApp.Models
{
    public class CrmAppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public CrmAppDbContext(DbContextOptions<CrmAppDbContext> options) :
            base(options)
        {

        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }
        public DbSet<AssetFault> AssetFaults { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Fault> Faults { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }

    }
}
