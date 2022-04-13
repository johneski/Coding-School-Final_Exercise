using FuelStation.Blazor.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.ViewModels
{
    public class ItemViewModel
    {
        public Guid Id { get; set; }
        [Required]
        [RegularExpression("^[0-9]{5,10}$")]
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public ItemType ItemType { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal Cost { get; set; }
    }
}
