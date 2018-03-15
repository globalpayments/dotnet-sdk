namespace GlobalPayments.Api {
    public abstract class Configuration {
        private int _timeout = 65000;

        /// <summary>
        /// Gateway service URL
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Timeout value for gateway communication (in milliseconds)
        /// </summary>
        public int Timeout { get { return _timeout; } set { _timeout = value; } }

        internal bool Validated { get; private set; }

        internal abstract void ConfigureContainer(ConfiguredServices services);

        internal virtual void Validate() {
            Validated = true;
        }
    }
}
