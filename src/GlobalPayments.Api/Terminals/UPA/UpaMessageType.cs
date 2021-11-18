namespace GlobalPayments.Api.Terminals.UPA
{
    internal class UpaMessageType
    {
        public const string Ack = "ACK";
        public const string Nak = "NAK";
        public const string Ready = "READY";
        public const string Busy = "BUSY";
        public const string TimeOut = "TO";
        public const string Msg = "MSG";
    }
}
