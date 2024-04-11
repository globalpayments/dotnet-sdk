using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class TcpClientAsync : IDisposable
    {
        #region Private data

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private TaskCompletionSource<bool> _closedTcs = new TaskCompletionSource<bool>();

        #endregion Private data

        #region Constructors

        public TcpClientAsync()
        {
            _closedTcs.SetResult(true);
        }

        #endregion Constructors

        #region Events

        public event EventHandler<MessageEventArgs> Message;

        #endregion Events

        #region Properties

        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);

        public TimeSpan MaxConnectTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public bool AutoReconnect { get; set; }

        public string HostName { get; set; }

        public IPAddress IpAddress { get; set; }

        public int Port { get; set; }

        public bool IsConnected => _tcpClient.Client.Connected;

        public StreamBuffer StreamBuffer { get; private set; } = new StreamBuffer();

        public Task ClosedTask => _closedTcs.Task;

        public bool IsClosing => ClosedTask.IsCompleted;

        public Func<TcpClientAsync, bool, Task> ConnectedCallback { get; set; }

        public Action<TcpClientAsync, bool> ClosedCallback { get; set; }

        public Func<TcpClientAsync, int, Task> ReceivedCallback { get; set; }

        #endregion Properties

        #region Public methods

        public async Task RunAsync()
        {
            var isReconnected = false;
            var reconnectTry = -1;
            do
            {
                reconnectTry++;
                StreamBuffer = new StreamBuffer();

                // Try to connect to remote host
                var connectTimeout = TimeSpan.FromTicks(ConnectTimeout.Ticks +
                                                        (MaxConnectTimeout.Ticks - ConnectTimeout.Ticks) / 20 *
                                                        Math.Min(reconnectTry, 20));
                _tcpClient = new TcpClient(AddressFamily.InterNetworkV6)
                {
                    SendTimeout = 2000,
                    ReceiveTimeout = 2000,
                    LingerState = new LingerOption(true, 0)
                };
                _tcpClient.Client.DualMode = true;
                Message?.Invoke(this, new MessageEventArgs("Connecting to server"));
                var connectTask = !string.IsNullOrWhiteSpace(HostName) ? _tcpClient.ConnectAsync(HostName, Port) : _tcpClient.ConnectAsync(IpAddress, Port);

                var timeoutTask = Task.Delay(connectTimeout);
                if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
                {
                    Message?.Invoke(this, new MessageEventArgs("Connection timeout"));
                    continue;
                }

                try
                {
                    await connectTask;
                }
                catch (Exception ex)
                {
                    Message?.Invoke(this, new MessageEventArgs("Error connecting to remote host", ex));
                    await timeoutTask;
                    continue;
                }

                reconnectTry = -1;
                _stream = _tcpClient.GetStream();
                _stream.ReadTimeout = 1000 * 3;

                // Read until the connection is closed.
                var networkReadTask = Task.Run(async () =>
                {
                    var buffer = new byte[10240];
                    while (true)
                    {
                        int readLength;
                        try
                        {
                            var readTask = _stream.ReadAsync(buffer, 0, buffer.Length);

                            readLength = await readTask;
                        }
                        catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode == (int)SocketError.OperationAborted)
                        {
                            Message?.Invoke(this, new MessageEventArgs("Connection closed locally", ex));
                            readLength = -1;
                        }
                        catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode == (int)SocketError.ConnectionAborted)
                        {
                            Message?.Invoke(this, new MessageEventArgs("Connection aborted", ex));
                            readLength = -1;
                        }
                        catch (IOException ex) when ((ex.InnerException as SocketException)?.ErrorCode == (int)SocketError.ConnectionReset)
                        {
                            Message?.Invoke(this, new MessageEventArgs("Connection reset remotely", ex));
                            readLength = -2;
                        }
                        catch (ObjectDisposedException)
                        {
                            Message?.Invoke(this, new MessageEventArgs("Connection closed by client"));
                            readLength = -4;
                        }
                        catch (Exception ex)
                        {
                            Message?.Invoke(this, new MessageEventArgs(ex.Message, ex));
                            readLength = -2;
                        }

                        if (readLength <= 0)
                        {
                            if (readLength == 0)
                            {
                                Message?.Invoke(this, new MessageEventArgs("Connection closed remotely"));
                            }

                            _closedTcs.TrySetResult(true);
                            OnClosed(readLength != -1);
                            return;
                        }

                        var segment = new ArraySegment<byte>(buffer, 0, readLength);
                        StreamBuffer.Enqueue(segment);
                        await OnReceivedAsync(readLength);
                    }
                });

                _closedTcs = new TaskCompletionSource<bool>();
                await OnConnectedAsync(isReconnected);

                // Wait for closed connection
                await networkReadTask;
                _tcpClient.Close();

                isReconnected = true;
            } while (AutoReconnect);
        }

        public void Disconnect()
        {
            _tcpClient.Client.Shutdown(SocketShutdown.Send);
            var read = 0;
            try
            {
                var buffer = new byte[10240];
                while ((read = _tcpClient.Client.Receive(buffer)) > 0)
                {

                }
            }
            catch
            {
                Message?.Invoke(this, new MessageEventArgs($"Connection Disconnected {read}"));
            }

            _tcpClient?.Client?.Close();
            _stream.Close(0);
            _stream.Dispose();
        }

        public void Dispose()
        {
            AutoReconnect = false;
            _tcpClient?.Client?.Dispose();
            _tcpClient?.Dispose();
            _stream = null;
            _tcpClient = null;
        }

        public async Task Send(ArraySegment<byte> data, CancellationToken cancellationToken = default)
        {
            if (!_tcpClient.Client.Connected)
                throw new InvalidOperationException("Not connected.");

            await _stream.WriteAsync(data.Array, data.Offset, data.Count, cancellationToken);
        }

        #endregion Public methods

        #region Protected virtual methods

        protected virtual Task OnConnectedAsync(bool isReconnected)
        {
            return ConnectedCallback != null ? ConnectedCallback(this, isReconnected) : Task.CompletedTask;
        }

        protected virtual void OnClosed(bool remote)
        {
            ClosedCallback?.Invoke(this, remote);
        }

        protected virtual Task OnReceivedAsync(int count)
        {
            return ReceivedCallback != null ? ReceivedCallback(this, count) : Task.CompletedTask;
        }

        #endregion Protected virtual methods
    }
}