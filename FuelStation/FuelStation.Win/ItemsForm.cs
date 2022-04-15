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

namespace FuelStation.Win
{
    public partial class ItemsForm : Form
    {
        private HttpClient _client;
        private List<ItemViewModel> _items;
        private ItemViewModel _currentItem;
        private BindingSource bsItems = new();
        public ItemsForm()
        {
            InitializeComponent();
            _client = Program.serviceProvider.GetRequiredService<HttpClient>();
        }

        private async void ItemsForm_Load(object sender, EventArgs e)
        {
            this.layoutControl1.Controls.Remove(this.btnUndo);

            _items = await GetActiveItemsAsync();
            SetBindings();
            SetView();
        }

        private void SetView()
        {
            grdViewItems.Columns["Id"].Visible = false;
        }

        public void SetBindings()
        {
            bsItems.DataSource = _items;
            cmbType.DataSource = Enum.GetValues(typeof(ItemType));
            cmbType.DataBindings.Add(new Binding("SelectedValue", bsItems, "ItemType", true, DataSourceUpdateMode.OnPropertyChanged));

            
            grdItems.DataSource = bsItems;

            txtCode.DataBindings.Add(new Binding("EditValue", bsItems, "Code", true));
            txtDescription.DataBindings.Add(new Binding("EditValue", bsItems, "Description", true));
            spinCost.DataBindings.Add(new Binding("EditValue", bsItems, "Cost", true));
            spinPrice.DataBindings.Add(new Binding("EditValue", bsItems, "Price", true));
            
            
            //cmbType.DataBindings.Add(new Binding("ValueMember", bsItems, "ItemType", true, DataSourceUpdateMode.OnPropertyChanged));
        }

        public async Task<List<ItemViewModel>> GetActiveItemsAsync()
        {
            return await _client.GetFromJsonAsync<List<ItemViewModel>>(Program.baseURL + "/item/active");
        }

        public async Task<List<ItemViewModel>> GetInactiveItemsAsync()
        {
            return await _client.GetFromJsonAsync<List<ItemViewModel>>(Program.baseURL + "/item/inactive");
        }

        private async void btnNew_Click(object sender, EventArgs e)
        {
            NewItemF form = new();
            form.ShowDialog();
            _items = await GetActiveItemsAsync();
            bsItems.DataSource = _items;
            grdItems.RefreshDataSource();
            grdViewItems.RefreshData();
        }

        private void grdViewItems_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (cmbType.SelectedIndex == -1 && _items.Count() > 0)
            {
                _currentItem = _items[0];
                cmbType.SelectedIndex = (int)_currentItem.ItemType;
            }
            else
            {
                _currentItem = grdViewItems.GetFocusedRow() as ItemViewModel;
                if (_currentItem is null) return;
                cmbType.SelectedIndex = (int)_currentItem.ItemType;
            }

        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            List<string> invalidItems = new();
            foreach (var item in _items)
            {
                var response = await _client.PutAsJsonAsync(Program.baseURL + "/item", item);
                if (!response.IsSuccessStatusCode)
                {
                    invalidItems.Add($"{item.Code} : {item.Description}");
                }
            }

            if (invalidItems.Count() > 0)
            {
                MessageBox.Show("These items could not be saved because of invalid input: \n" + string.Join("\n", invalidItems));
                bsItems.DataSource = await GetActiveItemsAsync();
                grdItems.RefreshDataSource();
            }

            MessageBox.Show("Save Completed!");
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_items is null || _items.Count() == 0) return;
            var currentItem = grdViewItems.GetFocusedRow() as ItemViewModel;
            var response = await _client.DeleteAsync(Program.baseURL + $"/item/{currentItem.Id}");
            if (response.IsSuccessStatusCode)
            {

                _items = await GetActiveItemsAsync();
                bsItems.DataSource = _items;
                grdItems.RefreshDataSource();
                grdViewItems.RefreshData();
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

            _items = await GetInactiveItemsAsync();
            bsItems.DataSource = _items;
            bsItems.ResetBindings(true);
            grdItems.RefreshDataSource();
            grdViewItems.RefreshData();
            lblItemsGrid.Text = "Deleted Customers";
        }
        private async void btnActiveList_Click(object sender, EventArgs e)
        {
            DestroyUndoButton();
            TextReadonly(false);
            ButtonsEnabled(true);

            _items = await GetActiveItemsAsync();
            bsItems.DataSource = _items;
            bsItems.ResetBindings(true);
            grdItems.RefreshDataSource();
            grdViewItems.RefreshData();
            lblItemsGrid.Text = "Active Customers";
        }

        private void DestroyUndoButton()
        {
            this.layoutControl1.Controls.Remove(this.btnUndo);
        }

        private void CreateUndoButton()
        {
            this.layoutControl1.Controls.Add(this.btnUndo);
        }

        private void ButtonsEnabled(bool isenabled)
        {
            btnNew.Enabled = isenabled;
            btnDelete.Enabled = isenabled;
            btnSave.Enabled = isenabled;
        }

        private void TextReadonly(bool isreadonly)
        {
            txtCode.ReadOnly = isreadonly;
            txtDescription.ReadOnly = isreadonly;
            cmbType.Enabled = !isreadonly;
            spinCost.ReadOnly = isreadonly;
            spinPrice.ReadOnly = isreadonly;
        }


        private async void btnUndo_Click(object sender, EventArgs e)
        {
            if (_items.Count == 0) return;
            var item = grdViewItems.GetFocusedRow() as ItemViewModel;

            if (item == null) return;
            var response = await _client.PutAsJsonAsync(Program.baseURL + $"/item/undo/{item.Id}", "");

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Something went wrong!");
                return;
            }

            bsItems.Remove(item);
            grdItems.RefreshDataSource();
            grdViewItems.RefreshData();

        }
    }
}
