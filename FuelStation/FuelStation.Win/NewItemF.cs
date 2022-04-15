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
    public partial class NewItemF : Form
    {
        private ItemViewModel _item = new();
        private HttpClient _client;
        public NewItemF()
        {
            InitializeComponent();
            _client = Program.serviceProvider.GetRequiredService<HttpClient>();
        }

        private async void NewItemF_Load(object sender, EventArgs e)
        {
            _item.Code = await _client.GetFromJsonAsync<string>(Program.baseURL + "/item/newcode");
            SetBindings();
            
        }

        private void SetBindings()
        {
            txtCode.DataBindings.Add(new Binding("EditValue", _item, "Code", true));
            txtDescription.DataBindings.Add(new Binding("EditValue", _item, "Description", true));
            cmbType.DataSource = Enum.GetValues(typeof(ItemType));
            cmbType.DataBindings.Add(new Binding("SelectedValue", _item, "ItemType", true));
            spinCost.DataBindings.Add(new Binding("EditValue", _item, "Cost", true));
            spinPrice.DataBindings.Add(new Binding("EditValue", _item, "Price", true));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var response = await _client.PostAsJsonAsync(Program.baseURL + "/item", _item);
            if (!response.IsSuccessStatusCode)
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
