// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WpfDrawingApp.Models;

namespace WpfDrawingApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<LineSegment> LineSegments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Явное указание таблицы и полей для PostgreSQL
            modelBuilder.Entity<LineSegment>(entity =>
            {
                entity.ToTable("line_segments");
                entity.Property(e => e.Length).HasColumnName("length");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}