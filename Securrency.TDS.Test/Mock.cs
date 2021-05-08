using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Securrency.TDS.Web.DataLayer;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Extras.Moq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Module = Autofac.Module;

namespace Securrency.TDS.Test
{
    internal class DbContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            IOptions<DataLayerOptions> opts = Options.Create(new DataLayerOptions
                {ConnectionString = Db.ApplicationConnectionString});
            var factory = new MockDbContextFactory(opts, TestLogger.Factory);
            builder.RegisterInstance(factory).As<IDbContextFactory>();
        }

        private class MockDbContextFactory : IDbContextFactory
        {
            private readonly IOptions<DataLayerOptions> _options;
            private readonly ILoggerFactory _loggerFactory;

            public MockDbContextFactory(IOptions<DataLayerOptions> options, ILoggerFactory loggerFactory)
            {
                _options = options;
                _loggerFactory = loggerFactory;
            }

            public AppDbContext CreateContext()
            {
                return new AppDbContext(_options, _loggerFactory);
            }

            public SqlConnection CreateConnection()
            {
                return new SqlConnection(_options.Value.ConnectionString);
            }
        }
    }

    internal class LoggerModule : Module
    {
        

        protected override void AttachToComponentRegistration(
            IComponentRegistryBuilder componentRegistry,
            IComponentRegistration registration)
        {
            base.AttachToComponentRegistration(componentRegistry, registration);
            registration.Preparing += RegistrationOnPreparing;
        }

        private void RegistrationOnPreparing(object sender, PreparingEventArgs e)
        {
            bool Match(ParameterInfo info, IComponentContext context)
            {
                return typeof(ILogger).IsAssignableFrom(info.ParameterType);
            }

            object Provide(ParameterInfo info, IComponentContext context)
            {
                Type loggerType = typeof(Logger<>).MakeGenericType(info.Member.DeclaringType!);
                ConstructorInfo constructor = loggerType.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                    null, new[] {typeof(ILoggerFactory)}, null);
                Debug.Assert(constructor != null);
                object instance = constructor.Invoke(new object[] {TestLogger.Factory});
                return instance;
            }

            e.Parameters = e.Parameters.Union(new[] {new ResolvedParameter(Match, Provide)});
        }
    }

    public static class Mock
    {
        public static AutoMock Auto()
        {
            return AutoMock.GetLoose(builder =>
            {
                builder.RegisterModule<DbContextModule>();
                builder.RegisterModule<LoggerModule>();
            });
        }
    }
}