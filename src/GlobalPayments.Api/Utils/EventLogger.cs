namespace GlobalPayments.Api.Utils {
    internal delegate void LoggingEventHandler(string message);

    internal class EventLogger {
        private static EventLogger _instance;

        public event LoggingEventHandler OnInfo;
        public event LoggingEventHandler OnDebug;
        public event LoggingEventHandler OnError;

        public static EventLogger Instance {
            get {
                if (_instance == null)
                    _instance = new EventLogger();
                return _instance;
            }
        }

        public void Info(string message) {
            OnInfo?.Invoke(message);
        }
        public void Debug(string message) {
            OnDebug?.Invoke(message);
        }
        public void Error(string message) {
            OnError?.Invoke(message);
        }
    }
}
