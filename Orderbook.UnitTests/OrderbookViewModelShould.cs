using Microsoft.VisualStudio.TestTools.UnitTesting;
using ICAP.Orderbook.ViewModel;
using FluentAssertions;
using System;
using ICAP.Orderbook.Interfaces;
using NSubstitute;
using Autofac;
using ICAP.Orderbook;
using System.Windows.Input;

namespace Orderbook.UnitTests
{
    [TestClass]
    public class OrderbookViewModelShould
    {
        private OrderbookViewModel target;

        internal ISqliteDal MockSqliteDal { get; set; }

        [TestInitialize]
        public void Setup()
        {
            MockSqliteDal = Substitute.For<ISqliteDal>();

            ServiceLocator.RegisterNewInstanceAndBuild(MockSqliteDal);

            target = new OrderbookViewModel();
        }


        [TestMethod]
        public void CreateViewModelAndGetTheOrders()
        {
            MockSqliteDal = ServiceLocator.Resolve<ISqliteDal>();

            this.target.Orders.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void CreateViewModelAndGetDefaultBrokerNameForOrders()
        {
            string expectedValue = "*";

            this.target.BrokerNameForOrders.Should().Contain(expectedValue);
        }

        [TestMethod]
        public void CreateViewModelAndSetNewBrokerNameForOrders()
        {
            string expectedValue = "Kapil Dev";

            this.target.BrokerNameForOrders = "Kapil Dev";

            this.target.BrokerNameForOrders.Should().Contain(expectedValue);
        }

        [TestMethod]
        public void CreateViewModelAndGetsTheOrdersCommand()
        {
            ICommand ordersCmd = this.target.GetOrdersCmd;
            ordersCmd.Should().NotBeNull();
        }

        [TestMethod]
        public void CreateViewModelAndGetsTheUpdateOrdersCommand()
        {
            ICommand updateOrderCmd = this.target.UpdateOrderCmd;
            updateOrderCmd.Should().NotBeNull();
        }

        [TestMethod]
        public void CreateViewModelAndGetsTheDeleteOrdersCommand()
        {
            ICommand deleteOrderCmd = this.target.DeleteOrderCmd;
            deleteOrderCmd.Should().NotBeNull();
        }
    }
}