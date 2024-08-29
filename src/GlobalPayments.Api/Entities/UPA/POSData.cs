namespace GlobalPayments.Api.Entities.UPA {
    public class POSData {
        public string AppName { get; set; }
        public int? LaunchOrder { get; set; } = 0;
        public bool Remove { get; set; } = false;
        public int? Silent { get; set; } = 0;
    }
}
