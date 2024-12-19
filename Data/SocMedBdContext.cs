using System;
using System.Collections.Generic;
using APISocMed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace APISocMed.Data;

public partial class SocMedBdContext: DbContext
{

    private readonly IConfiguration _configuration;
    public SocMedBdContext(DbContextOptions<SocMedBdContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }
    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Follower> Followers { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ConnectionSQL"));
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__DD0C739A08A934E6");

            entity.Property(e => e.PostId).HasColumnName("postId");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Posts__userId__440B1D61");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__CB9A1CFFEA5129DE");

            entity.HasIndex(e => e.UserName, "UQ__Users__66DCF95CCB4DF8BB").IsUnique();

            entity.HasIndex(e => e.UserEmail, "UQ__Users__D54ADF55BB7EA918").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .HasColumnName("password_hash");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("userEmail");
            entity.Property(e => e.UserName)
                .HasMaxLength(15)
                .HasColumnName("userName");
        });

        modelBuilder.Entity<Follower>()
            .HasKey(f => new { f.FollowerId, f.FollowedId }); 

        modelBuilder.Entity<Follower>()
            .HasOne(f => f.FollowerUser)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Follower>()
            .HasOne(f => f.FollowedUser)
            .WithMany(u => u.FollowedBy)
            .HasForeignKey(f => f.FollowedId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefreshToken>()
           .HasOne(rt => rt.User)
           .WithMany(u => u.RefreshTokens)
           .HasForeignKey(rt => rt.UserId);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
