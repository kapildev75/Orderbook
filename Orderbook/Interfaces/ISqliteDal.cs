﻿using System.Collections.ObjectModel;

namespace ICAP.Orderbook.Interfaces
{
    public interface ISqliteDal
    {
        ObservableCollection<IOrder> Orders { get; }

        void GetOrders(string brokerName);

        int UpdateOrder(IOrder updateOrder);

        int InsertNewOrder(IOrder newOrder);

        int DeleteOrder(int deleteOrderId);
    }
}
