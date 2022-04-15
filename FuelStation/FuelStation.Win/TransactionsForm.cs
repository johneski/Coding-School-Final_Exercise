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
    public partial class TransactionsForm : Form
    {
        private HttpClient _client;
        private List<TransactionViewModel> _transactions;
        private BindingSource _bsTransactions = new();
        public TransactionsForm()
        {
            InitializeComponent();
            _client = Program.serviceProvider.GetRequiredService<HttpClient>();
        }

        private async void TransactionsForm_Load(object sender, EventArgs e)
        {
            _transactions = await _client.GetFromJsonAsync<List<TransactionViewModel>>(Program.baseURL + "/transaction/active");
            _transactions ??= new();
            SetBindings();
            SetView();
        }

        private void SetView()
        {
            grdViewTransactions.Columns["Id"].Visible = false;
            grdViewTransactions.Columns["EmployeeId"].Visible = false;
            grdViewTransactions.Columns["CustomerId"].Visible = false;

        }

        private void SetBindings()
        {
            _bsTransactions.DataSource = _transactions;
            grdTransactions.DataSource = _bsTransactions;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnNew_Click(object sender, EventArgs e)
        {
            TransactionEditF form = new();
            form.ShowDialog();
            _transactions = await _client.GetFromJsonAsync<List<TransactionViewModel>>(Program.baseURL + "/transaction/active");
            _transactions ??= new();
            _bsTransactions.DataSource = _transactions;
            _bsTransactions.ResetBindings(true);
            grdViewTransactions.RefreshData();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (grdViewTransactions.RowCount == 0) return;

            var transaction = grdViewTransactions.GetFocusedRow() as TransactionViewModel;
            await _client.DeleteAsync(Program.baseURL + $"/transaction/{transaction.Id}");

            _bsTransactions.Remove(transaction);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if(grdViewTransactions.RowCount == 0) return;

            var transaction = grdViewTransactions.GetFocusedRow() as TransactionViewModel;
            if (transaction is null) return;
            TransactionEditF form = new(transaction.Id);
            form.ShowDialog();
        }
    }
}
