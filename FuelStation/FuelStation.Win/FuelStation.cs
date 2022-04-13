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
    }
}