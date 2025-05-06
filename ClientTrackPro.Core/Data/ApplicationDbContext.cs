using System;
using Microsoft.EntityFrameworkCore;
using ClientTrackPro.Core.Models.Account;
using ClientTrackPro.Core.Models.Brand;
using ClientTrackPro.Core.Models.Platform;

namespace ClientTrackPro.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<SubscriptionGiftCode> SubscriptionGiftCodes { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        public DbSet<BrandAsset> BrandAssets { get; set; }
        public DbSet<PlatformConnection> PlatformConnections { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.VerificationCode).HasMaxLength(6);
                entity.Property(e => e.SubscriptionTier).HasConversion<string>();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // AdminUser configuration
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Role).IsRequired();
                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<AdminUser>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SubscriptionGiftCode configuration
            modelBuilder.Entity<SubscriptionGiftCode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SubscriptionTier).IsRequired();
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasOne(e => e.UsedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.UsedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.CreatedByAdmin)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByAdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PaymentRecord configuration
            modelBuilder.Entity<PaymentRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PaymentId).IsRequired();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.TransactionType).IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BrandAsset configuration
            modelBuilder.Entity<BrandAsset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.FileUrl).IsRequired();
                entity.Property(e => e.ThumbnailUrl);
                entity.Property(e => e.Type).HasConversion<string>();
                entity.Property(e => e.PlatformSyncStatus);
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PlatformConnection configuration
            modelBuilder.Entity<PlatformConnection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Platform).HasConversion<string>();
                entity.Property(e => e.PlatformUserId).IsRequired();
                entity.Property(e => e.PlatformUsername).IsRequired();
                entity.Property(e => e.AccessToken).IsRequired();
                entity.Property(e => e.RefreshToken);
                entity.Property(e => e.SyncStatus);
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 