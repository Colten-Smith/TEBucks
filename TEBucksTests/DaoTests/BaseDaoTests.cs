using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using TEbucksServer.DAO;
using System.Data.Common;
using System.Transactions;
using System.IO;
using System;

namespace TEBucksTests
{
    [TestClass]
    public class BaseDaoTests
    {
        private const string DatabaseName = "tebucksTemp";


        private static string AdminConnectionString;
        protected static string ConnectionString;
        private TransactionScope testTransaction;



        //TODO Finish BaseDaoTests (Assembly, Test)
        [AssemblyInitialize]
        public void StartTests()
        {
            SetConnectionStrings(DatabaseName);

            //Make the test Database
            string sql = File.ReadAllText("SQLs\\create-test-db.sql").Replace("test_db_name", DatabaseName);

            using (SqlConnection conn = new SqlConnection(AdminConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.ExecuteNonQuery();
            }

            sql = File.ReadAllText("SQLs\\test-data.sql");
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
            }

        }
        [AssemblyCleanup]
        public void EndTests()
        {
            string sql = File.ReadAllText("SQLs\\drop-test-db.sql").Replace("test_db_name", DatabaseName);

            using (SqlConnection conn = new SqlConnection(AdminConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }

        [TestInitialize]
        public virtual void StartTest()
        {
            
            testTransaction = new TransactionScope();
        }

        [TestCleanup]
        public void EndTest()
        {
            testTransaction.Dispose();
        }



        private static void SetConnectionStrings(string defaultDbName)
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? @".\SQLEXPRESS";
            string dbName = Environment.GetEnvironmentVariable("DB_DATABASE") ?? defaultDbName;
            string username = Environment.GetEnvironmentVariable("DB_USERNAME");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            if (username != null && password != null)
            {
                ConnectionString = $"Data Source={host};Initial Catalog={dbName};User Id={username};Password={password};";
            }
            else
            {
                ConnectionString = $"Data Source={host};Initial Catalog={dbName};Integrated Security=SSPI;";
            }
            AdminConnectionString = ConnectionString.Replace(dbName, "master");
        }
    }
}