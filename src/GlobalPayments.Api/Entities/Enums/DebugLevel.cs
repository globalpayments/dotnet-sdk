using System;

namespace GlobalPayments.Api.Entities.Enums {
    [Flags]
    public enum DebugLevel {
        NOLOGS = 0,
        ERROR = 1,
        WARNING = 2,
        FLOW = 4,
        MESSAGE = 8,
        DATA = 16,
        PACKETS = 32,
        PIA = 64,
        PERF = 128
    }
}
