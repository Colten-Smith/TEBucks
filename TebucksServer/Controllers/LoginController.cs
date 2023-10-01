using Microsoft.AspNetCore.Mvc;
using TEbucksServer.DAO;
using TEbucksServer.Models;
using TEBucksServer.DAO;
using TEBucksServer.Models;
using TEBucksServer.Security;

namespace TEBucksServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenGenerator tokenGenerator;
        private readonly IPasswordHasher passwordHasher;
        private readonly IUserDao userDao;
        private readonly IAccountDao accountDao;

        public LoginController(ITokenGenerator _tokenGenerator, IPasswordHasher _passwordHasher, IUserDao _userDao, IAccountDao _accountDao)
        {
            tokenGenerator = _tokenGenerator;
            passwordHasher = _passwordHasher;
            userDao = _userDao;
            accountDao = _accountDao;
        }

        [HttpPost]
        public IActionResult Authenticate(LoginUser userParam)
        {
            // Default to bad username/password message
            IActionResult result = BadRequest(new { message = "Username or password is incorrect" });

            // Get the user by username
            User user = userDao.GetUser(userParam.Username);

            // If we found a user and the password hash matches
            if (user != null && passwordHasher.VerifyHashMatch(user.PasswordHash, userParam.Password, user.Salt))
            {
                // Create an authentication token
                string token = tokenGenerator.GenerateToken(user.UserId, user.Username/*, user.Role*/);

                // Create a ReturnUser object to return to the client
                ReturnUser retUser = new ReturnUser() { user = user, Token = token };

                // Switch to 200 OK
                result = Ok(retUser);
            }

            return result;
        }

        [HttpPost("/register")]
        public IActionResult Register(RegisterUser userParam)
        {
            IActionResult result;

            User existingUser = userDao.GetUser(userParam.Username);
            if (existingUser != null)
            {
                return Conflict(new { message = "Username already taken. Please choose a different username." });
            }

            User user = userDao.AddUser(userParam);
            if (user != null)
            {
                Account outputAccount = accountDao.CreateAccount(user.UserId);
                if(outputAccount == null || outputAccount.AccountId == 0)
                {
                    return StatusCode(500);
                }
                result = Created(user.Username, null); //values aren't read on client
            }
            else
            {
                result = BadRequest(new { message = "An error occurred and user was not created." });
            }
            return result;
        }
    }
}
