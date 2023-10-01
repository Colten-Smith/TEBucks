using System.Security.Principal;

namespace TEbucksServer.DTOModels
{
    public class LoginUser
    {
        public string username;
        public string password;
        public LoginUser(string _username, string _password)
        {
            username = _username;
            password = _password;
        }
    }


}
