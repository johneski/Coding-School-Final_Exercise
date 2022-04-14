using FuelStation.Blazor.Shared.Enums;
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
using System.Text.RegularExpressions;

namespace FuelStation.Win
{
    public partial class TransactionEditF : Form
    {
        private TransactionViewModel _transaction = new();
        private HttpClient _client;
        private EmployeeViewModel _employee;
        private CustomerViewModel _customer;
        private List<ItemViewModel> _items;
        private BindingSource _bsTransaction = new ();
        private BindingSource _bsEmployee = new ();
        private BindingSource _bsItems = new ();
        private BindingSource _bsTransactionLines = new();

        public TransactionEditF()
        {
            InitializeComponent();
            _client = Program.serviceProvider.GetRequiredService<HttpClient>();
        }

        private async void TransactionEditF_Load(object sender, EventArgs e)
        {
            _employee = await _client.GetFromJsonAsync<EmployeeViewModel>(Program.baseURL + "/employee/current");
            _items = await _client.GetFromJsonAsync<List<ItemViewModel>>(Program.baseURL + "/item/active");

            
            SetBindings();
            SetView();
        }

        private void SetBindings()
        {
            _bsItems.DataSource = _items;
            grdItems.DataSource = _bsItems;

            _bsTransaction.DataSource = _transaction;

            _bsTransactionLines.DataSource = _transaction.TransactionLines;
            grdLines.DataSource = _bsTransactionLines;

            txtDate.DataBindings.Add(new Binding("EditValue", _bsTransaction, "Date", true));
            _bsEmployee.DataSource = _employee;
            cmbEmployee.DataSource = _bsTransaction;
            cmbEmployee.DisplayMember = "EmployeeName";
            txtEmployeeId.DataBindings.Add(new Binding("EditValue", _bsTransaction, "EmployeeId", true));
            txtCustomer.DataBindings.Add(new Binding("EditValue", _bsTransaction, "CustomerName", true));
            cmbPaymentMethod.DataSource = Enum.GetValues(typeof(PaymentMethod));
            cmbPaymentMethod.DataBindings.Add(new Binding("SelectedValue", _bsTransaction, "PaymentMethod", true));
        }

        private void SetView()
        {
            grdViewItems.Columns["Id"].Visible = false;
        }

        private async void txtCardNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)13) return;

            string cardNumber = txtCardNumber.Text;
            if(!Regex.IsMatch(cardNumber, @"^A+[0-9]{12}$"))
            {
                MessageBox.Show("Wrong CardNumber Format!");
                return;
            }

            _bsTransaction.DataSource = await _client.GetFromJsonAsync<TransactionViewModel>(Program.baseURL + $"/transaction/newtransaction/{cardNumber}");
        }
    }
}
