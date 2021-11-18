using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Terminals {
    public enum ControlCodes : byte {
        STX = 0x02,
        ETX = 0x03,
        ACK = 0x06,
        NAK = 0x15,
        ENQ = 0x05,
        FS = 0x1C,
        GS = 0x1D,
        EOT = 0x04,

        // PAX Specific ??
        US = 0x1F,
        RS = 0x1E,
        COMMA = 0x2C,
        COLON = 0x3A,
        PTGS = 0x7C,
        LF = 0x0A
    }
}
