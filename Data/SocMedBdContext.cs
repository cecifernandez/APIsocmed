﻿using System;
using System.Collections.Generic;
using APISocMed.Models;
using Microsoft.EntityFrameworkCore;

namespace APISocMed.Data;

public partial class SocMedBdContext : DbContext
{
    public SocMedBdContext()
    {
    }

    public SocMedBdContext(DbContextOptions<SocMedBdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }


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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}