using System.Collections.Generic;
using System.Data.SqlClient;
using TEbucksServer.DTOModels;
using TEbucksServer.Models;

namespace TEbucksServer.DAO
{
    public interface ITransferDao
    {
        public List<Transfer> GetAllTransfers();
        public List<Transfer> GetTransfersByAccountId(int id);
        public Transfer AddTransferToDatabase(NewTransferDTO transferToAdd);
        public Transfer UpdateTransferStatus(int id, string newStatus);
        public Transfer GetTransferByTransferId(int id);
        public Transfer GetTransferFromReader(SqlDataReader reader);
    }
}
