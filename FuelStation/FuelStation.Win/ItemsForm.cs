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

            _items = await GetActiveItems();
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
            grdItems.DataSource = bsItems;

            txtCode.DataBindings.Add(new Binding("EditValue", bsItems, "Code", true));
            txtDescription.DataBindings.Add(new Binding("EditValue", bsItems, "Description", true));
            spinCost.DataBindings.Add(new Binding("EditValue", bsItems, "Cost", true));
            spinPrice.DataBindings.Add(new Binding("EditValue", bsItems, "Price", true));
            
            cmbType.DataSource = Enum.GetValues(typeof(ItemType));
            cmbType.DataBindings.Add(new Binding("SelectedValue", bsItems, "ItemType", true, DataSourceUpdateMode.OnPropertyChanged));
            //cmbType.DataBindings.Add(new Binding("DisplayMember", _currentItem, "ItemType", true, DataSourceUpdateMode.OnPropertyChanged));
        }

        public async Task<List<ItemViewModel>> GetActiveItems()
        {
            return _items = await _client.GetFromJsonAsync<List<ItemViewModel>>(Program.baseURL + "/item/active");
        }

        public async Task<List<ItemViewModel>> GetInactiveItems()
        {
            return _items = await _client.GetFromJsonAsync<List<ItemViewModel>>(Program.baseURL + "/item/inactive");
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewItemF form = new();
            form.ShowDialog();
        }

        private void grdViewItems_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if(cmbType.SelectedIndex != -1)
            {
                _currentItem = grdViewItems.GetFocusedRow() as ItemViewModel;
                cmbType.SelectedIndex = (int)_currentItem.ItemType;
            }
            
        }
    }
}
