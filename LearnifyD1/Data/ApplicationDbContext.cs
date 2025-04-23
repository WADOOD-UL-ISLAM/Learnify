using LearnifyD1.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnifyD1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Batch> Batches { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<StudentBatch> studentBatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentBatch>().ToTable("StudentBatches");
            // Configure composite key and relationships
            modelBuilder.Entity<StudentBatch>().HasKey(sb => new { sb.StudentId, sb.BatchId });

            modelBuilder.Entity<StudentBatch>()
                .HasOne(s => s.student)
                .WithMany(sb => sb.studentBatches)
                .HasForeignKey(s => s.StudentId);

            modelBuilder.Entity<StudentBatch>()
                .HasOne(b => b.batch)
                .WithMany(sb => sb.studentbatches)
                .HasForeignKey(b => b.BatchId);

        }


    }
}
