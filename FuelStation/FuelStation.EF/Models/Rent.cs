using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Models
{
    public class Rent
    {
        public Guid Id { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
}
