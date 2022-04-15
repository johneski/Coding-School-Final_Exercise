using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuelStation.Win
{
    public class Login
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }
    public partial class LoginForm : Form
    {
        private Login login = new();
        private HttpClient client;


        public LoginForm()
        {
            InitializeComponent();
            client = Program.serviceProvider.GetRequiredService<HttpClient>();
        }

        private async void simpleButton1_Click(object sender, EventArgs e)
        {
            HttpResponseMessage response;
            string authorization;
            using (var request = new HttpRequestMessage(HttpMethod.Post, Program.baseURL + "/validation"))
            {
                request.Headers.Add("username", login.username);
                request.Headers.Add("password", login.password);
                try
                {
                    response = await client.SendAsync(request);
                    authorization = await response.Content.ReadAsStringAsync();
                }catch (Exception ex)
                {
                    authorization = Guid.Empty.ToString("D");
                }
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorization.Replace("\"", ""));
            }

            authorization = authorization.Replace("\"", "");
            if (Guid.Parse(authorization) != Guid.Empty)
            {
                FuelStation form = new();
                this.Hide();
                form.ShowDialog();
                this.Close();
                return;
            }


        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            SetBindings();
        }

        private void SetBindings()
        {
            txtUsername.DataBindings.Add(new Binding("EditValue", login, "username", true));
            txtPassword.DataBindings.Add(new Binding("EditValue", login, "password", true));
        }
    }
}
