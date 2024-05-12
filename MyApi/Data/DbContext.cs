using Microsoft.EntityFrameworkCore;
using MyApi.Models;

namespace MyApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Laboratory> Laboratories { get; set; }
        public DbSet<AnalysisType> AnalysisTypes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationships
            modelBuilder.Entity<Direction>()
                .HasOne<Patient>(s => s.Patient)
                .WithMany(g => g.Directions)
                .HasForeignKey(s => s.PatientId);

            modelBuilder.Entity<Direction>()
                .HasOne<Laboratory>(s => s.Laboratory)
                .WithMany(g => g.Directions)
                .HasForeignKey(s => s.LaboratoryId);

            modelBuilder.Entity<Direction>()
                .HasOne<AnalysisType>(s => s.AnalysisType)
                .WithMany(g => g.Directions)
                .HasForeignKey(s => s.AnalysisTypeId);

            modelBuilder.Entity<Direction>()
                .HasOne<Department>(s => s.Department)
                .WithMany(g => g.Directions)
                .HasForeignKey(s => s.DepartmentId);

            modelBuilder.Entity<StatusHistory>()
                .HasOne<Direction>(s => s.Direction)
                .WithMany(g => g.StatusHistories)
                .HasForeignKey(s => s.DirectionId);

            modelBuilder.Entity<Indicator>()
                .HasOne<Direction>(s => s.Direction)
                .WithMany(g => g.Indicators)
                .HasForeignKey(s => s.DirectionId);

            // Set up keys and indexes
            modelBuilder.Entity<Laboratory>().HasKey(x => x.Id);
            modelBuilder.Entity<AnalysisType>().HasKey(x => x.Id);
            modelBuilder.Entity<Department>().HasKey(x => x.Id);
            modelBuilder.Entity<Patient>().HasKey(x => x.Id);
            modelBuilder.Entity<Direction>().HasKey(x => x.Id);
            modelBuilder.Entity<StatusHistory>().HasKey(x => x.Id);
            modelBuilder.Entity<Indicator>().HasKey(x => new { x.IndicatorId, x.DirectionId });
            modelBuilder.Entity<User>().HasKey(x => x.Username);
        }
    }
}
