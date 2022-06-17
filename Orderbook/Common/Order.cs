using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ICAP.Orderbook.Common;
using ICAP.Orderbook.Interfaces;

namespace Orderbook.Common
{
    public class Order : NotifyProperty, IOrder
    {
        public Order()
        {
            _customerName = string.Empty;
            _brokerName = string.Empty;
        }

        private int _orderId;

        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }

        private string _customerName;

        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; OnPropertyChanged("CustomerName"); }
        }

        private string _brokerName;

        public string BrokerName
        {
            get { return _brokerName; }
            set { _brokerName = value; OnPropertyChanged("BrokerName"); }
        }

        private double _price;

        public double Price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged("Price"); }
        }

        private int _size;

        public int Size
        {
            get { return _size; }
            set { _size = value; OnPropertyChanged("Size"); }
        }

        private SellType _sellType;

        public SellType SellType
        {
            get { return _sellType; }
            set { _sellType = value; OnPropertyChanged("SellType"); }
        }
    }
}
