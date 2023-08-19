using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace List.Models
{
    public partial class INTERNContext : DbContext
    {
        public INTERNContext()
        {
        }

        public INTERNContext(DbContextOptions<INTERNContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SdDatabase> SdDatabases { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("CONNECTIONSTRING");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SdDatabase>(entity =>
            {
                entity.HasKey(e => e.SicilNo);

                entity.ToTable("SD_DATABASE");

                entity.Property(e => e.SicilNo)
                    .ValueGeneratedNever()
                    .HasColumnName("SICIL_NO");

                entity.Property(e => e.Ad)
                    .HasMaxLength(50)
                    .HasColumnName("AD");

                entity.Property(e => e.Bolum)
                    .HasMaxLength(50)
                    .HasColumnName("BOLUM");

                entity.Property(e => e.DagitimId)
                    .HasMaxLength(50)
                    .HasColumnName("DAGITIM_ID");

                entity.Property(e => e.EklendigiTarih)
                    .HasColumnType("date")
                    .HasColumnName("EKLENDIGI_TARIH");

                entity.Property(e => e.EkleyenKisi)
                    .HasMaxLength(50)
                    .HasColumnName("EKLEYEN_KISI");

                entity.Property(e => e.Flos).HasColumnName("FLOS");

                entity.Property(e => e.Soyad)
                    .HasMaxLength(50)
                    .HasColumnName("SOYAD");

                entity.Property(e => e.VerilisTarih)
                    .HasColumnType("date")
                    .HasColumnName("VERILIS_TARIH");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
