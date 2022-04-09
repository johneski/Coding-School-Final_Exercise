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
    internal class UserCredentialsConfiguration : IEntityTypeConfiguration<UserCredentials>
    {
        public void Configure(EntityTypeBuilder<UserCredentials> builder)
        {
            builder.HasKey(cred => cred.Id);
            builder.Property(cred => cred.UserName).HasMaxLength(128);
            builder.Property(cred => cred.Password).HasMaxLength(128);

            builder.HasOne(cred => cred.Employee).WithOne(x => x.Credentials).HasForeignKey<UserCredentials>(x => x.EmployeeId);
        }
    }
}
