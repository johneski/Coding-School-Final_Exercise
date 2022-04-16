using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
