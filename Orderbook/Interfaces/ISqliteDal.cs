using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orderbook.Common;

namespace ICAP.Orderbook.Interfaces
{
    public interface ISqliteDal
    {
        ObservableCollection<IOrder> Orders { get; set; }

        void GetOrders(string brokerName);

        int UpdateOrder(IOrder updateOrder);

        int InsertNewOrder(IOrder newOrder);

        int DeleteOrder(int deleteOrderId);
    }
}
