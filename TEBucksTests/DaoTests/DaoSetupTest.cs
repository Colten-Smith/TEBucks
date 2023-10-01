using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TEbucksServer.DAO;

namespace TEBucksTests.DaoTests
{
    [TestClass]
    public class DaoSetupTest : BaseDaoTests
    {
        [TestInitialize]
        public override void StartTest()
        {
            TransferSqlDao dao = new TransferSqlDao(ConnectionString);
            base.StartTest();
        }
        [TestMethod]
        public void StartupTests()
        {
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void WhyWontThisWork()
        {
            Assert.IsFalse(false);
        }

    }
}
