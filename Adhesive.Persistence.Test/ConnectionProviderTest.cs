using System;
using System.Data;
using Adhesive.Common;
using NUnit.Framework;

namespace Adhesive.Persistence.Test
{
    [TestFixture]
    public class ConnectionProviderTest
    {
        [Test]
        public void TestConnectionProvider()
        {
            IConnectionProvider connectionProvider = LocalServiceLocator.GetService<IConnectionProvider>();
            IDbConnection conn = connectionProvider.GetConnection("CustomerDbContext");
            Console.WriteLine(conn.ConnectionString);
        }
    }
}
