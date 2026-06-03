using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PennyWise.Models;

namespace PennyWise.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- DATA SEEDING: Varsayılan Kategoriler ---
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Maaş",           Icon = "fa-wallet",               Color = "#10b981" },
                new Category { Id = 2, Name = "Yatırım",        Icon = "fa-chart-line",            Color = "#6366f1" },
                new Category { Id = 3, Name = "Diğer Gelir",    Icon = "fa-coins",                 Color = "#14b8a6" },
                new Category { Id = 4, Name = "Gıda / Market",  Icon = "fa-cart-shopping",         Color = "#f59e0b" },
                new Category { Id = 5, Name = "Kira / Ev",      Icon = "fa-house",                 Color = "#3b82f6" },
                new Category { Id = 6, Name = "Faturalar",      Icon = "fa-file-invoice-dollar",   Color = "#ef4444" },
                new Category { Id = 7, Name = "Ulaşım",         Icon = "fa-bus",                   Color = "#8b5cf6" },
                new Category { Id = 8, Name = "Eğlence",        Icon = "fa-gamepad",               Color = "#ec4899" },
                new Category { Id = 9, Name = "Sağlık",         Icon = "fa-heart-pulse",           Color = "#f43f5e" },
                new Category { Id = 10, Name = "Diğer Gider",   Icon = "fa-asterisk",              Color = "#64748b" }
            );

            // --- ROLLER: Admin / User seeding ---
            var adminRoleId = "role-admin-001";
            var userRoleId  = "role-user-001";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "stamp-admin-001" },
                new IdentityRole { Id = userRoleId,  Name = "User",  NormalizedName = "USER",  ConcurrencyStamp = "stamp-user-001"  }
            );
        }
    }
}
