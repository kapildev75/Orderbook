using System;
using System.Windows;

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
