using FuelStation.Blazor.Shared.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuelStation.Win
{
    public partial class RentForm : Form
    {
        private readonly HttpClient client;
        private RentViewModel rent = new() { Date=DateTime.Now};
        public RentForm()
        {
            InitializeComponent();
            client = Program.serviceProvider.GetRequiredService<HttpClient>();
        }

        private void RentForm_Load(object sender, EventArgs e)
        {
            spinRent.DataBindings.Add(new Binding("EditValue", rent, "Value", true));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var response = await client.PostAsJsonAsync(Program.baseURL + "/transaction/rent", rent);
            if (response.IsSuccessStatusCode)
            {
                this.Close();
                return;
            }

            MessageBox.Show("Something went wrong!");

        }

        
    }
}
