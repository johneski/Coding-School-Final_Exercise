using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;

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

        public string GenerateCode()
        {
            Random rnd = new Random();
            int number = 0;
            int length = rnd.Next(5, 11);
            string code = "";
            for (int i = 0; i < length; i++)
            {
                number = rnd.Next(0, 10);
                code += number.ToString();
            }

            return code;
        }

        public async Task LoadAuthToken(HttpClient client, ILocalStorageService localStorage)
        {
            var authToken = await localStorage.GetItemAsStringAsync("authToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authToken.Replace("\"", ""));
        }

        public async Task<bool> WasWorking(DateTime start, DateTime? end, DateTime current)
        {
            if(end is null)
                end = DateTime.Now;

            if (start <= current && end >= current)
                return true;

            return false;
        }

    }
}
