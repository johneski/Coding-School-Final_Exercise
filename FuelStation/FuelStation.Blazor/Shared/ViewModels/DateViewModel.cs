using FuelStation.Blazor.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.ViewModels
{
    public class DateViewModel
    {
        [Required]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage="Incorrect Year format")]
        public string Year { get; set; }
        [Required]
        public Month Month { get; set; }
    }
}
