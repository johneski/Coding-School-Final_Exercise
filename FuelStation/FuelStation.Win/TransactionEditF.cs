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
        private int _fuelItemsInList = 0;

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

            _bsTransactionLines.DataSource = _transaction.TransactionLines ?? new();
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

            grdViewLines.Columns["Id"].Visible = false;
            //grdViewLines.Columns.Append(new DevExpress.XtraGrid.Columns.GridColumn()
            //{
            //    Caption = "Qty",
            //    Visible = true,
            //    UnboundType = DevExpress.Data.UnboundColumnType.Decimal
            //});
            
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

            _transaction = await _client.GetFromJsonAsync<TransactionViewModel>(Program.baseURL + $"/transaction/newtransaction/{cardNumber}");

            if (_transaction.CustomerId == Guid.Empty)
                MessageBox.Show("Customer not found!");

            _bsTransaction.DataSource = _transaction;
        }

        private void btnAddLine_Click(object sender, EventArgs e)
        {
            if (_items.Count == 0) return;

            var item = grdViewItems.GetFocusedRow() as ItemViewModel;

            if (item.ItemType == ItemType.Fuel && _fuelItemsInList > 0)
            {
                MessageBox.Show("There has already been added Fuel item in the list!");
                return;
            }

            if (item.ItemType == ItemType.Fuel) 
                _fuelItemsInList++;

            TransactionLineViewModel line = new()
            {
                ItemName = item.Description,
                ItemPrice = item.Price,
                DiscountPercent = 0,
                DiscountValue = 0,
                NetValue = item.Price,
                Qty = 1,
                TotalValue = item.Price,
            };


            _bsTransactionLines.Add(line);
        }

        private void grdViewLines_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var line = grdViewLines.GetRow(e.RowHandle) as TransactionLineViewModel;

            if (line is null) return;

            line.NetValue = line.Qty * line.ItemPrice;
            line.DiscountValue = (line.DiscountPercent/100) * line.NetValue;
            line.TotalValue = line.NetValue - line.DiscountValue;
        }
    }
}
