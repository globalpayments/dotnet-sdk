using System;

namespace GlobalPayments.Api.Entities {
    public class StoredCredential {
        public StoredCredentialType Type { get; set; }
        public StoredCredentialInitiator Initiator { get; set; }
        public StoredCredentialSequence Sequence { get; set; }
        public StoredCredentialReason Reason { get; set; }
        public string SchemeId { get; set; }
    }
}
