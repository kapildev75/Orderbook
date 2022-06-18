using ICAP.Orderbook.Common;
using ICAP.Orderbook.Interfaces;

namespace Orderbook.Common
{
    public class Order : NotifyProperty, IFullOrder
    {
        public Order()
        {
            _customerName = string.Empty;
            _brokerName = string.Empty;
            _description = string.Empty;
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

        private PriceType _priceType;

        public PriceType PriceType
        {
            get { return _priceType; }
            set { _priceType = value; OnPropertyChanged("PriceType"); }
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

        private int _priceId;

        public int PriceId
        {
            get { return _priceId; }
            set { _priceId = value; }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private double _price;

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }
    }
}
