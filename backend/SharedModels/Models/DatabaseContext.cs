using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SharedModels.Models;

public partial class DatabaseContext : DbContext
{
    IConfiguration _configuration;
    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("Chat_pkey");

            entity.ToTable("Chat");

            entity.Property(e => e.ChatId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Chats)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Chat_UserId_fkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("Message_pkey");

            entity.ToTable("Message");

            entity.Property(e => e.MessageId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.MessageText).HasColumnName("MessageText");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Message_ChatId_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pkey");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "User_Username_key").IsUnique();

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
