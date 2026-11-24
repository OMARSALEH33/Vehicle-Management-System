using Microsoft.EntityFrameworkCore;
using Challenge_1_Build_APIS.Core.Models;
using Challenge_1_Build_APIS.Core.Model;

namespace Challenge_1_Build_APIS.Persistence
{
    // DBContext instance represents a session with the database
    // and can be used to query and save instances.
    public class VegaDbContext : DbContext
    {
        public DbSet<Make> Makes { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleFeature> VehicleFeatures { get; set; }
        public DbSet<User> users { get; set; }

        // تمرير الـ options القادمة من Program.cs مباشرة إلى الـ Base clas
        public VegaDbContext(DbContextOptions<VegaDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Make>()
                .HasIndex(m => m.Code)
                .IsUnique();

            modelBuilder.Entity<Model>()
                .HasOne(m => m.Make)
                .WithMany()
                .HasForeignKey(m => m.MakeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Model>()
                .HasIndex(m => m.Name)
                .IsUnique();

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VehicleFeature>()
                .HasKey(vf => new { vf.VehicleId, vf.FeatureId });

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Model)
                .WithMany()
                .HasForeignKey(v => v.ModelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


}