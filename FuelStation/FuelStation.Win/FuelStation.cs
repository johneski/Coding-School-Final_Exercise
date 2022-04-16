using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;

namespace FuelStation.Win
{
    public partial class FuelStation : Form
    {
        private HttpClient _httpClient;
        public FuelStation()
        {
            InitializeComponent();
            _httpClient = Program.serviceProvider.GetRequiredService<HttpClient>();
        }

        private void FuelStation_Load(object sender, EventArgs e)
        {

        }

        private async void customersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var allowed = await _httpClient.GetFromJsonAsync<bool>(Program.baseURL + "/customer/authorization");
            if (allowed)
            {
                CustomersForm form = new CustomersForm(_httpClient);
                form.ShowDialog();
            }
            else
                MessageBox.Show("You are not authorized!");
        }

        private async void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var allowed = await _httpClient.GetFromJsonAsync<bool>(Program.baseURL + "/item/authorization");
            if (allowed)
            {
                ItemsForm form = new();
                form.ShowDialog();
            }
            else
                MessageBox.Show("You are not authorized!");
            
        }

        private async void transactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var allowed = await _httpClient.GetFromJsonAsync<bool>(Program.baseURL + "/transaction/authorization");
            if (allowed)
            {
                TransactionsForm form = new();
                form.ShowDialog();
            }
            else
                MessageBox.Show("You are not authorized!");
            
        }

        private async void btnLogOut_Click(object sender, EventArgs e)
        {
            await _httpClient.PostAsJsonAsync(Program.baseURL + "/employee/logout", "");
            this.Close();
            LoginForm form = new();
            form.ShowDialog();
        }

        private async void rentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var allowed = await _httpClient.GetFromJsonAsync<bool>(Program.baseURL + "/employee/authorization");
            if (allowed)
            {
                RentForm form = new();
                form.ShowDialog();
            }
            else
                MessageBox.Show("You are not authorized!");
        }
    }
}