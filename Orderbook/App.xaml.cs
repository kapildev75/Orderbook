using System;
using System.Configuration;
using System.IO;
using System.Windows;
using ICAP.Orderbook.Interfaces;
using ICAP.Orderbook.Model;

namespace ICAP.Orderbook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly string connectionString = String.Empty;

        public App()
        {
            try
            {
                connectionString = GetDatabaseConnection();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ISqliteDal sqliteDal = new SqliteDal(connectionString);
            ServiceLocator.RegisterNewInstanceAndBuild(sqliteDal);
        }

        public string GetDatabaseConnection()
        {
            var connectionStr = Path.GetFullPath(ConfigurationManager.ConnectionStrings["OrderDatabase"].ConnectionString);
            if (!File.Exists(connectionStr))
            {
                throw new Exception($"SQLite database is not availble on given path. {connectionStr}");
            }

            connectionStr = $"Data Source={connectionStr}";

            return connectionStr;
        }
    }
}
