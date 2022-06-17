using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Configuration;
using System.Collections.ObjectModel;
using System.IO;
using ICAP.Orderbook.Common;
using ICAP.Orderbook.Interfaces;
using Orderbook.Common;
using System.Data;

namespace ICAP.Orderbook.Model
{
    public class SqliteDal : ISqliteDal
    {
        readonly string connectionString;

        public SqliteDal()
        {
            var connectionStr = Path.GetFullPath(ConfigurationManager.ConnectionStrings["OrderDatabase"].ConnectionString);
            if (!File.Exists(connectionStr))
            {
                throw new Exception($"SQLite database is not availble on given path. {connectionStr}");
            }

            connectionString = $"Data Source={connectionStr}";

            Orders = new ObservableCollection<IOrder>();
        }

        public ObservableCollection<IOrder> Orders { get; set; }

        public void GetOrders(string brokerName)
        {
            if(string.IsNullOrEmpty(brokerName))
            {
                throw new ArgumentNullException(nameof(brokerName));
            }

            try
            {
                using SqliteConnection conn = new SqliteConnection(connectionString);

                string commandQuery = brokerName == "*" ?
                    "Select * from Orderbook" :
                    $"select * from Orderbook where [Broker Name] = '{brokerName}'";

                SqliteCommand command = new SqliteCommand(commandQuery, conn);

                conn.Open();

                using SqliteDataReader reader = command.ExecuteReader();

                // Clear old observable list and fill with new order data.
                Orders.Clear();

                while (reader.Read())
                {
                    IOrder order = new Order();

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

                    order.Price = Convert.ToDouble(reader["Price"]);

                    order.Size = Convert.ToInt32(reader["Size"]);

                    int sellType = Convert.ToInt32(reader["SellType"]);
                    order.SellType = NumToEnum<SellType>(sellType);

                    Orders.Add(order);
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

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    SqliteCommand command = conn.CreateCommand();

                    command.CommandText = $"UPDATE Orderbook SET [Customer Name] = '{updateOrder.CustomerName}', " +
                        $"[Broker Name] = '{updateOrder.BrokerName}', " +
                        $"Price = {updateOrder.Price}, " +
                        $"Size = {updateOrder.Size}, " +
                        $"SellType = {(int)updateOrder.SellType} " +
                        $"WHERE OrderId = {updateOrder.OrderId}";

                    command.Parameters.AddWithValue("@[Customer Name]", updateOrder.CustomerName);
                    command.Parameters.AddWithValue("@[Broker Name]", updateOrder.BrokerName);
                    command.Parameters.AddWithValue("@Price", updateOrder.Price);
                    command.Parameters.AddWithValue("@Size", updateOrder.Size);
                    command.Parameters.AddWithValue("@SellType", (int)updateOrder.SellType);

                    conn.Open();
                    int updatedRecordNumber = command.ExecuteNonQuery();
                    if(updatedRecordNumber > 0)
                    {
                        UpdateObservableCollectionOrders(updateOrder);
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
                        $"{newOrder.Price}, " +
                        $"{newOrder.Size}, " +
                        $"{(int)newOrder.SellType} )";

                    command.Parameters.AddWithValue("@[Customer Name]", newOrder.CustomerName);
                    command.Parameters.AddWithValue("@[Broker Name]", newOrder.BrokerName);
                    command.Parameters.AddWithValue("@Price", newOrder.Price);
                    command.Parameters.AddWithValue("@Size", newOrder.Size);
                    command.Parameters.AddWithValue("@SellType", (int)newOrder.SellType);

                    conn.Open();

                    int insertRecordNumber = command.ExecuteNonQuery();
                    if (insertRecordNumber > 0)
                    {
                        command.CommandText = "SELECT MAX(OrderId) AS max_id FROM Orderbook";

                        object? lastPrimaryKey = command.ExecuteScalar();
                        if(lastPrimaryKey != null)
                        {
                            int newOrderId = Convert.ToInt32(lastPrimaryKey);
                            newOrder.OrderId = newOrderId;
                        }

                        Orders.Add(newOrder);
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

        private void DeleteItemFromObservableCollection(int deleteOrderId)
        {
            var item = Orders.FirstOrDefault(x => x.OrderId == deleteOrderId);
            if (item != null)
            {
                Orders.Remove(item);
            }
        }

        private void UpdateObservableCollectionOrders(IOrder order)
        {
            var oldItem = Orders.FirstOrDefault(x => x.OrderId == order.OrderId);
            if (oldItem != null)
            {
                var oldIndex = Orders.IndexOf(oldItem);
                Orders[oldIndex] = order;
            }
        }

        public T NumToEnum<T>(int number)
        {
            return (T)Enum.ToObject(typeof(T), number);
        }
    }
}
