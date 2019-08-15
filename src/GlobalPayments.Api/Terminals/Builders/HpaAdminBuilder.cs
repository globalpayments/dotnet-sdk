using System.Text;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Terminals.Builders {
    public class HpaAdminBuilder {
        private StringBuilder messageBuilder;

        internal bool AwaitResponse;
        internal bool KeepAlive { get; set; }
        internal string[] MessageIds { get; set; }

        public HpaAdminBuilder(params string[] messageIds) {
            if (messageIds.Length <= 0) {
                throw new BuilderException("You must provide at least one message id.");
            }

            messageBuilder = new StringBuilder();
            messageBuilder.Append(string.Format("<SIP><Version>1.0</Version><ECRId>1004</ECRId><Request>{0}</Request>", messageIds[0]));

            MessageIds = messageIds;
            KeepAlive = false;
            AwaitResponse = true;
        }

        public HpaAdminBuilder Set(string tagName, int value) {
            if (value != default(int)) {
                messageBuilder.Append(string.Format("<{0}>{1}</{2}>", tagName, value, tagName));
            }
            return this;
        }

        public HpaAdminBuilder Set(string tagName, string value) {
            if (!string.IsNullOrEmpty(value)) {
                messageBuilder.Append(string.Format("<{0}>{1}</{2}>", tagName, value, tagName));
            }
            return this;
        }

        public string BuildMessage() {
            messageBuilder.Append("</SIP>");
            return messageBuilder.ToString();
        }
    }
}
