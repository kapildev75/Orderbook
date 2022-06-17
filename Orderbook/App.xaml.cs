using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using ICAP.Orderbook.Interfaces;
using ICAP.Orderbook.Model;

namespace ICAP.Orderbook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            try
            {

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ISqliteDal sqliteDal = new SqliteDal();
            ServiceLocator.RegisterNewInstanceAndBuild(sqliteDal);
        }
    }
}
