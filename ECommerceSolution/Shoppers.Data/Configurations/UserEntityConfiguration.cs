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
    public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.FirstName)
                   .IsRequired()
                   .HasAnnotation("MinLength", 2)
                   .HasMaxLength(50);

            builder.Property(x => x.LastName)
                   .IsRequired()
                   .HasAnnotation("MinLength", 2)
                   .HasMaxLength(50);


            builder.Property(x => x.Password)
                .HasAnnotation("MinLength", 1)
                   .IsRequired();

            builder.Property(x => x.Enabled)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasOne(x => x.Role)
                   .WithMany(y => y.Users)
                   .HasForeignKey(x => x.RoleId)
                   .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
