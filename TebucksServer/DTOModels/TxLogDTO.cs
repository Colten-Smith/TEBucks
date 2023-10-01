using Microsoft.Extensions.Primitives;
using System;
using TEbucksServer.DAO;
using TEbucksServer.Models;

namespace TEbucksServer.DTOModels
{
    public class TxLogDTO
    {
        //description* string
        //username_from* string
        //username_to* string
        //amount* number($double)
        public string description;
        public string username_from;
        public string username_to;
        public decimal amount;

        public TxLogDTO(Transfer transferToLog)
        {
            username_from = transferToLog.UserFrom.Username;
            username_to = transferToLog.UserTo.Username;
            amount = transferToLog.Amount;
                if (transferToLog.TransferType == "Send")
            {
                description = $"{transferToLog.UserFrom.Firstname} {transferToLog.UserFrom.Lastname} sent {amount.ToString("C2")} to {transferToLog.UserTo.Firstname} {transferToLog.UserTo.Lastname} on {DateTime.Now.ToShortDateString()} at {DateTime.Now.ToShortTimeString()}";
            }
            else
            {
                description = $"{transferToLog.UserTo.Firstname} {transferToLog.UserTo.Lastname} requested ${amount} from {transferToLog.UserFrom.Firstname} {transferToLog.UserFrom.Lastname} on {DateTime.Now.ToShortDateString()} at {DateTime.Now.ToShortTimeString()}";
            }
        }
    }
}
