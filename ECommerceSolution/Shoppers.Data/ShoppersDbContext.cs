using Microsoft.EntityFrameworkCore;
using Shoppers.Data.Entities;
using Shoppers.Data.Configurations;
using System.Reflection;

namespace Shoppers.Data
{
    public class ShoppersDbContext : DbContext
    {
        public ShoppersDbContext(DbContextOptions<ShoppersDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductImages { get; set; }
        public DbSet<ProductCommentEntity> ProductComments { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<RoleEntity>().HasData(
                new RoleEntity { Id = 1, Name = "buyer", CreatedAt = DateTime.Now },
                new RoleEntity { Id = 2, Name = "seller", CreatedAt = DateTime.Now },
                new RoleEntity { Id = 3, Name = "admin", CreatedAt = DateTime.Now }
            );

            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity
                {
                    Id = 1,
                    Email = "admin@shop.com",
                    FirstName = "System",
                    LastName = "Admin",
                    Password = "123",
                    RoleId = 3,
                    Enabled = true,
                    CreatedAt = DateTime.Now
                },
                new UserEntity
                {
                    Id = 2,
                    Email = "buyer@shop.com",
                    FirstName = "John",
                    LastName = "Buyer",
                    Password = "123",
                    RoleId = 1,
                    Enabled = true,
                    CreatedAt = DateTime.Now
                },
                new UserEntity
                {
                    Id = 3,
                    Email = "seller@shop.com",
                    FirstName = "Jane",
                    LastName = "Seller",
                    Password = "123",
                    RoleId = 2,
                    Enabled = true,
                    CreatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<CategoryEntity>().HasData(
                new CategoryEntity { Id = 1, Name = "Tops", Color = "red", IconCssClass = "icon-shirt", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 2, Name = "Bottoms", Color = "blue", IconCssClass = "icon-trousers", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 3, Name = "Dresses", Color = "pink", IconCssClass = "icon-dress", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 4, Name = "Outerwear", Color = "green", IconCssClass = "icon-jacket", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 5, Name = "Shoes", Color = "orange", IconCssClass = "icon-shoe", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 6, Name = "Accessories", Color = "purple", IconCssClass = "icon-accessories", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 7, Name = "Sportswear", Color = "teal", IconCssClass = "icon-sport", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 8, Name = "Underwear", Color = "brown", IconCssClass = "icon-underwear", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 9, Name = "Kids Wear", Color = "yellow", IconCssClass = "icon-kids", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 10, Name = "Bags", Color = "cyan", IconCssClass = "icon-bag", CreatedAt = DateTime.Now }
            );

            modelBuilder.Entity<ProductEntity>().HasData(
                new ProductEntity
                {
                    Id = 1,
                    Name = "Test T-Shirt",
                    Price = 49.99m,
                    StockAmount = 100,
                    CategoryId = 1,
                    SellerId = 1,
                    Enabled = true,
                    CreatedAt = DateTime.Now,
                    Details = "A great test product for development purposes."
                }
            );
        }
    }
}