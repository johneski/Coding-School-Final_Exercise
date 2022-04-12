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

        public async Task LoadAuthToken(HttpClient client, ILocalStorageService localStorage)
        {
            var authToken = await localStorage.GetItemAsStringAsync("authToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authToken.Replace("\"", ""));
        }

    }
}
