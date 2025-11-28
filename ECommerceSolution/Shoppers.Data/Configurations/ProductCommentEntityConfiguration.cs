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
    public class ProductCommentEntityConfiguration : IEntityTypeConfiguration<ProductCommentEntity>
    {
        public void Configure(EntityTypeBuilder<ProductCommentEntity> builder)
        {
            builder.ToTable("ProductComments", tb =>
            {
                tb.HasCheckConstraint(
                    "CK_ProductComment_StarCount",
                    "[StarCount] >= 1 AND [StarCount] <= 5"
                );
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.Text)
                   .IsRequired()
                   .HasMaxLength(500)
                   .HasAnnotation("MinLength", 2);

            builder.Property(x => x.StarCount)
                   .IsRequired();

            builder.Property(x => x.IsConfirmed)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasOne(x => x.Product)
                   .WithMany(y => y.Comments)
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
