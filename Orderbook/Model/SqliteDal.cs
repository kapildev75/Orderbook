using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using ICAP.Orderbook.Common;
using ICAP.Orderbook.Interfaces;
using Orderbook.Common;

namespace ICAP.Orderbook.Model
{
    public class SqliteDal : ISqliteDal
    {
        readonly string connectionString;

        public SqliteDal(string connectionString)
        {
            this.connectionString = connectionString;

            Orders = new ObservableCollection<IFullOrder>();
        }

        public ObservableCollection<IFullOrder> Orders { get; }

        public void GetOrders(string brokerName)
        {
            if (string.IsNullOrEmpty(brokerName))
            {
                throw new ArgumentNullException(nameof(brokerName));
            }

            try
            {
                using SqliteConnection conn = new SqliteConnection(connectionString);

                string commandQuery = brokerName == "*" ?
                    $"Select OrderId, [Customer Name], [Broker Name], Price, Size, SellType, " +
                    "Description, [Price in $] from Orderbook " +
                    "Inner Join " +
                    "Price On Orderbook.Price == Price.Id" :
                    "Select OrderId, [Customer Name], [Broker Name], Price, Size, SellType, " +
                    "Description, [Price in $] from Orderbook " +
                    "Left Join Price On Orderbook.Price == Price.Id " +
                    "Where[Broker Name] = '{brokerName}'";

                SqliteCommand command = new SqliteCommand(commandQuery, conn);

                conn.Open();

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    // Clear old observable list and fill with new order data.
                    Orders.Clear();
                    while (reader.Read())
                    {
                        IFullOrder order = ConvertSqliteRecordIntoOrder(reader);
                        Orders.Add(order);
                    }
                }
            }
            catch (SqliteException exception)
            {
                throw exception;
            }
        }

        public int UpdateOrder(IOrder updateOrder)
        {
            if (updateOrder == null)
            {
                throw new ArgumentNullException(nameof(updateOrder));
            }

            if (!ValidateEnumItem<SellType>(updateOrder.SellType))
            {
                return 0;
            }

            if (!ValidateEnumItem<PriceType>(updateOrder.PriceType))
            {
                return 0;
            }

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    SqliteCommand command = conn.CreateCommand();

                    command.CommandText = $"UPDATE Orderbook SET [Customer Name] = '{updateOrder.CustomerName}', " +
                        $"[Broker Name] = '{updateOrder.BrokerName}', " +
                        $"Price = {(int)updateOrder.PriceType}, " +
                        $"Size = {updateOrder.Size}, " +
                        $"SellType = {(int)updateOrder.SellType} " +
                        $"WHERE OrderId = {updateOrder.OrderId}";

                    command.Parameters.AddWithValue("@[Customer Name]", updateOrder.CustomerName);
                    command.Parameters.AddWithValue("@[Broker Name]", updateOrder.BrokerName);
                    command.Parameters.AddWithValue("@Price", (int)updateOrder.PriceType);
                    command.Parameters.AddWithValue("@Size", updateOrder.Size);
                    command.Parameters.AddWithValue("@SellType", (int)updateOrder.SellType);

                    conn.Open();
                    int updatedRecordNumber = command.ExecuteNonQuery();
                    if (updatedRecordNumber > 0)
                    {
                        command.CommandText = BuildSelectQueryBasedOnOrderId(updateOrder.OrderId);

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                IFullOrder updatedOrder = ConvertSqliteRecordIntoOrder(reader);
                                UpdateObservableCollectionOrders(updatedOrder);
                            }
                        }
                    }

                    return updatedRecordNumber;
                }
            }
            catch (SqliteException exception)
            {
                throw exception;
            }
        }

        public int InsertNewOrder(IOrder newOrder)
        {
            if (newOrder == null)
            {
                throw new ArgumentNullException(nameof(newOrder));
            }

            if (!ValidateEnumItem<SellType>(newOrder.SellType))
            {
                return 0;
            }

            if (!ValidateEnumItem<PriceType>(newOrder.PriceType))
            {
                return 0;
            }

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    SqliteCommand command = conn.CreateCommand();

                    command.CommandText = $"INSERT INTO Orderbook ( [Customer Name], " +
                        $"[Broker Name], " +
                        $"Price, " +
                        $"Size, " +
                        $"SellType ) " +
                        $"VALUES ( '{newOrder.CustomerName}', " +
                        $"'{newOrder.BrokerName}', " +
                        $"{(int)newOrder.PriceType}, " +
                        $"{newOrder.Size}, " +
                        $"{(int)newOrder.SellType} )";

                    command.Parameters.AddWithValue("@[Customer Name]", newOrder.CustomerName);
                    command.Parameters.AddWithValue("@[Broker Name]", newOrder.BrokerName);
                    command.Parameters.AddWithValue("@Price", (int)newOrder.PriceType);
                    command.Parameters.AddWithValue("@Size", newOrder.Size);
                    command.Parameters.AddWithValue("@SellType", (int)newOrder.SellType);

                    conn.Open();

                    int insertRecordNumber = command.ExecuteNonQuery();
                    if (insertRecordNumber > 0)
                    {
                        command.CommandText = "SELECT MAX(OrderId) AS max_id FROM Orderbook";

                        object? lastPrimaryKey = command.ExecuteScalar();
                        if (lastPrimaryKey != null)
                        {
                            int newOrderId = Convert.ToInt32(lastPrimaryKey);

                            command.CommandText = BuildSelectQueryBasedOnOrderId(newOrderId);

                            using (SqliteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    IFullOrder order = ConvertSqliteRecordIntoOrder(reader);
                                    Orders.Add(order);
                                }
                            }
                        }
                    }

                    return insertRecordNumber;
                }
            }
            catch (SqliteException exception)
            {
                throw exception;
            }
        }

        public int DeleteOrder(int deleteOrderId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    SqliteCommand command = conn.CreateCommand();

                    command.CommandText = $"DELETE from Orderbook WHERE OrderId = {deleteOrderId}";

                    conn.Open();
                    int deletedRecordNumber = command.ExecuteNonQuery();
                    if (deletedRecordNumber > 0)
                    {
                        DeleteItemFromObservableCollection(deleteOrderId);
                    }

                    return deletedRecordNumber;
                }
            }
            catch (SqliteException exception)
            {
                throw exception;
            }
        }

        public T NumToEnum<T>(int number)
        {
            return (T)Enum.ToObject(typeof(T), number);
        }

        private void DeleteItemFromObservableCollection(int deleteOrderId)
        {
            var item = Orders.FirstOrDefault(x => x.OrderId == deleteOrderId);
            if (item != null)
            {
                Orders.Remove(item);
            }
        }

        private void UpdateObservableCollectionOrders(IFullOrder order)
        {
            var oldItem = Orders.FirstOrDefault(x => x.OrderId == order.OrderId);
            if (oldItem != null)
            {
                var oldIndex = Orders.IndexOf(oldItem);
                Orders[oldIndex] = order;
            }
        }

        private string BuildSelectQueryBasedOnOrderId(int orderId)
        {
            return $"Select OrderId, [Customer Name], [Broker Name], Price, Size, " +
                                $"SellType, Description, [Price in $] from Orderbook " +
                                $"Left Join Price On Orderbook.Price == Price.Id " +
                                $"Where Orderbook.OrderId == {orderId}";
        }

        private IFullOrder ConvertSqliteRecordIntoOrder(SqliteDataReader reader)
        {
            IFullOrder order = new Order();

            order.OrderId = Convert.ToInt32(reader["OrderId"]);

            var customerName = reader["Customer Name"];
            if (customerName != null)
            {
                order.CustomerName = (string)customerName;
            }

            var bName = reader["Broker Name"];
            if (bName != null)
            {
                order.BrokerName = (string)bName;
            }

            int priceType = Convert.ToInt32(reader["Price"]);
            order.PriceType = NumToEnum<PriceType>(priceType);

            order.Size = Convert.ToInt32(reader["Size"]);

            int sellType = Convert.ToInt32(reader["SellType"]);
            order.SellType = NumToEnum<SellType>(sellType);

            var description = reader["Description"];
            if (description != null)
            {
                order.Description = (string)description;
            }

            order.Price = Convert.ToDouble(reader["Price in $"]);

            return order;
        }

        private bool ValidateEnumItem<T>(T eEnumItem)
        {
            if (eEnumItem == null)
            {
                return false;
            }

            return Enum.IsDefined(typeof(T), eEnumItem) ? true : false;
        }
    }
}
