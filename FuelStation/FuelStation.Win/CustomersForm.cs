using FuelStation.Blazor.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuelStation.Win
{
    public partial class CustomersForm : Form
    {
        private HttpClient _client;        
        private List<CustomerViewModel> _customers;
        private BindingSource bsCuctomers = new();
        public CustomersForm(HttpClient client)
        {
            InitializeComponent();
            _client = client;
        }

        private async void CustomersForm_Load(object sender, EventArgs e)
        {
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Post, Program.baseURL + "/validation"))
            {
                request.Headers.Add("username", "admin");
                request.Headers.Add("password", "123456789");
                response = await _client.SendAsync(request);
                var authorization = await response.Content.ReadAsStringAsync();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorization.Replace("\"", ""));
            }

            _customers = await _client.GetFromJsonAsync<List<CustomerViewModel>>(Program.baseURL + "/customer/active");
            SetBindings();
            SetView();
        }

        private void SetBindings()
        {
            
            bsCuctomers.DataSource = _customers;

            grdCustomers.DataSource = bsCuctomers;
            txtName.DataBindings.Add(new Binding("EditValue", grdCustomers.DataSource, "Name"));
            txtSurname.DataBindings.Add(new Binding("EditValue", grdCustomers.DataSource, "Surname"));
            txtCardNumber.DataBindings.Add(new Binding("EditValue", grdCustomers.DataSource, "CardNumber"));
        }

        private void SetView()
        {
            grdViewCustomers.Columns["Id"].Visible = false;
            //grdViewCustomers.Columns["Id"].Visible = false;
        }

        private async void btnNew_Click(object sender, EventArgs e)
        {
            NewCustomerF form = new();
            form.ShowDialog();
            _customers = await _client.GetFromJsonAsync<List<CustomerViewModel>>(Program.baseURL + "/customer/active");
            bsCuctomers.DataSource = _customers;
            grdCustomers.RefreshDataSource();
            grdViewCustomers.RefreshData();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            List<string> invalidCustomers = new();
            foreach(var customer in _customers)
            {
                var response = await _client.PutAsJsonAsync(Program.baseURL + "/customer", customer);
                if (!response.IsSuccessStatusCode)
                {
                    invalidCustomers.Add(customer.Fullname);
                }
            }

            if (invalidCustomers.Count() > 0)
            {
                MessageBox.Show("These customers could not be saved because of invalid input: \n" + string.Join("\n", invalidCustomers));
                grdCustomers.DataSource = await _client.GetFromJsonAsync<List<CustomerViewModel>>(Program.baseURL + "/customer/active"); ;
            }

            MessageBox.Show("Save Completed!");

        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_customers is null || _customers.Count() == 0) return;
            var currentCustomer = grdViewCustomers.GetFocusedRow() as CustomerViewModel;
            var response = await _client.DeleteAsync(Program.baseURL + $"/customer/{currentCustomer.Id}");
            if (response.IsSuccessStatusCode)
            {
                
                _customers = await _client.GetFromJsonAsync<List<CustomerViewModel>>(Program.baseURL + "/customer/active");
                bsCuctomers.DataSource = _customers;
                //grdViewCustomers.SetFocusedRowModified();
                grdCustomers.RefreshDataSource();
                grdViewCustomers.RefreshData();
                MessageBox.Show("Complete!");
                return;
            }

            MessageBox.Show("Something went wrong!");
               
        }
    }
}
