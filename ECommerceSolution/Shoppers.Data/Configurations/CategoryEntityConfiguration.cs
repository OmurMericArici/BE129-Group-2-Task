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
    public class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasAnnotation("MinLength", 2);

            builder.Property(x => x.Color)
                   .IsRequired()
                   .HasMaxLength(6)
                   .HasAnnotation("MinLength", 3);

            builder.Property(x => x.IconCssClass)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasAnnotation("MinLength", 2);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();
        }
    }
}
