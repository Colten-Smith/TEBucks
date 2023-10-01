using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TEbucksServer.Exceptions;
using TEbucksServer.Models;

namespace TEbucksServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {
        private readonly string connectionString;

        public AccountSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Account CreateAccount(int userId)
        {
            Account newAccount = new Account();
            newAccount.AccountId = 0;
            newAccount.UserId = userId;
            newAccount.Balance = 1000;

            string sql = "INSERT INTO account (user_id, balance) OUTPUT INSERTED.account_id " +
                "VALUES (@user_id, @balance);";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand sqlCommand = new SqlCommand(sql, conn);
                    sqlCommand.Parameters.AddWithValue("@user_id", newAccount.UserId);
                    sqlCommand.Parameters.AddWithValue("@balance", newAccount.Balance);
                    newAccount.AccountId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                }
            }
            catch (Exception)
            {
                return null;
            }

            return newAccount;
        }

        public Account GetAccountById(int accountId)
        {
            Account returnAccount = new Account();
            string sql = "SELECT account_id, user_id, balance FROM account " +
                "WHERE account_id = @account_id;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand sqlCommand = new SqlCommand(sql, conn);
                    sqlCommand.Parameters.AddWithValue("@account_id", accountId);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        returnAccount.AccountId = Convert.ToInt32(reader["account_id"]);
                        returnAccount.UserId = Convert.ToInt32(reader["user_id"]);
                        returnAccount.Balance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return returnAccount;
        }

        public Account GetAccountBalance(string username)
        {
            Account returnAccount = new Account();
            string sql = "SELECT account_id, user_id, balance FROM account WHERE user_id = " +
                "(SELECT TOP 1 user_id FROM tebucks_user WHERE username = @username);";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand sqlCommand = new SqlCommand(sql, conn);
                    sqlCommand.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        returnAccount.AccountId = Convert.ToInt32(reader["account_id"]);
                        returnAccount.UserId = Convert.ToInt32(reader["user_id"]);
                        returnAccount.Balance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return returnAccount;
        }

        public List<Transfer> GetAccountTransfers(string username)
        {
            TransferSqlDao transferDao = new TransferSqlDao(connectionString);
            int accountId = GetAccountBalance(username).AccountId;

            return transferDao.GetTransfersByAccountId(accountId);
        }

        public void Transfer_UpdateBalance(Transfer transfer, Account recipient, Account sender)
        {
            if (transfer.UserFrom.UserId != recipient.UserId)
            {
                if (!(transfer.Amount <= 0))
                {
                    if (transfer.Amount <= sender.Balance)
                    {
                        recipient.Balance += transfer.Amount;
                        sender.Balance -= transfer.Amount;

                        string sql = "UPDATE account SET balance = @balance WHERE account_id = @account_id;";

                        try
                        {
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();

                                SqlCommand recipientCmd = new SqlCommand(sql, conn);
                                recipientCmd.Parameters.AddWithValue("@balance", recipient.Balance);
                                recipientCmd.Parameters.AddWithValue("@account_id", recipient.AccountId);

                                int rowsAffected = recipientCmd.ExecuteNonQuery();

                                if (rowsAffected != 1)
                                {
                                    throw new Exception("Error updating recipient balance.");
                                }
                                else
                                {
                                    SqlCommand senderCmd = new SqlCommand(sql, conn);
                                    senderCmd.Parameters.AddWithValue("@balance", sender.Balance);
                                    senderCmd.Parameters.AddWithValue("@account_id", sender.AccountId);

                                    rowsAffected = senderCmd.ExecuteNonQuery();

                                    if (rowsAffected != 1)
                                    {
                                        throw new Exception("Error updating sender balance.");

                                    }
                                    else
                                    {
                                        // Update recipient and sender's balances was successful.
                                    }
                                }
                            }

                            // If return type is needed, it'd go here.
                        }
                        catch (SqlException)
                        {
                            throw new Exception("SQL exception occurred");
                        }
                    }
                    else
                    {
                        throw new OverdraftException("Insufficient funds.");
                    }
                }
                else
                {
                    throw new InvalidPaymentAmountException("Transfer amount invalid.");
                }
            }
            else
            {
                throw new Exception("Transfer recipient cannot be the current user.");
            }
        }
    }
}
