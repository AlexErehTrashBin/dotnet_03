namespace Task03.IO;

public class ThroughputStream
{
    private readonly InputStreamClass _inputStream;
    private readonly OutputStreamClass _outputStream;

    private readonly Queue<byte[]?> _queue = new();

    private readonly EventWaitHandle _queueEvent = new(false, EventResetMode.AutoReset);

    public ThroughputStream()
    {
        _inputStream = new InputStreamClass(this);
        _outputStream = new OutputStreamClass(this);
    }

    public Stream InputStream => _inputStream;

    public Stream OutputStream => _outputStream;

    private class InputStreamClass : Stream
    {
        private readonly Queue<byte[]?> _queue;
        private readonly ThroughputStream _parent;
        private byte[]? _currentBlock;
        private int _currentBlockPos;
        private bool _closed;
        private int _readTimeoutMs = Timeout.Infinite;

        public InputStreamClass(ThroughputStream parent)
        {
            this._parent = parent;
            _queue = parent._queue;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override void Flush()
        {
            // Do nothing, always flushes.
        }

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override bool CanTimeout => true;

        public override int ReadTimeout
        {
            get => _readTimeoutMs;
            set => _readTimeoutMs = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_currentBlock == null)
            {
                int queueCount;
                lock (_queue)
                {
                    queueCount = _queue.Count;
                    if (queueCount > 0) _currentBlock = _queue.Dequeue();
                }

                if (_currentBlock == null && !_parent._outputStream.IsClosed)
                {
                    _parent._queueEvent.WaitOne(_readTimeoutMs);

                    lock (_queue)
                    {
                        if (_queue.Count == 0) return 0;
                        _currentBlock = _queue.Dequeue();
                    }
                }

                _currentBlockPos = 0;
            }

            if (_currentBlock == null) return 0;

            var read = Math.Min(count, _currentBlock.Length - _currentBlockPos);
            Array.Copy(_currentBlock, _currentBlockPos, buffer, offset, read);
            _currentBlockPos += read;
            if (_currentBlockPos != _currentBlock.Length) return read;
            // did read whole block
            _currentBlockPos = 0;
            _currentBlock = null;

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            _closed = true;
            base.Close();
        }
    }

    private class OutputStreamClass : Stream
    {
        private readonly Queue<byte[]?> _queue;
        private readonly ThroughputStream _parent;

        public OutputStreamClass(ThroughputStream parent)
        {
            _parent = parent;
            _queue = parent._queue;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override void Flush()
        {
            // always flush
        }

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var copy = new byte[count];
            Array.Copy(buffer, offset, copy, 0, count);
            lock (_queue)
            {
                _queue.Enqueue(copy);
                try
                {
                    _parent._queueEvent.Set();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public override void Close()
        {
            IsClosed = true;
            base.Close();

            // Signal event, to stop waiting consumer
            try
            {
                _parent._queueEvent.Set();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public bool IsClosed { get; private set; }
    }
}