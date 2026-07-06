using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Exceptions;
using Promethix.Framework.Ado.Implementation;
using Promethix.Framework.Ado.Interfaces;
using System.Data.Common;

namespace Promethix.Framework.Ado.Tests.IntegrationTests
{
    [TestClass]
    public class AdoScopeFactoryOptionsIsolationTests
    {
        private readonly IAdoScopeFactory adoScopeFactory;

        private readonly IAmbientAdoContextLocator ambientAdoContextLocator;

        public AdoScopeFactoryOptionsIsolationTests()
        {
            var services = new ServiceCollection();
            DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", SqliteFactory.Instance);

            services.AddSingleton<IAmbientAdoContextLocator, AmbientAdoContextLocator>();
            services.AddSingleton<IAdoContextGroupFactory, AdoContextGroupFactory>();
            services.AddSingleton<IAdoScopeFactory, AdoScopeFactory>();
            services.AddSingleton(new AdoScopeOptionsBuilder()
                .WithScopeExecutionOption(AdoContextGroupExecutionOption.Standard));
            services.AddSingleton<IAdoContextOptionsRegistry>(_ =>
                new AdoContextConfigurationBuilder()
                    .AddAdoContext<NonTransactionalSqliteContext>(options => options
                        .WithNamedContext(nameof(NonTransactionalSqliteContext))
                        .WithProviderName("Microsoft.Data.Sqlite")
                        .WithConnectionString("Data Source=options-isolation.db")
                        .WithExecutionOption(AdoContextExecutionOption.NonTransactional))
                    .Build());

            ServiceProvider container = services.BuildServiceProvider();
            adoScopeFactory = container.GetRequiredService<IAdoScopeFactory>();
            ambientAdoContextLocator = container.GetRequiredService<IAmbientAdoContextLocator>();
        }

        [TestMethod]
        public void ExplicitScopeOverrides_DoNotLeakIntoLaterScopes()
        {
            NonTransactionalSqliteContext explicitContext;

            using (IAdoScope explicitScope = adoScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                explicitContext = ambientAdoContextLocator.GetContext<NonTransactionalSqliteContext>();
                Assert.IsTrue(explicitContext.IsInTransaction);
                explicitScope.Complete();
            }

            Assert.IsFalse(explicitContext.IsInTransaction);

            using (IAdoScope defaultScope = adoScopeFactory.Create())
            {
                NonTransactionalSqliteContext defaultContext = ambientAdoContextLocator.GetContext<NonTransactionalSqliteContext>();
                Assert.IsFalse(defaultContext.IsInTransaction);
                defaultScope.Complete();
            }
        }

        [TestMethod]
        public void IncompleteExplicitScope_ClearsTransactionStateOnRollback()
        {
            NonTransactionalSqliteContext rollbackContext;

            using (IAdoScope explicitScope = adoScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                rollbackContext = ambientAdoContextLocator.GetContext<NonTransactionalSqliteContext>();
                Assert.IsTrue(rollbackContext.IsInTransaction);
            }

            Assert.IsFalse(rollbackContext.IsInTransaction);
        }

        [TestMethod]
        public void UnconfiguredContext_ThrowsExplicitLibraryException()
        {
            using IAdoScope adoScope = adoScopeFactory.Create();

            UnconfiguredAdoContextException ex = Assert.ThrowsException<UnconfiguredAdoContextException>(
                () => ambientAdoContextLocator.GetContext<UnconfiguredSqliteContext>());

            StringAssert.Contains(ex.Message, nameof(UnconfiguredSqliteContext));
        }

        public class NonTransactionalSqliteContext : AdoContext
        {
        }

        public class UnconfiguredSqliteContext : AdoContext
        {
        }
    }
}
