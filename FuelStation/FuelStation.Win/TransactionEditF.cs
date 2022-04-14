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
        private TransactionViewModel _transaction = new() { TransactionLines = new()};
        private HttpClient _client;
        private EmployeeViewModel _employee;
        private CustomerViewModel _customer;
        private List<ItemViewModel> _items;
        private BindingSource _bsTransaction = new ();
        private BindingSource _bsEmployee = new ();
        private BindingSource _bsItems = new ();
        private BindingSource _bsTransactionLines = new();
        private int _fuelItemsInList = 0;
        public Guid? _transactionEditId;

        public TransactionEditF()
        {
            InitializeComponent();
            _client = Program.serviceProvider.GetRequiredService<HttpClient>();
        }

        private async void TransactionEditF_Load(object sender, EventArgs e)
        {
            _employee = await _client.GetFromJsonAsync<EmployeeViewModel>(Program.baseURL + "/employee/current");
            _items = await _client.GetFromJsonAsync<List<ItemViewModel>>(Program.baseURL + "/item/active");

            
            if (_transactionEditId is not null || _transactionEditId != Guid.Empty)
            {
                var transactionToEdit = await _client.GetFromJsonAsync<TransactionViewModel>(Program.baseURL + $"/transaction/active/{_transactionEditId}");
                if (transactionToEdit is not null)
                    _transaction = transactionToEdit;
            }

            
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
            txtCustomer.DataBindings.Add(new Binding("EditValue", _bsTransaction, "CustomerName", true));
            cmbPaymentMethod.DataSource = Enum.GetValues(typeof(PaymentMethod));
            cmbPaymentMethod.DataBindings.Add(new Binding("SelectedValue", _bsTransaction, "PaymentMethod", true));
            txtTotal.DataBindings.Add(new Binding("EditValue", _bsTransaction , "Total", true));
        }

        private void SetView()
        {
            grdViewItems.Columns["Id"].Visible = false;

            grdViewLines.Columns["Id"].Visible = false;
            grdViewLines.Columns["ItemName"].OptionsColumn.AllowEdit = false;
            grdViewLines.Columns["ItemPrice"].OptionsColumn.AllowEdit = false;
            grdViewLines.Columns["NetValue"].OptionsColumn.AllowEdit = false;
            grdViewLines.Columns["DiscountValue"].OptionsColumn.AllowEdit = false;
            grdViewLines.Columns["TotalValue"].OptionsColumn.AllowEdit = false;
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

            var newTransaction = await _client.GetFromJsonAsync<TransactionViewModel>(Program.baseURL + $"/transaction/newtransaction/{cardNumber}");

            CopyTransaction(_transaction, newTransaction);

            if (_transaction.CustomerId == Guid.Empty)
                MessageBox.Show("Customer not found!");

            _bsTransaction.DataSource = _transaction;
            _bsTransaction.ResetBindings(true);
        }

        private void CopyTransaction(TransactionViewModel to, TransactionViewModel from )
        {
            to.Date = from.Date;
            to.CustomerName = from.CustomerName;
            to.CustomerId = from.CustomerId;
            to.EmployeeId = from.EmployeeId;
            to.EmployeeName = from.EmployeeName;
            to.PaymentMethod = from.PaymentMethod;
        }

        private void btnAddLine_Click(object sender, EventArgs e)
        {
            AddLine();
        }

        private void AddLine()
        {
            if (_items.Count == 0) return;

            var item = grdViewItems.GetFocusedRow() as ItemViewModel;

            if (item.ItemType == ItemType.Fuel && _fuelItemsInList > 0)
            {
                MessageBox.Show("There has already been added Fuel item in the list!");
                return;
            }

            TransactionLineViewModel line = new()
            {
                ItemId = item.Id,
                ItemName = item.Description,
                ItemPrice = item.Price,
                DiscountPercent = 0,
                DiscountValue = 0,
                NetValue = item.Price,
                Qty = 1,
                TotalValue = item.Price,
                ItemType = item.ItemType,
            };


            _bsTransactionLines.Add(line);

            CheckDiscount(line);
            
            line.DiscountValue = (line.DiscountPercent/100) * line.NetValue;
            line.TotalValue = line.NetValue - line.DiscountValue;
                
            if(line.ItemType == ItemType.Fuel)
                _fuelItemsInList++;
            

            CalculateTotal();

            
        }

        private void grdViewLines_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var line = grdViewLines.GetRow(e.RowHandle) as TransactionLineViewModel;

            if (line is null) return;

            CheckDiscount(line);

            line.NetValue = line.Qty * line.ItemPrice;
            line.DiscountValue = (line.DiscountPercent/100) * line.NetValue;
            line.TotalValue = line.NetValue - line.DiscountValue;

            CalculateTotal();
        }



        private void CheckDiscount(TransactionLineViewModel line)
        {
            if (line.ItemType == ItemType.Fuel
                && line.NetValue > 20
                && line.ItemPrice * line.Qty <= 20)
            {
                line.DiscountPercent -= 10m;
                return;
            }

            if (line.ItemType == ItemType.Fuel 
                && line.NetValue <= 20 
                && line.ItemPrice * line.Qty > 20)
            {
                line.DiscountPercent += 10m;
                return;
            }

            if (line.ItemType == ItemType.Fuel
                && line.NetValue > 20
                && line.ItemPrice * line.Qty == line.NetValue)
            {
                line.DiscountPercent += 10m;
                return;
            }
        }


        private void CalculateTotal()
        {
            _transaction.Total = 0;
            foreach(var line in _transaction.TransactionLines)
            {
                _transaction.Total += line.TotalValue;
            }

            txtTotal.Text = _transaction.Total.ToString();
        }

        private void ApplyDiscount(decimal discount)
        {
            foreach(TransactionLineViewModel line in _bsTransactionLines)
            {
                line.DiscountPercent += discount;
                line.DiscountValue = line.ItemPrice * line.DiscountPercent;
                line.TotalValue = line.NetValue - line.DiscountValue;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (grdViewLines.RowCount == 0) return;

            var line = grdViewLines.GetFocusedRow() as TransactionLineViewModel;

            if (line.ItemType == ItemType.Fuel)
                _fuelItemsInList--;

            _bsTransactionLines.Remove(line);

            CalculateTotal();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var response = await _client.PostAsJsonAsync(Program.baseURL + "/transaction", _transaction);
        }
    }
}
