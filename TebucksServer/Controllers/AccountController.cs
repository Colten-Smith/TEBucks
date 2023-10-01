using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TEbucksServer.DAO;
using TEbucksServer.Models;

namespace TEbucksServer.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountDao accountDao;

        public AccountController(IAccountDao accountDao)
        {
            this.accountDao = accountDao;
        }

        [HttpGet("balance")]
        public ActionResult<Account> GetAccountBalance()
        {
            try
            {
                Account balance = accountDao.GetAccountBalance(User.Identity.Name);

                if (balance == null)
                {
                    return NotFound("ERROR: Balance not found!");
                }
                else
                {
                    return Ok(balance);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("transfers")]
        public ActionResult<List<Transfer>> GetAccountTransfers()
        {
            try
            {
                List<Transfer> transferList = accountDao.GetAccountTransfers(User.Identity.Name);
                return transferList;
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
