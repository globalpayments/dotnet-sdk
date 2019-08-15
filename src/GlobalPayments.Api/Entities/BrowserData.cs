using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Entities {
    public class BrowserData {
        public string AcceptHeader { get; set; }
        public ColorDepth ColorDepth { get; set; }
        public string IpAddress { get; set; }
        public bool JavaEnabled { get; set; }
        public bool JavaScriptEnabled { get; set; }
        public string Language { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }
        public ChallengeWindowSize ChallengeWindowSize { get; set; }
        public string Timezone { get; set; }
        public string UserAgent { get; set; }
    }
}
