using System;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Terminals.Builders;

namespace GlobalPayments.Api.Terminals.Genius {
    internal class GeniusInterface : DeviceInterface<GeniusController> {
        internal GeniusInterface(GeniusController controller) : base(controller) {
        }
    }
}
