#if NETSTANDARD2_0
namespace Betalgo.Anthropic.Extensions
{
    public class AsyncDisposableStream : Stream, IAsyncDisposable
    {
        private readonly Stream _innerStream;

        /// <inheritdoc />
        public AsyncDisposableStream(Stream stream)
        {
            _innerStream = stream;
        }

        /// <inheritdoc />
        public override bool CanRead => _innerStream.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => _innerStream.CanSeek;

        /// <inheritdoc />
        public override bool CanTimeout => _innerStream.CanTimeout;

        /// <inheritdoc />
        public override bool CanWrite => _innerStream.CanWrite;

        /// <inheritdoc />
        public override long Length => _innerStream.Length;

        /// <inheritdoc />
        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        /// <inheritdoc />
        public override int ReadTimeout
        {
            get => _innerStream.ReadTimeout;
            set => _innerStream.ReadTimeout = value;
        }

        /// <inheritdoc />
        public override int WriteTimeout
        {
            get => _innerStream.WriteTimeout;
            set => _innerStream.WriteTimeout = value;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (_innerStream != null)
            {
                if (_innerStream is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    await Task.Run(() => _innerStream.Dispose()).ConfigureAwait(false);
                }
            }
        }

        public new Task CopyToAsync(Stream destination)
        {
            return _innerStream.CopyToAsync(destination);
        }

        public new Task CopyToAsync(Stream destination, int bufferSize)
        {
            return _innerStream.CopyToAsync(destination, bufferSize);
        }

        /// <inheritdoc />
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public new void CopyTo(Stream destination)
        {
            _innerStream.CopyTo(destination);
        }

        public new void CopyTo(Stream destination, int bufferSize)
        {
            _innerStream.CopyTo(destination, bufferSize);
        }

        /// <inheritdoc />
        public override void Close()
        {
            _innerStream.Close();
        }

        public new void Dispose()
        {
            _innerStream.Dispose();
        }

        /// <inheritdoc />
        public override void Flush()
        {
            _innerStream.Flush();
        }

        public new Task FlushAsync()
        {
            return _innerStream.FlushAsync();
        }

        /// <inheritdoc />
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _innerStream.FlushAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <inheritdoc />
        public override int EndRead(IAsyncResult asyncResult)
        {
            return _innerStream.EndRead(asyncResult);
        }

        public new Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return _innerStream.ReadAsync(buffer, offset, count);
        }

        /// <inheritdoc />
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc />
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <inheritdoc />
        public override void EndWrite(IAsyncResult asyncResult)
        {
            _innerStream.EndWrite(asyncResult);
        }

        public new Task WriteAsync(byte[] buffer, int offset, int count)
        {
            return _innerStream.WriteAsync(buffer, offset, count);
        }

        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        /// <inheritdoc />
        public override int ReadByte()
        {
            return _innerStream.ReadByte();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }

        /// <inheritdoc />
        public override void WriteByte(byte value)
        {
            _innerStream.WriteByte(value);
        }
    }
}
#endif