using System;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string message, Exception exception = null)
        {
            Message = message;
            Exception = exception;
        }

        public string Message { get; }

        public Exception Exception { get; }
    }
}