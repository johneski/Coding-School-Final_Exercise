using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.ViewModels
{
    public class CustomerViewModel
    {
        
        public Guid Id { get; set; }
        [Required]
        [RegularExpression("[A-Za-z]{3, 20}", ErrorMessage = "Number and symbols are not allowed.")]
        public string Name { get; set; }
        [Required]
        [RegularExpression("[A-Za-z]{3, 20}", ErrorMessage = "Number and symbols are not allowed.")]
        public string Surname { get; set; }
        public string Fullname { get => $"{Name} {Surname}"; }
        [Required]
        public string CardNumber { get; set; }
    }
}
