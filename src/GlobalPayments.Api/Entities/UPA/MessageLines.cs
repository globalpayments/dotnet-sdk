namespace GlobalPayments.Api.Entities.UPA {
    public class MessageLines {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Line4 { get; set; }
        public string Line5 { get; set; }
        /// <summary>
        /// int|null Number of seconds that the message will be displayed. If this parameter is not received, the default
        /// timeout is the IdleTimeout value set through the settings.
        /// </summary>
        public int? Timeout { get; set; }
    }
}
