using ICAP.Orderbook.Common;

namespace ICAP.Orderbook.Interfaces
{
    public interface IOrder
    {
        int OrderId { get; set; }

        string CustomerName { get; set; }

        string BrokerName { get; set; }

        double Price { get; set; }

        int Size { get; set; }

        SellType SellType { get; set; }
    }
}
