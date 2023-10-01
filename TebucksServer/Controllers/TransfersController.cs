using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TEbucksServer.DAO;
using TEbucksServer.DTOModels;
using TEbucksServer.Exceptions;
using TEbucksServer.Models;
using TEBucksServer.DAO;
using TEBucksServer.Models;
using TEBucksServer.Security;


namespace TEbucksServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransfersController : ControllerBase
    {
        private ITransferDao transferDao;
        private IUserDao userDao;

        public TransfersController(ITransferDao _transferDao, IUserDao _userDao)
        {
            transferDao = _transferDao;
            userDao = _userDao;
        }

        [HttpGet("{id}")]
        public ActionResult<Transfer> GetTransferById(int id)
        {
            try
            {
                Transfer output = transferDao.GetTransferByTransferId(id);
                if (output == null)
                {
                    return NotFound();
                }
                return Ok(output);
            }
            catch (System.Exception)
            {

                return StatusCode(500);
            }
        }
        [HttpPost()]
        public ActionResult<Transfer> AddNewTransfer(NewTransferDTO transferToAdd)
        {
            try
            {
                Transfer output = transferDao.AddTransferToDatabase(transferToAdd);
                if (output == null)
                {
                    return StatusCode(402);
                }
                return Created("/api/transfers/" + output.TransferId, output);
            }
            catch(OverdraftException e)
            {
                return StatusCode(402);
            }
            catch(InvalidPaymentAmountException e)
            {
                return BadRequest("Please Enter a full number greater than 0.");
            }
            catch (System.Exception)
            {
                return StatusCode(500);
            }
        }
        [HttpPut("{id}/status")]
        public ActionResult<Transfer> UpdateTransferStatus(int id, TransferStatusUpdateDTO update)
        {
            //TODO Fix UpdateTransferStatus
            try
            {
                Transfer output = transferDao.UpdateTransferStatus(id, update.transferStatus);
                if(output == null)
                {
                    return NotFound();
                }
                return Ok(output);
            }
            catch (System.Exception)
            {
                return StatusCode(500);
            }
        }
    } 
}
