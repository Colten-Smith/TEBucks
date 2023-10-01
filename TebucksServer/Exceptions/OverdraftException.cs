using System;

namespace TEbucksServer.Exceptions
{
    public class OverdraftException : Exception
    {
        public OverdraftException() : base() { }
        public OverdraftException(string message) : base(message) { }
        public OverdraftException(string message, Exception inner) : base(message, inner) { }
    }
}
