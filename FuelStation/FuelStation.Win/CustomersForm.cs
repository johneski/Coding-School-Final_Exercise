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
            DestroyUndoButton();

            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Post, Program.baseURL + "/validation"))
            {
                request.Headers.Add("username", "admin");
                request.Headers.Add("password", "123456789");
                response = await _client.SendAsync(request);
                var authorization = await response.Content.ReadAsStringAsync();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorization.Replace("\"", ""));
            }

            _customers = await GetActiveCustomersAsync();
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
            _customers = await GetActiveCustomersAsync();
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
                grdCustomers.DataSource = await GetActiveCustomersAsync();
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

                _customers = await GetActiveCustomersAsync();
                bsCuctomers.DataSource = _customers;
                //grdViewCustomers.SetFocusedRowModified();
                grdCustomers.RefreshDataSource();
                grdViewCustomers.RefreshData();
                MessageBox.Show("Complete!");
                return;
            }

            MessageBox.Show("Something went wrong!");
               
        }

        private async void btnDeletedList_Click(object sender, EventArgs e)
        {
            CreateUndoButton();
            TextReadonly(true);
            ButtonsEnabled(false);

            _customers = await GetInactiveCustomersAsync();
            bsCuctomers.DataSource = _customers;
            bsCuctomers.ResetBindings(true);
            grdCustomers.RefreshDataSource();
            grdViewCustomers.RefreshData();
            lblCustomers.Text = "Deleted Customers";
        }

        private async void btnActiveList_Click(object sender, EventArgs e)
        {
            DestroyUndoButton();
            TextReadonly(false);
            ButtonsEnabled(true);

            _customers = await GetActiveCustomersAsync();
            bsCuctomers.DataSource = _customers;
            bsCuctomers.ResetBindings(true);
            grdCustomers.RefreshDataSource();
            grdViewCustomers.RefreshData();
            lblCustomers.Text = "Active Customers";
        }

        public async Task<List<CustomerViewModel>> GetActiveCustomersAsync()
        {
            return await _client.GetFromJsonAsync<List<CustomerViewModel>>(Program.baseURL + "/customer/active");
        }

        public async Task<List<CustomerViewModel>> GetInactiveCustomersAsync()
        {
            return await _client.GetFromJsonAsync<List<CustomerViewModel>>(Program.baseURL + "/customer/inactive");
        }

        public void ButtonsEnabled(bool isenabled)
        {
            btnNew.Enabled = isenabled;
            btnDelete.Enabled = isenabled;
            btnSave.Enabled = isenabled;
        }

        public void TextReadonly(bool iseditable)
        {
            txtName.ReadOnly = iseditable;
            txtSurname.ReadOnly = iseditable;
        }

        public void CreateUndoButton()
        {
            this.layoutControl1.Controls.Add(this.btnUndo);
        }

        public void DestroyUndoButton()
        {
            this.layoutControl1.Controls.Remove(this.btnUndo);
        }

        private async void btnUndo_Click(object sender, EventArgs e)
        {
            if (_customers.Count == 0) return;
            var customer = grdViewCustomers.GetFocusedRow() as CustomerViewModel;
            if (customer is null) return;
            await _client.PutAsJsonAsync(Program.baseURL + $"/customer/undo/{customer.Id}", "");
            bsCuctomers.Remove(customer);
            grdCustomers.RefreshDataSource();
            grdViewCustomers.RefreshData();
        }
    }
}
