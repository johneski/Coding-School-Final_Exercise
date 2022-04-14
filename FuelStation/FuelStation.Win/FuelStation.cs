using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace FuelStation.Win
{
    public partial class FuelStation : Form
    {
        public FuelStation()
        {
            InitializeComponent();
        }

        private void FuelStation_Load(object sender, EventArgs e)
        {

        }

        private void customersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomersForm form = new CustomersForm(Program.serviceProvider.GetRequiredService<HttpClient>());
            form.ShowDialog();
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemsForm form = new();
            form.ShowDialog();
        }

        private void transactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransactionsForm form = new();
            form.ShowDialog();
        }
    }
}