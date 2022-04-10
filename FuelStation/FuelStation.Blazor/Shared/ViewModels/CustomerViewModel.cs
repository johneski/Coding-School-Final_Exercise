using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.ViewModels
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Fullname { get => $"{Name} {Surname}"; }
        public string CardNumber { get; set; }
    }
}
