using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using KinderChatServer.Migrations;
using KinderChatServer.Models;

namespace KinderChatServer.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserDevice> UserDevices { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Avatar> Avatars { get; set; }

        public DbSet<Achievement> Achievements { get; set; }

        public DbSet<AchievementsRecieved> AchievementsRecieveds { get; set; }

        public DbSet<Flag> Flags { get; set; }

        public DbSet<UserFlag> UserFlags { get; set; }

        public DbSet<Friend> Friends { get; set; }

        public DbSet<Block> Blocked { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(c => c.Devices);
            modelBuilder.Entity<User>().HasMany(c => c.Achievements);
            modelBuilder.Entity<User>().HasMany(c => c.UserFlags);
            modelBuilder.Entity<UserFlag>().HasKey<int>(l => l.Id);
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new {r.RoleId, r.UserId});
            base.OnModelCreating(modelBuilder);
        }
    }
}
