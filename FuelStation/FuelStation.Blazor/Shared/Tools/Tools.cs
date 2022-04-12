using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.Tools
{
    public  class Tools
    {
        public string GenerateCardNumber()
        {
            Random rnd = new Random();
            int number = 0;
            string cardNumber = "A";
            for(int i = 0; i < 3; i++)
            {
                number = rnd.Next(1000, 10000);
                cardNumber += number.ToString();
            } 

            return cardNumber;
        }
    }
}
