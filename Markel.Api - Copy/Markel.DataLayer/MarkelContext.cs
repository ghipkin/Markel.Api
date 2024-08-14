using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Markel.DataLayer;

public partial class MarkelContext : DbContext
{
    public MarkelContext()
    {
    }

    public MarkelContext(DbContextOptions<MarkelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Claim> Claims { get; set; }

    public virtual DbSet<ClaimType> ClaimTypes { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-NCNJA1M;Database=Markel;Trusted_Connection=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(e => e.Ucr).HasName("PK__Claims__C5B186D71AAC4DCD");

            entity.Property(e => e.Ucr)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("UCR");
            entity.Property(e => e.AssuredName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Assured Name");
            entity.Property(e => e.ClaimDate).HasColumnType("datetime");
            entity.Property(e => e.IncurredLoss)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("Incurred Loss");
            entity.Property(e => e.LossDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ClaimType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClaimTyp__3214EC077AF4461D");

            entity.ToTable("ClaimType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Company__3214EC07FA138A6E");

            entity.ToTable("Company");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address1)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Address2)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Address3)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InsuranceEndDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Postcode)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
