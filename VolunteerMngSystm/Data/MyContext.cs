using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteerMngSystm.Models;
using Microsoft.EntityFrameworkCore;

namespace VolunteerMngSystm.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) {}

        public MyContext() : base() { }
        public DbSet<Users> Users { get; set; }
        public DbSet<VolunteeringTask> Tasks { get; set; }
        public DbSet<Organisations> Organisations { get; set; }
        public DbSet<Requests> Requests { get; set; }
        public DbSet<Expertise> Expertises { get; set; }
        public DbSet<SelectedExpertise> SelectedExpertises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Users>().ToTable("Date of Birth");
            //modelBuilder.Entity<Users>().ToTable("Personal ID");
            //modelBuilder.Entity<Users>().ToTable("Postal Code");
            //modelBuilder.Entity<Users>().ToTable("Phone number");
            modelBuilder.Entity<Requests>().ToTable("Request");
            modelBuilder.Entity<Requests>().HasKey(c => new { c.VolunteeringTask_ID, c.Users_ID });
            modelBuilder.Entity<SelectedExpertise>().ToTable("SelectedExpertise");
            modelBuilder.Entity<SelectedExpertise>().HasKey(c => new { c.Expertise_ID, c.Users_ID });

            modelBuilder.Entity<SelectedExpertise>().HasOne(se => se.User).WithMany(e => e.SelectedExperise).HasForeignKey(se => se.Users_ID);
            modelBuilder.Entity<SelectedExpertise>().HasOne(se => se.Expertise).WithMany(s => s.SelectedExpertise).HasForeignKey(se => se.Expertise_ID);
        }
    }
}
