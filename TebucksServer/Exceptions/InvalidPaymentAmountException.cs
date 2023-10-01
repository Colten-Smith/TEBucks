using System;

namespace TEbucksServer.Exceptions
{
    public class InvalidPaymentAmountException : Exception
    {
        public InvalidPaymentAmountException() : base() { }
        public InvalidPaymentAmountException(string message) : base(message) { }
        public InvalidPaymentAmountException(string message, Exception inner) : base(message, inner) { }
    }
}
