using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ICAP.Orderbook.Commands;
using ICAP.Orderbook.Common;
using ICAP.Orderbook.Interfaces;
using Orderbook.Common;

namespace ICAP.Orderbook.ViewModel
{
    public class OrderViewModel : NotifyProperty, IOrder
    {
        private readonly ISqliteDal sqliteDal;
        private ICommand? _insertOrderCmd;

        public OrderViewModel()
        {
            sqliteDal = ServiceLocator.Resolve<ISqliteDal>();
            
            _insertOrderCmd = null;
            CustomerName = string.Empty;
            BrokerName = string.Empty;
            SellType = SellType.Buy;
        }

        public int OrderId { get; set; }

        public string CustomerName { get; set; }

        public string BrokerName { get; set; }

        public double Price { get; set; }

        public int Size { get; set; }

        public SellType SellType { get; set; }

        public ICommand InsertOrderCmd
        {
            get
            {
                if (_insertOrderCmd == null)
                {
                    _insertOrderCmd = new RelayCommand(InsertOrder, CanInsertOrder);
                }

                return _insertOrderCmd;
            }
        }

        private void InsertOrder(object parameter)
        {
            if (parameter != null)
            {
                IOrder newOrder = new Order();
                newOrder.BrokerName = BrokerName;
                newOrder.CustomerName = CustomerName;
                newOrder.Price = Price;
                newOrder.Size = Size;
                newOrder.SellType = SellType;

                sqliteDal.InsertNewOrder(newOrder);
            }
        }

        private bool CanInsertOrder(object parameter)
        {
            if(string.IsNullOrEmpty(CustomerName) || 
                string.IsNullOrEmpty(BrokerName) ||
                Price <= 0 || Size <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
