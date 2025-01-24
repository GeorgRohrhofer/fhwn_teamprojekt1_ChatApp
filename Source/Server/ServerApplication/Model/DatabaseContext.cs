using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Configuration;

namespace ServerApplication.Model
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Privilege> privileges { get; set; }
        public DbSet<UserGroup> usergroups { get; set; }
        public DbSet<UserGroupMember> usergroupmembers { get; set; }
        public DbSet<Message> messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the composite primary key for UserGroupMembers
            modelBuilder.Entity<UserGroupMember>()
                .HasKey(ugm => new { ugm.group_id, ugm.user_id });

            // Configure relationships



            //UPDATED REALATIONSHIP: NEEDS CHANGES IN CODE!
            // UserGroup <-> User
            modelBuilder.Entity<User>()
                .HasMany(g => g.groups)
                .WithMany(u => u.users)
                .UsingEntity<UserGroupMember>();




            // UserGroupMember -> UserGroup
            //modelBuilder.Entity<UserGroupMember>()
            //    .HasOne(ugm => ugm.group)
            //    .WithMany(g => g.usergroupmembers)
            //    .HasForeignKey(ugm => ugm.group_id);

            //// UserGroup -> UserGroupMember
            //modelBuilder.Entity<UserGroup>()
            //    .HasMany(g => g.usergroupmembers)
            //    .WithOne(ugm => ugm.group)
            //    .HasForeignKey(ugm => ugm.group_id);

            //// UserGroupMember -> User
            //modelBuilder.Entity<UserGroupMember>()
            //    .HasOne(ugm => ugm.user)
            //    .WithMany(u => u.usergroupmembers)
            //    .HasForeignKey(ugm => ugm.user_id);

            //// User -> UserGroupMember
            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.usergroupmembers)
            //    .WithOne(ugm => ugm.user)
            //    .HasForeignKey(ugm => ugm.user_id);

            // Message -> User
            modelBuilder.Entity<Message>()
                .HasOne(m => m.user)
                .WithMany(u => u.messages)
                .HasForeignKey(m => m.user_id);

            // User -> Message
            modelBuilder.Entity<User>()
                .HasMany(u => u.messages)
                .WithOne(m => m.user)
                .HasForeignKey(m => m.user_id);

            // Message -> UserGroup
            modelBuilder.Entity<Message>()
                .HasOne(m => m.group)
                .WithMany(g => g.messages)
                .HasForeignKey(m => m.group_id);

            // UserGroup -> Message
            modelBuilder.Entity<UserGroup>()
                .HasMany(g => g.messages)
                .WithOne(m => m.group)
                .HasForeignKey(m => m.group_id);



            // configure user name to be unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.username)
                .IsUnique();

            // configure email to be unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string host = ConfigurationManager.AppSettings.Get("DbHost");
            string port = ConfigurationManager.AppSettings.Get("DbPort");
            string database = ConfigurationManager.AppSettings.Get("DbName");
            string username = ConfigurationManager.AppSettings.Get("DbUsername");
            string password = ConfigurationManager.AppSettings.Get("DbPassword");

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(database) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("host: " + host);
                Console.WriteLine("port: " + port);
                Console.WriteLine("database: " + database);
                Console.WriteLine("username: " + username);
                Console.WriteLine("password: " + password);
                throw new ConfigurationErrorsException("Database configuration is missing or incomplete.");
            }

            string connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
            optionsBuilder.UseNpgsql(connectionString);
        }

        
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
            string path = Environment.GetFolderPath(folder);
            string Dbpath = $"{path}{Path.DirectorySeparatorChar}ServerApplication.db";
            optionsBuilder.UseNpgsql($"Host=postgres;Port=5432;Database=postgres;Username=postgres;Password=secret");

        }*/
    }
}
