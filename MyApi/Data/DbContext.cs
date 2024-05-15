using Microsoft.EntityFrameworkCore;
using MyApi.Models;

namespace MyApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<LaboratoryData> LaboratoryDatas { get; set; }
        public DbSet<AnalysType> AnalysTypes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<DirectionStatusHistory> DirectionStatusHistories { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationships and foreign keys
            modelBuilder.Entity<Direction>()
                .HasOne(s => s.Patient)
                .WithMany(g => g.Directions)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Direction>()
                .HasOne(s => s.LaboratoryData)
                .WithMany(g => g.Directions)
                .HasForeignKey(s => s.LaboratoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Direction>()
                .HasOne(s => s.AnalysType)
                .WithMany(g => g.Directions)
                .HasForeignKey(s => s.AnalysTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Direction>()
                .HasOne(s => s.Department)
                .WithMany(g => g.Directions)
                .HasForeignKey(s => s.DepartmentId)
                .IsRequired(false)  // Indicates that DepartmentId can be null
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DirectionStatusHistory>()
                .HasOne(s => s.Direction)
                .WithMany(g => g.DirectionStatusHistory)
                .HasForeignKey(s => s.DirectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Indicator>()
                .HasOne(s => s.Direction)
                .WithMany(g => g.Indicators)
                .HasForeignKey(s => s.DirectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Set up keys and indexes
            modelBuilder.Entity<LaboratoryData>().HasKey(x => x.Id);
            modelBuilder.Entity<AnalysType>().HasKey(x => x.Id);
            modelBuilder.Entity<Department>().HasKey(x => x.Id);
            modelBuilder.Entity<Patient>().HasKey(x => x.Id);
            modelBuilder.Entity<Direction>().HasKey(x => x.Id);
            modelBuilder.Entity<DirectionStatusHistory>().HasKey(x => x.Id);
            modelBuilder.Entity<Indicator>().HasKey(x => new { x.IndicatorId, x.DirectionId });
            modelBuilder.Entity<User>().HasKey(x => x.Username);

            // Adding indexes to improve querying performance
            modelBuilder.Entity<Direction>()
                .HasIndex(p => p.PatientId)
                .HasDatabaseName("IX_Directions_PatientId");

            modelBuilder.Entity<Direction>()
                .HasIndex(p => p.LaboratoryId)
                .HasDatabaseName("IX_Directions_LaboratoryId");

            modelBuilder.Entity<Direction>()
                .HasIndex(p => p.AnalysTypeId)
                .HasDatabaseName("IX_Directions_AnalysTypeId");
        }
    }
}
