using Autofac;

namespace ICAP.Orderbook
{

    /// <summary>
    ///     Static class to provides functionality for container.
    /// </summary>
    public static class ServiceLocator
    {
        private static ContainerBuilder containerBuilder;

        private static IContainer Container { get; set; }

        public static bool IsContainerBuild => Container != null;

        public static void Build()
        {
            if (!IsContainerBuild &&
                containerBuilder != null)
            {
                Container = containerBuilder.Build();
            }
        }

        /// <summary>
        ///     Resolves an instance which has no session or connection specific dependencies or state.
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Type value</returns>
        public static T GetInstance<T>()
        {
            var instance = Container.Resolve<T>();
            return instance;
        }

        /// <summary>
        ///     Registers the instance. Normally this is used only in the service locator
        ///     but may be used to override normal configured instances for test purposes
        /// </summary>
        /// <typeparam name="T">Interface type to register</typeparam>
        /// <param name="instance">The instance.</param>
        public static void RegisterAndBuild<T>(T instance)
            where T : class
        {
            if (IsContainerBuild)
            {
                return;
            }

            RegisterInstance(instance);
            Build();
        }

        /// <summary>
        ///     Registers the instance. Normally this is used only in the service locator
        ///     but may be used to override normal configured instances for test purposes
        /// </summary>
        /// <typeparam name="T">Interface type to register</typeparam>
        /// <param name="instance">The instance.</param>
        public static void RegisterInstance<T>(T instance)
            where T : class
        {
            if (containerBuilder == null)
            {
                containerBuilder = new ContainerBuilder();
            }

            containerBuilder.RegisterInstance(instance).As<T>();
        }

        /// <summary>
        ///     Registers the instance. Normally this is used only in the service locator
        ///     but may be used to override normal configured instances for test purposes
        /// </summary>
        /// <typeparam name="T">Interface type to register</typeparam>
        /// <param name="instance">The instance.</param>
        public static void RegisterNewInstanceAndBuild<T>(T instance)
            where T : class
        {
            containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(instance).As<T>();
            Container = containerBuilder.Build();
        }

        /// <summary>
        /// Registers single instance of base class.
        /// </summary>
        /// <typeparam name="TBase">The type of base.</typeparam>
        /// <param name="instance">The instance.</param>
        public static void RegisterSingleInstance<TBase>(object instance)
        {
            if (containerBuilder == null)
            {
                containerBuilder = new ContainerBuilder();
            }

            containerBuilder.RegisterInstance(instance).As<TBase>().SingleInstance();
        }

        public static void RegisterSingleton<TDerived, TBase>()
            where TDerived : TBase
        {
            if (containerBuilder == null)
            {
                containerBuilder = new ContainerBuilder();
            }

            containerBuilder.RegisterType<TDerived>().As<TBase>().SingleInstance();
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <typeparam name="TDerived">The type of derived.</typeparam>
        /// <typeparam name="TBase">The type of base.</typeparam>
        public static void RegisterType<TDerived, TBase>()
           where TDerived : TBase
        {
            if (containerBuilder == null)
            {
                containerBuilder = new ContainerBuilder();
            }

            containerBuilder.RegisterType<TDerived>().As<TBase>();
        }

        /// <summary>
        ///     Provides an instance of T.
        /// </summary>
        /// <typeparam name="T">Type of the resolved class.</typeparam>
        /// <returns>The instance of the T type.</returns>
        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// Registers the new container. This is only for testing. Please don't use this somewhereelse.
        /// </summary>
        /// <param name="newContainer"></param>
        public static void RegisterNewContainer(IContainer newContainer)
        {
            Container = newContainer;
        }
    }
}