using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
using System.Configuration;

namespace ICAP.Orderbook
{
    /// <summary>
    /// Interaction logic for Orderbook.xaml
    /// </summary>
    public partial class OrderbookView : Window
    {
        public OrderbookView()
        {
            InitializeComponent();
        }

        private void NewOrderClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create the new order window
                OrderView orderViewWindow = new OrderView();

                // Show the new order window
                orderViewWindow.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}
