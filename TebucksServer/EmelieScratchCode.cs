using System;
using System.Data.SqlClient;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using TEbucksServer.DAO;
using TEbucksServer.Models;
using TEBucksServer.Models;

namespace TEbucksServer
{
    public class EmelieScratchCode
    {
        private readonly string connectionString;

        /*
        The receiver's account balance is increased by the amount of the transfer.
        The sender's account balance is decreased by the amount of the transfer.
        I can't send more TE Bucks than I have in my account.
        I can't send a zero or negative amount.
        I must not be allowed to send money to myself.
        A Sending Transfer has an initial status of Approved.
        */

        public void Transfer_UpdateBalance(Transfer transfer, Account recipient, Account sender)
        {
            if (transfer.UserFrom.UserId != recipient.UserId)
            {
                if (!(transfer.Amount <= 0))
                {
                    if (transfer.Amount >= sender.Balance)
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
                        throw new Exception("Insufficient funds.");
                    }
                }
                else
                {
                    throw new Exception("Transfer amount invalid.");
                }
            }
            else
            {
                throw new Exception("Transfer recipient cannot be the current user.");
            }
        }
    }
}
