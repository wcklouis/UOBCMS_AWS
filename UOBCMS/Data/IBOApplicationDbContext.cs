using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Models;
using UOBCMS.Models.IBO;

namespace UOBCMS.Data
{
    public class IBOApplicationDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define keyless entity type
            modelBuilder.Entity<SelectListGroup>(eb =>
            {
                eb.HasNoKey();
                // If you have specific properties, configure them here                
            });

            // Other configurations
            modelBuilder.Entity<SelectListItem>(entity =>
            {
                // Ignore the Group navigation property
                entity.Ignore(e => e.Group);
            });

            modelBuilder.Entity<Instrument>()
                .ToTable("Instrument")
                .Property(p => p.Instr)
                .HasColumnName("Instrument"); // Map to the actual column name in the database

            modelBuilder.Entity<ClntCRS>()
                .ToTable("ClntCRS", "dbo");

            modelBuilder.Entity<ClntContact>()
               .ToTable("ClntContact", "dbo");

            //modelBuilder.Entity<Instrument>()
            //    .HasKey(ca => new { ca.Market, ca.Instr });

            modelBuilder
                .Entity<Instrument>(eb =>
                {
                    eb.HasNoKey(); // Specify that this is a keyless entity type
                    // Optionally, you can specify the table or view it maps to
                    // eb.ToTable("YourTableName");
                    // Or if it's a view:
                    // eb.ToView("YourViewName");

                    eb.Property(e => e.CalculationPrice)
                    .HasColumnType("numeric(14, 6)") // Adjust precision and scale as needed
                    .HasPrecision(14, 6); // This sets precision and scale

                    eb.Property(e => e.DailyQtyLimit)
                    .HasColumnType("numeric(28, 8)") // Adjust precision and scale as needed
                    .HasPrecision(28, 8); // This sets precision and scale

                    eb.Property(e => e.LastSuspendPrice)
                    .HasColumnType("numeric(14, 6)") // Adjust precision and scale as needed
                    .HasPrecision(14, 6); // This sets precision and scale

                    eb.Property(e => e.LotSize)
                    .HasColumnType("numeric(28, 8)") // Adjust precision and scale as needed
                    .HasPrecision(28, 8); // This sets precision and scale

                    eb.Property(e => e.MarginPercent)
                    .HasColumnType("numeric(5, 2)") // Adjust precision and scale as needed
                    .HasPrecision(5, 2); // This sets precision and scale

                    eb.Property(e => e.ParValue)
                    .HasColumnType("numeric(14, 6)") // Adjust precision and scale as needed
                    .HasPrecision(14, 6); // This sets precision and scale

                    eb.Property(e => e.Price)
                    .HasColumnType("numeric(14, 6)") // Adjust precision and scale as needed
                    .HasPrecision(14, 6); // This sets precision and scale

                    eb.Property(e => e.PriceCap)
                    .HasColumnType("numeric(14, 6)") // Adjust precision and scale as needed
                    .HasPrecision(14, 6); // This sets precision and scale

                    eb.Property(e => e.TransactionQtyLimit)
                    .HasColumnType("numeric(28, 8)") // Adjust precision and scale as needed
                    .HasPrecision(28, 8); // This sets precision and scale

                    eb.Property(e => e.TransactionValueLimit)
                    .HasColumnType("numeric(28, 8)") // Adjust precision and scale as needed
                    .HasPrecision(28, 8); // This sets precision and scale

                    eb.Property(e => e.DailyValueLimit)
                    .HasColumnType("numeric(28, 8)") // Adjust precision and scale as needed
                    .HasPrecision(28, 8); // This sets precision and scale
                });

            modelBuilder
                .Entity<InstrumentHolding>(eb =>
                {
                    eb.HasNoKey(); // Specify that this is a keyless entity type
                    // Optionally, you can specify the table or view it maps to
                    // eb.ToTable("YourTableName");
                    // Or if it's a view:
                    // eb.ToView("YourViewName");

                    eb.Property(e => e.AsAt)
                    .HasColumnType("numeric(28, 8)") // Adjust precision and scale as needed
                    .HasPrecision(28, 8); // This sets precision and scale
                });

            modelBuilder.Ignore<SelectListItem>(); // Ignore SelectListItem

            modelBuilder
                .Entity<ClntCRS>(eb =>
                {
                    eb.HasNoKey(); // Specify that this is a keyless entity type
                    // Optionally, you can specify the table or view it maps to
                    // eb.ToTable("YourTableName");
                    // Or if it's a view:
                    // eb.ToView("YourViewName");
                });

            modelBuilder
                .Entity<ClntContact>(eb =>
                {
                    eb.HasNoKey(); // Specify that this is a keyless entity type
                    // Optionally, you can specify the table or view it maps to
                    // eb.ToTable("YourTableName");
                    // Or if it's a view:
                    // eb.ToView("YourViewName");
                });

            modelBuilder
                .Entity<UserIDContact>(eb =>
                {
                    eb.HasNoKey(); // Specify that this is a keyless entity type
                    // Optionally, you can specify the table or view it maps to
                    // eb.ToTable("YourTableName");
                    // Or if it's a view:
                    // eb.ToView("YourViewName");
                });
        }

        public IBOApplicationDbContext(DbContextOptions<IBOApplicationDbContext> options) : base(options)
        {
        }        

        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<InstrumentHolding> InstrumentHoldings { get; set; }

        public DbSet<ClntCRS> ClntCRS { get; set; }

        public DbSet<ClntContact> ClntContact { get; set; }

        public DbSet<UserIDContact> UserIDContact { get; set; }
    }
}
