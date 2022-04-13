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
    public partial class NewCustomerF : Form
    {
        private CustomerViewModel _customer;
        private HttpClient _client;
        public NewCustomerF()
        {
            InitializeComponent();
            _client = Program.serviceProvider.GetRequiredService<HttpClient>();
        }

        private async void NewCustomerF_Load(object sender, EventArgs e)
        {
            _customer = await _client.GetFromJsonAsync<CustomerViewModel>( Program.baseURL + "/customer/newcustomer");
            SetBindings();
        }

        public void SetBindings()
        {
            txtName.DataBindings.Add(new Binding("EditValue", _customer, "Name"));
            txtSurname.DataBindings.Add(new Binding("EditValue", _customer, "Surname"));
            txtCardNumber.DataBindings.Add(new Binding("EditValue", _customer, "CardNumber"));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var response = await _client.PostAsJsonAsync(Program.baseURL + "/customer", _customer);
            if (response is null || !response.IsSuccessStatusCode)
            {
                lblMessage.Text = "Wrong Inputs!";
                return;
            }

            if (!string.IsNullOrEmpty(lblMessage.Text))
                lblMessage.Text = "";
            this.Close();
        }
    }
}
