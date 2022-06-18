using ICAP.Orderbook.Commands;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ICAP.Orderbook.Common;
using ICAP.Orderbook.Interfaces;
using Orderbook.Common;

namespace ICAP.Orderbook.ViewModel
{
    public class OrderbookViewModel : NotifyProperty
    {
        private readonly ISqliteDal sqliteDal;
        private string _brokerNameForOrders;

        ICommand? _getOrdersCmd;
        ICommand? _updateOrderCmd;
        ICommand? _deleteOrderCmd;

        public OrderbookViewModel()
        {
            sqliteDal = ServiceLocator.Resolve<ISqliteDal>();

            _brokerNameForOrders = "*";
            _getOrdersCmd = null;
            _updateOrderCmd = null;
            _deleteOrderCmd = null;
        }

        public ObservableCollection<IOrder> Orders => sqliteDal.Orders;

        public string BrokerNameForOrders
        {
            get { return _brokerNameForOrders; }
            set
            {
                if (_brokerNameForOrders != value)
                {
                    _brokerNameForOrders = value;
                    OnPropertyChanged("BrokerNameForOrders");
                }
            }
        }

        public ICommand GetOrdersCmd
        {
            get
            {
                if (_getOrdersCmd == null)
                {
                    _getOrdersCmd = new RelayCommand(GetOrders, CanGetOrders);
                }

                return _getOrdersCmd;
            }
        }

        private void GetOrders(object parameter)
        {
            sqliteDal.GetOrders(_brokerNameForOrders);
        }

        private bool CanGetOrders(object parameter)
        {
            return !string.IsNullOrEmpty(_brokerNameForOrders);
        }

        public ICommand UpdateOrderCmd
        {
            get
            {
                if (_updateOrderCmd == null)
                {
                    _updateOrderCmd = new RelayCommand(UpdateOrder, CanUpdateOrder);
                }

                return _updateOrderCmd;
            }
        }

        private void UpdateOrder(object parameter)
        {
            if (parameter != null)
            {
                sqliteDal.UpdateOrder((Order)parameter);
            }
        }

        private bool CanUpdateOrder(object parameter)
        {
            return parameter as Order != null;
        }

        public ICommand DeleteOrderCmd
        {
            get
            {
                if (_deleteOrderCmd == null)
                {
                    _deleteOrderCmd = new RelayCommand(DeleteOrder, CanDeleteOrder);
                }

                return _deleteOrderCmd;
            }
        }

        private void DeleteOrder(object parameter)
        {
            if (parameter is Order order)
            {
                sqliteDal.DeleteOrder(order.OrderId);
            }
        }

        private bool CanDeleteOrder(object parameter)
        {
            return parameter as Order != null;
        }
    }
}
