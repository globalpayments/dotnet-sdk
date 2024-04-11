using System;
using System.Threading;
using System.Threading.Tasks;

namespace GlobalPayments.Api.Terminals.UPA
{
    public class StreamBuffer
    {
        #region Private data

        private const int DefaultCapacity = 1024;

        private readonly object _syncObj = new object();

        private byte[] _buffer = new byte[DefaultCapacity];

        private int _head;

        private int _tail = -1;

        private bool _isEmpty = true;

        private TaskCompletionSource<bool> _dequeueManualTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        private TaskCompletionSource<bool> _availableTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        #endregion Private data

        #region Constructors

        public StreamBuffer()
        {
        }

        public StreamBuffer(byte[] bytes)
        {
            Enqueue(bytes);
        }

        public StreamBuffer(int capacity)
        {
            AutoTrimMinCapacity = capacity;
            SetCapacity(capacity);
        }

        #endregion Constructors

        #region Properties

        public int Count
        {
            get
            {
                lock (_syncObj)
                {
                    if (_isEmpty)
                    {
                        return 0;
                    }
                    if (_tail >= _head)
                    {
                        return _tail - _head + 1;
                    }
                    return Capacity - _head + _tail + 1;
                }
            }
        }

        public byte[] Buffer
        {
            get
            {
                lock (_syncObj)
                {
                    var bytes = new byte[Count];
                    if (!_isEmpty)
                    {
                        if (_tail >= _head)
                        {
                            Array.Copy(_buffer, _head, bytes, 0, _tail - _head + 1);
                        }
                        else
                        {
                            Array.Copy(_buffer, _head, bytes, 0, Capacity - _head);
                            Array.Copy(_buffer, 0, bytes, Capacity - _head, _tail + 1);
                        }
                    }
                    return bytes;
                }
            }
        }

        public int Capacity => _buffer.Length;

        public bool AutoTrim { get; set; } = true;

        public int AutoTrimMinCapacity { get; set; } = DefaultCapacity;

        #endregion Properties

        #region Public methods

        public void Clear()
        {
            lock (_syncObj)
            {
                _head = 0;
                _tail = -1;
                _isEmpty = true;
                Reset(ref _availableTcs);
            }
        }

        public void SetCapacity(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "The capacity must not be negative.");

            lock (_syncObj)
            {
                var count = Count;
                if (capacity < count)
                    throw new ArgumentOutOfRangeException(nameof(capacity), "The capacity is too small to hold the current buffer content.");

                if (capacity == _buffer.Length) 
                    return;

                var newBuffer = new byte[capacity];
                Array.Copy(Buffer, newBuffer, count);
                _buffer = newBuffer;
                _head = 0;
                _tail = count - 1;
            }
        }

        public void TrimExcess()
        {
            lock (_syncObj)
            {
                if (Count < Capacity * 0.9)
                {
                    SetCapacity(Count);
                }
            }
        }

        public void Enqueue(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            Enqueue(bytes, 0, bytes.Length);
        }

        public void Enqueue(ArraySegment<byte> segment)
        {
            Enqueue(segment.Array, segment.Offset, segment.Count);
        }

        public void Enqueue(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (offset + count > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return;   // Nothing to do

            lock (_syncObj)
            {
                if (Count + count > Capacity)
                {
                    SetCapacity(Math.Max(Capacity * 2, Count + count));
                }

                int tailCount;
                int wrapCount;
                if (_tail >= _head || _isEmpty)
                {
                    tailCount = Math.Min(Capacity - 1 - _tail, count);
                    wrapCount = count - tailCount;
                }
                else
                {
                    tailCount = Math.Min(_head - 1 - _tail, count);
                    wrapCount = 0;
                }

                if (tailCount > 0)
                {
                    Array.Copy(bytes, offset, _buffer, _tail + 1, tailCount);
                }
                if (wrapCount > 0)
                {
                    Array.Copy(bytes, offset + tailCount, _buffer, 0, wrapCount);
                }
                _tail = (_tail + count) % Capacity;
                _isEmpty = false;
                Set(_dequeueManualTcs);
                Set(_availableTcs);
            }
        }

        public byte[] Dequeue(int maxCount)
        {
            return DequeueInternal(maxCount, peek: false);
        }

        public int Dequeue(byte[] buffer, int offset, int maxCount)
        {
            return DequeueInternal(buffer, offset, maxCount, peek: false);
        }

        public byte[] Peek(int maxCount)
        {
            return DequeueInternal(maxCount, peek: true);
        }

        public async Task<byte[]> DequeueAsync(int count, CancellationToken cancellationToken = default)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "The count must not be negative.");

            while (true)
            {
                TaskCompletionSource<bool> myDequeueManualTcs;
                lock (_syncObj)
                {
                    if (count <= Count)
                    {
                        return Dequeue(count);
                    }
                    myDequeueManualTcs = Reset(ref _dequeueManualTcs);
                }
                await AwaitAsync(myDequeueManualTcs, cancellationToken);
            }
        }

        public async Task DequeueAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "The count must not be negative.");
            if (buffer.Length < offset + count)
                throw new ArgumentException("The buffer is too small for the requested data.", nameof(buffer));

            while (true)
            {
                TaskCompletionSource<bool> myDequeueManualTcs;
                lock (_syncObj)
                {
                    if (count <= Count)
                    {
                        Dequeue(buffer, offset, count);
                    }
                    myDequeueManualTcs = Reset(ref _dequeueManualTcs);
                }
                await AwaitAsync(myDequeueManualTcs, cancellationToken);
            }
        }

        public async Task WaitAsync(CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<bool> myAvailableTcs;
            lock (_syncObj)
            {
                if (Count > 0)
                {
                    return;
                }
                myAvailableTcs = Reset(ref _availableTcs);
            }
            await AwaitAsync(myAvailableTcs, cancellationToken);
        }

        #endregion Public methods

        #region Private methods

        private byte[] DequeueInternal(int count, bool peek)
        {
            if (count > Count)
                count = Count;
            var bytes = new byte[count];
            DequeueInternal(bytes, 0, count, peek);
            return bytes;
        }

        private int DequeueInternal(byte[] bytes, int offset, int count, bool peek)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "The count must not be negative.");
            if (count == 0)
                return count;   // Easy
            if (bytes.Length < offset + count)
                throw new ArgumentException("The buffer is too small for the requested data.", nameof(bytes));

            lock (_syncObj)
            {
                if (count > Count)
                    count = Count;

                if (_tail >= _head)
                {
                    Array.Copy(_buffer, _head, bytes, offset, count);
                }
                else
                {
                    if (count <= Capacity - _head)
                    {
                        Array.Copy(_buffer, _head, bytes, offset, count);
                    }
                    else
                    {
                        var headCount = Capacity - _head;
                        Array.Copy(_buffer, _head, bytes, offset, headCount);
                        var wrapCount = count - headCount;
                        Array.Copy(_buffer, 0, bytes, offset + headCount, wrapCount);
                    }
                }

                if (peek) 
                    return count;

                if (count == Count)
                {
                    _isEmpty = true;
                    _head = 0;
                    _tail = -1;
                    Reset(ref _availableTcs);
                }
                else
                {
                    _head = (_head + count) % Capacity;
                }

                if (!AutoTrim || Capacity <= AutoTrimMinCapacity || Count > Capacity / 2) 
                    return count;

                var newCapacity = Count;
                if (newCapacity < AutoTrimMinCapacity)
                {
                    newCapacity = AutoTrimMinCapacity;
                }
                if (newCapacity < Capacity)
                {
                    SetCapacity(newCapacity);
                }
                return count;
            }
        }

        // Must be called within the lock
        private static TaskCompletionSource<bool> Reset(ref TaskCompletionSource<bool> tcs)
        {
            if (tcs.Task.IsCompleted)
            {
                tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            }
            return tcs;
        }

        // Must be called within the lock
        private static void Set(TaskCompletionSource<bool> tcs)
        {
            tcs.TrySetResult(true);
        }

        // Must NOT be called within the lock
        private static async Task AwaitAsync(TaskCompletionSource<bool> tcs, CancellationToken cancellationToken)
        {
            if (await Task.WhenAny(tcs.Task, Task.Delay(-1, cancellationToken)) == tcs.Task)
            {
                await tcs.Task;   // Already completed
                return;
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

        #endregion Private methods
    }
}