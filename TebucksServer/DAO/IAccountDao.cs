using System.Collections.Generic;
using TEbucksServer.Models;
using TEBucksServer.Models;

namespace TEbucksServer.DAO
{
    public interface IAccountDao
    {
        Account GetAccountById(int accountId);
        Account GetAccountBalance(string username);
        public List<Transfer> GetAccountTransfers(string username);
        public Account CreateAccount(int userId);
    }
}
