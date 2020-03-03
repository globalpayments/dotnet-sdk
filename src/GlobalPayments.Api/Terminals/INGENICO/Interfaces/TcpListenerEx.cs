using System.Net;
using System.Net.Sockets;

namespace GlobalPayments.Api.Terminals.INGENICO {
    public class TcpListenerEx : TcpListener {
        public TcpListenerEx(IPAddress localaddr, int port) : base(localaddr, port) {

        }

        public new bool Active { get { return base.Active; } }
    }
}
