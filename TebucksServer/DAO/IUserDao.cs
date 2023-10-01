using System.Collections.Generic;
using TEBucksServer.Models;

namespace TEBucksServer.DAO
{
    public interface IUserDao
    {
        User GetUser(string username);
        User AddUser(RegisterUser registerUser);
        List<User> GetUsers(string usernameToIgnore = "");
    }
}
