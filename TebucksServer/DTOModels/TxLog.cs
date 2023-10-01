using System;
using TEbucksServer.Models;

namespace TEbucksServer.DTOModels
{
    public class TxLog
    {
        //description* string
        //username_from* string
        //username_to* string
        //amount* number($double)
        //log_id integer($int32)
        //createdDate string ($date-time)
        public string description;
        public string username_from;
        public string username_to;
        public double amount;
        public int log_id;
        public string createdDate;

    }
}
