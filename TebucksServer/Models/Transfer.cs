using TEBucksServer.Models;

namespace TEbucksServer.Models
{
    public class Transfer
    {
        /*
        "transferId" : "An integer holding the transfer id",
        "transferType" : "A string for the transfer type: Send or Request",
        "transferStatus" : "A string for the transer status: Pending, Approved, or Rejected",
        "userFrom" : "A user object representing the user who is transfering the money",
        "userTo" : "A user object representing the user who receiving the transfered money",
        "amount" : "A decimal indicating the amount to transfer"
        */
        public int TransferId { get;  set; }
        public string TransferType { get;  set; }
        public string TransferStatus { get;  set; }
        public  User UserFrom { get; set; }
        public  User UserTo { get; set; }
        public decimal Amount { get;  set; }
        public Transfer()
        {

        }
        public Transfer(int id, string type, string status, User from, User to, decimal amount, bool isTracked)
        {
            TransferId = id;
            TransferType = type;
            TransferStatus = status;
            UserFrom = from;
            UserTo = to;
            Amount = amount;
        }
    }
}
