using FuelStation.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Configurations
{
    internal class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(item => item.Id);
            builder.Property(item => item.Code).HasMaxLength(50);
            builder.Property(item => item.Description).HasMaxLength(50);
            builder.Property(item => item.Price).HasPrecision(10, 2);
            builder.Property(item => item.Cost).HasPrecision(10, 2);
            builder.HasIndex(item => item.IsActive);

            builder.HasIndex(item => item.Code).IsUnique();
        }
    }
}
