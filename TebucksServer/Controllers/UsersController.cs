using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TEBucksServer.DAO;
using TEBucksServer.Models;

namespace TEbucksServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserDao userDao;

        public UsersController(IUserDao userDao)
        {
            this.userDao = userDao;
        }

        [HttpGet]
        public ActionResult<List<User>> GetUsers()
        {
            try
            {
                List<User> userList = userDao.GetUsers(User.Identity.Name);

                if (userList.Count > 0)
                {
                    return Ok(userList);
                }

                return NotFound("No users available.");
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
