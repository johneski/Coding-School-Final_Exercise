using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Models
{
    public class UserCredentials
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid? AuthenticationToken { get; set; }
        public bool IsLogged { get; set; } = false;
        public Employee Employee { get; set; }

    }
}
