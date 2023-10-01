using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Security.Cryptography.Xml;
using TEbucksServer.DTOModels;
using TEbucksServer.Exceptions;
using TEbucksServer.Models;
using TEbucksServer.Services;
using TEBucksServer.DAO;
using TEBucksServer.Models;

namespace TEbucksServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private readonly string connectionString;
        private UserSqlDao userDao;
        private AccountSqlDao accountDao;
        public TransferSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
            userDao = new UserSqlDao(dbConnectionString);
            accountDao = new AccountSqlDao(dbConnectionString);
        }
        public List<Transfer> GetAllTransfers()
        {
            try
            {
                string sql = "select * from transfer " +
                    "join transfer_status as status on status.status_id = transfer.status_id " +
                    "join transfer_type as type on type.type_id = transfer.type_id ";
                List<Transfer> output = new List<Transfer>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Transfer newTransfer = GetTransferFromReader(reader);
                        output.Add(newTransfer);
                    }
                }
                return output;
            }
            catch (SqlException)
            {
                return null;
            }
        }
        public List<Transfer> GetTransfersByAccountId(int id)
        {
            try
            {
                List<Transfer> output = new List<Transfer>();
                string sql = "select * from transfer where account_from = @id or account_to = @id";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Transfer newTransfer = GetTransferFromReader(reader);
                        output.Add(newTransfer);
                    }
                }
                return output;
            }
            catch (SqlException)
            {
                return null;
            }
        }
        public Transfer AddTransferToDatabase(NewTransferDTO transferToAdd)
        {
            if (transferToAdd.Amount <= 0)
            {
                throw new InvalidPaymentAmountException();
            }
            Transfer newTransfer = new Transfer();
            newTransfer.Amount = transferToAdd.Amount;
            newTransfer.TransferStatus = GetStatus(transferToAdd);
            if (newTransfer.TransferStatus == null)
            {
                return null;
            }
            newTransfer.TransferType = transferToAdd.TransferType;
            try
            {
                
                string sql = "insert into transfer (account_from, account_to, amount, status_id, type_id) " +
                    "output inserted.transfer_id " +
                    "values ((select top 1 account_id from account where user_id = @from), " +
                    "(select top 1 account_id from account where user_id = @to), @amount, " +
                    "(select top 1 status_id from transfer_status where status_name = @status), " +
                    "(select top 1 type_id from transfer_type where type_name = @type))";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@from", transferToAdd.UserFrom);
                    cmd.Parameters.AddWithValue("@to", transferToAdd.UserTo);
                    cmd.Parameters.AddWithValue("@amount", newTransfer.Amount);
                    cmd.Parameters.AddWithValue("@status", newTransfer.TransferStatus);
                    cmd.Parameters.AddWithValue("@type", newTransfer.TransferType);
                    newTransfer.UserFrom = userDao.GetUserById(transferToAdd.UserFrom);
                    newTransfer.UserTo = userDao.GetUserById(transferToAdd.UserTo);
                    newTransfer.TransferId = Convert.ToInt32(cmd.ExecuteScalar());
                    EnactTransfer(newTransfer);
                    return newTransfer;
                }
            }
            catch (SqlException)
            {
                return null;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public Transfer UpdateTransferStatus(int id, string newStatus)
        {
            try
            {
                string sql = "update transfer set status_id = " +
                    "(select top 1 status_id from transfer_status where status_name = @name) " +
                    "where @name in (select status_name from transfer_status) " +
                    "and transfer_id = @id";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@name", newStatus);
                    cmd.Parameters.AddWithValue("@id", id);
                    if (cmd.ExecuteNonQuery() >= 0)
                    {
                        Transfer output = GetTransferByTransferId(id);
                        EnactTransfer(output);
                        return output;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Transfer GetTransferByTransferId(int id)
        {
            try
            {
                string sql = "select * from transfer " +
                        "where transfer_id = @id";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return GetTransferFromReader(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }
        }
        public Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer output = new Transfer();
            output.TransferId = Convert.ToInt32(reader["transfer_id"]);
            output.UserFrom = userDao.GetUserById((accountDao.GetAccountById(Convert.ToInt32(reader["account_from"]))).UserId);
            output.UserTo = userDao.GetUserById((accountDao.GetAccountById(Convert.ToInt32(reader["account_to"]))).UserId);
            output.TransferType = GetTypeNameById(Convert.ToInt32(reader["type_id"]));
            output.TransferStatus = GetStatusNameById(Convert.ToInt32(reader["status_id"]));
            output.Amount = Convert.ToDecimal(reader["amount"]);

            return output;
        }
        public string GetStatus(NewTransferDTO transfer)
        {
            if (transfer.TransferType.ToLower().Trim() == "send")
            {
                return "Approved";
            }
            else if (transfer.TransferType.ToLower().Trim() == "request")
            {
                return "Pending";
            }
            throw new Exception();
        }
        public string GetStatusNameById(int id)
        {
            string sql = "select top 1 status_name from transfer_status where status_id = @id";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    return Convert.ToString(cmd.ExecuteScalar());
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string GetTypeNameById(int id)
        {
            string sql = "select top 1 type_name from transfer_type where type_id = @id";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    return Convert.ToString(cmd.ExecuteScalar());
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool EnactTransfer(Transfer transferToEnact)
        {
            if(transferToEnact.Amount > accountDao.GetAccountBalance(transferToEnact.UserFrom.Username).Balance)
            {
                LogTransferService txLogService = new LogTransferService();
                txLogService.LogTransfer(transferToEnact);
            }
            try
            {
                if (transferToEnact.TransferStatus == "Approved")
                {
                    Account recipient = accountDao.GetAccountBalance(transferToEnact.UserTo.Username);
                    Account sender = accountDao.GetAccountBalance(transferToEnact.UserFrom.Username);
                    accountDao.Transfer_UpdateBalance(transferToEnact, recipient, sender);
                    if (transferToEnact.Amount >= 1000)
                    {
                        LogTransferService txLogService = new LogTransferService();
                        txLogService.LogTransfer(transferToEnact);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return false;

        }
    }
}
