using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamenNet.Models
{
    public class ExamDbContext : DbContext
    {
        public ExamDbContext(DbContextOptions<ExamDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
            });

        //    builder.Entity<Pachet>(entity =>
        //    {
        //        entity.HasIndex(p => p.CodTracking).IsUnique();
        //    });

        //    //asa fac sa fie unica o cheie primara compusa (cu HasKey)
        //    //builder.Entity<UserUserRole>(entity => {
        //    //    entity.HasKey(u => new { u.UserId, u.UserRoleId }).IsUnique();
        //    //});

        //    //builder.Entity<Comment>()
        //    //    .HasOne(e => e.Expense)
        //    //    .WithMany(c => c.Comments)
        //    //    .OnDelete(DeleteBehavior.Cascade);

        //    //builder.Entity<Comment>()
        //    //    .HasOne(c => c.AddedBy)
        //    //    .WithMany(c => c.Comments)
        //    //    .OnDelete(DeleteBehavior.Cascade);
        }

          public DbSet<User> Users { get; set; }
       // public DbSet<Pachet> Pachete { get; set; }
    }
}

