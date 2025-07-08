using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Models;
using UOBCMS.Models.eform;

namespace UOBCMS.Data
{
    public class EFormApplicationDbContext : DbContext
    {
        public EFormApplicationDbContext(DbContextOptions<EFormApplicationDbContext> options) : base(options)
        {
            this.Database.SetCommandTimeout(120); // Set timeout to 120 seconds
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define keyless entity type
            modelBuilder.Entity<SelectListGroup>(eb =>
            {
                eb.HasNoKey();
                // If you have specific properties, configure them here                
            });

            modelBuilder.Entity<eformUser>()
                .ToTable("tblEFormUser", "dbo");

            modelBuilder.Entity<eformUserGroup>()
                .ToTable("tblEFormUserGroup", "dbo");

            modelBuilder.Entity<eformUserInGroup>()
                .ToTable("tblEFormUserInGroup", "dbo");

            modelBuilder.Entity<eformUser>()
                .HasMany(c => c.eFormUserInGroups)
                .WithOne(a => a.eFormUser)
                .HasForeignKey(a => a.Userid);

            modelBuilder.Entity<eformUserGroup>()
                .HasMany(c => c.eFormUserInGroups)         
                .WithOne(w => w.eFormUserGroup)           
                .HasForeignKey(w => w.Usergpid);

            // Triggers
            modelBuilder.Entity<eformUser>()
                .ToTable(tb => tb.HasTrigger("tr_tblEFormUser_dbopr"));

            modelBuilder.Entity<eformUserGroup>()
                .ToTable(tb => tb.HasTrigger("tr_tblEFormUserGroup_dbopr"));

            modelBuilder.Entity<eformUserInGroup>()
                .ToTable(tb => tb.HasTrigger("tr_tblEFormUserInGroup_dbopr"));

            modelBuilder.Ignore<SelectListItem>(); // Ignore SelectListItem
        }

        public DbSet<eformUser> EFormUsers { get; set; }
        public DbSet<eformUserGroup> EFormUserGroups { get; set; }
        public DbSet<eformUserInGroup> EFormUserInGroups { get; set; }
    }
}
