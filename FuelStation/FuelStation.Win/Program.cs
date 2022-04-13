using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace FuelStation.Win
{
    internal static class Program
    {
        public static ServiceProvider serviceProvider;
        public static string baseURL = "https://localhost:7159";
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Adding Services
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<HttpClient>();

            serviceProvider = services.BuildServiceProvider();

            // Initialize credentials

            var httpClient = serviceProvider.GetRequiredService<HttpClient>();
            
            
            ApplicationConfiguration.Initialize();
            Application.Run(new FuelStation());
            

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            
        }
    }
}