using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Models
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Fullname { get => $"{Name} {Surname}"; }
        public string CardNumber { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
