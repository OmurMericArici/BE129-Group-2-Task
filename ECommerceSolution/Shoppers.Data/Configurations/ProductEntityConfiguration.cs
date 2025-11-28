using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoppers.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppers.Data.Configurations
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasAnnotation("MinLength", 2);

            builder.Property(x => x.Price)
                   .IsRequired()
                   .HasColumnType("money"); // currency

            builder.Property(x => x.Details)
                   .HasMaxLength(1000);

            builder.Property(x => x.StockAmount)
                   .IsRequired();

            builder.Property(x => x.Enabled)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasOne(x => x.Category)
                   .WithMany(y => y.Products)
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Seller)
                   .WithMany()
                   .HasForeignKey(x => x.SellerId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
