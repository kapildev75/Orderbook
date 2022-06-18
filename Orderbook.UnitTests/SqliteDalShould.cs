using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using ICAP.Orderbook.Interfaces;
using ICAP.Orderbook;
using ICAP.Orderbook.Common;
using ICAP.Orderbook.Model;
using System.IO;
using System;
using System.Linq;
using Orderbook.Common;

namespace Orderbook.UnitTests
{
    [TestClass]
    public class SqliteDalShould
    {
        private readonly string connectionString;
        IFullOrder defaultOrder;

        public SqliteDalShould()
        {
            connectionString = GetDatabaseConnection();
            ISqliteDal sqliteDal = new SqliteDal(connectionString);

            ServiceLocator.RegisterNewInstanceAndBuild(sqliteDal);
        }

        [TestInitialize]
        public void Setup()
        {
            DeleteAllRecords();

            defaultOrder = new Order();
            defaultOrder.BrokerName = "New Broker";
            defaultOrder.CustomerName = "New Customer";
            defaultOrder.PriceType = PriceType.Silver;
            defaultOrder.Size = 20;
            defaultOrder.SellType = SellType.Sale;
            defaultOrder.Description = "Silver per Kg";
            defaultOrder.Price = 1000;
        }

        [TestMethod]
        public void GetTheOrdersWithoutAddingAnythingReturnsZeroOrders()
        {
            var sqliteDal = ServiceLocator.Resolve<ISqliteDal>();
            sqliteDal.GetOrders("*");

            sqliteDal.Orders.Count.Should().Be(0);
        }

        [TestMethod]
        public void AddNewOrderAndExpectOrderGreaterThanZero()
        {
            var sqliteDal = ServiceLocator.Resolve<ISqliteDal>();
            
            // Nothing should be available in the database.
            sqliteDal.GetOrders("*");
            sqliteDal.Orders.Count.Should().Be(0);

            sqliteDal.InsertNewOrder(defaultOrder);
            sqliteDal.Orders.Count.Should().Be(1);
        }

        [TestMethod]
        public void UpdateAnOrderAndExpectTheSameOrderInReturn()
        {
            var sqliteDal = ServiceLocator.Resolve<ISqliteDal>();

            // Nothing should be available in the database.
            sqliteDal.GetOrders("*");
            sqliteDal.Orders.Count.Should().Be(0);

            sqliteDal.InsertNewOrder(defaultOrder);
            sqliteDal.Orders.Count.Should().Be(1);

            IOrder expectedOrder = new Order();
            expectedOrder.OrderId = sqliteDal.Orders.FirstOrDefault().OrderId;
            expectedOrder.BrokerName = "New Broker Alex";
            expectedOrder.CustomerName = "New Customer John";
            expectedOrder.PriceType = PriceType.Silver;
            expectedOrder.Size = 20;
            expectedOrder.SellType = SellType.Buy;

            // Update the order with new values.
            defaultOrder.BrokerName = "New Broker Alex";
            defaultOrder.CustomerName = "New Customer John";
            defaultOrder.SellType = SellType.Buy;
            
            int updatedRows = sqliteDal.UpdateOrder(expectedOrder);
            updatedRows.Should().Be(1);

            // Order ID is auto generated in database so better to compare each element except Order Id.
            var modifiedOrder = sqliteDal.Orders.FirstOrDefault();
            modifiedOrder.Should().NotBeNull();

            modifiedOrder.BrokerName.Should().Be(defaultOrder.BrokerName);
            modifiedOrder.CustomerName.Should().Be(defaultOrder.CustomerName);
            modifiedOrder.PriceType.Should().Be(defaultOrder.PriceType);
            modifiedOrder.Size.Should().Be(defaultOrder.Size);
            modifiedOrder.SellType.Should().Be(defaultOrder.SellType);
        }

        [TestMethod]
        public void AddNewOrderAndDeleteItAndReturnsZeroOrder()
        {
            var sqliteDal = ServiceLocator.Resolve<ISqliteDal>();

            // Nothing should be available in the database.
            sqliteDal.GetOrders("*");
            sqliteDal.Orders.Count.Should().Be(0);

            sqliteDal.InsertNewOrder(defaultOrder);
            sqliteDal.Orders.Count.Should().Be(1);

            var newAddedOrder = sqliteDal.Orders.FirstOrDefault();
            newAddedOrder.Should().NotBeNull();

            int deletedOrders = sqliteDal.DeleteOrder(newAddedOrder.OrderId);
            deletedOrders.Should().Be(1);

            sqliteDal.Orders.Should().BeEmpty();
        }


        [TestMethod]
        public void AddNewOrderAndDeleteTheWrongOrderAndExpectOneOrderIsAvailable()
        {
            var sqliteDal = ServiceLocator.Resolve<ISqliteDal>();

            // Nothing should be available in the database.
            sqliteDal.GetOrders("*");
            sqliteDal.Orders.Count.Should().Be(0);

            sqliteDal.InsertNewOrder(defaultOrder);
            sqliteDal.Orders.Count.Should().Be(1);

            var newAddedOrder = sqliteDal.Orders.FirstOrDefault();
            newAddedOrder.Should().NotBeNull();

            int deletedOrders = sqliteDal.DeleteOrder(newAddedOrder.OrderId + 1);
            deletedOrders.Should().Be(0);

            sqliteDal.Orders.Count.Should().Be(1);
        }

        [TestMethod]
        public void AddNewOrderWithWrongForeignKeyConstraintAndReturnNoInsertionHappened()
        {
            var sqliteDal = ServiceLocator.Resolve<ISqliteDal>();

            // Nothing should be available in the database.
            sqliteDal.GetOrders("*");
            sqliteDal.Orders.Count.Should().Be(0);

            defaultOrder.SellType = 0;

            int result = sqliteDal.InsertNewOrder(defaultOrder);
            result.Should().Be(0);
            sqliteDal.Orders.Count.Should().Be(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNewOrderWithNullOrderAndExpectException()
        {
            var sqliteDal = ServiceLocator.Resolve<ISqliteDal>();

            // Nothing should be available in the database.
            sqliteDal.GetOrders("*");
            sqliteDal.Orders.Count.Should().Be(0);

            sqliteDal.InsertNewOrder(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateOrderWithNullOrderAndExpectException()
        {
            var sqliteDal = ServiceLocator.Resolve<ISqliteDal>();

            // Nothing should be available in the database.
            sqliteDal.GetOrders("*");
            sqliteDal.Orders.Count.Should().Be(0);

            sqliteDal.UpdateOrder(null);
        }

        private string GetDatabaseConnection()
        {
            var connectionStr = Path.GetFullPath(@"..\..\..\..\..\Orderbook\SqliteDatabase\TestOrder");
            if (!File.Exists(connectionStr))
            {
                throw new Exception($"Test SQLite database is not availble on given path. {connectionStr}");
            }

            connectionStr = $"Data Source={connectionStr}";
            return connectionStr;
        }

        private void DeleteAllRecords()
        {
            var sqliteDal = ServiceLocator.Resolve<ISqliteDal>();
            
            // Delete all the records from database if it exist.
            sqliteDal.GetOrders("*");

            if (sqliteDal.Orders.Count > 0)
            {
                do
                {
                    IFullOrder order = sqliteDal.Orders.First();
                    sqliteDal.DeleteOrder(order.OrderId);
                } while(sqliteDal.Orders.Count > 0);
            }
        }
    }
}
