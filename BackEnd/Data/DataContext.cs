﻿using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        // create Table AppUser
        public DbSet<AppUser> Users { get; set; }

        // create tabele UserLike Which Has many to many relation with(AppUser, AppUser)
        public DbSet<UserLike> likes { get; set; }

        public DbSet<Message> messages { get; set; }    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLike>()
                        .HasKey(k => new {k.LkedUserID, k.SourceUserId});

            modelBuilder.Entity<UserLike>()
                        .HasOne(e => e.SourceUser)
                        .WithMany(e => e.LikedUsers)
                        .HasForeignKey(e => e.SourceUserId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserLike>()
                        .HasOne(e => e.LkedUser)
                        .WithMany(e => e.LikesdByUsers)
                        .HasForeignKey(e => e.LkedUserID)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                        .HasOne(m => m.Sender)
                        .WithMany(l => l.MessageSent)
                        .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Message>()
                        .HasOne(m => m.Recipient)
                        .WithMany(l => l.MessageRecevied)
                        .OnDelete(DeleteBehavior.Restrict);
        }
        
    }
}
