namespace Flsurf.Infrastructure.Adapters.FileStorage
{
    public sealed class LimitedStream : Stream
    {
        private readonly Stream _inner;
        private readonly long _limit;
        private long _read;

        public LimitedStream(Stream inner, long limit)
        {
            _inner = inner;
            _limit = limit;
        }

        private void Check(long n)
        {
            _read += n;
            if (_read > _limit)
                throw new InvalidOperationException("Stream exceeds configured limit.");
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct = default)
        {
            var n = await _inner.ReadAsync(buffer, ct);
            Check(n);
            return n;
        }

        public override int Read(byte[] buf, int off, int cnt)
        {
            var n = _inner.Read(buf, off, cnt);
            Check(n);
            return n;
        }

        // остальные члены просто проксируем
        public override void Flush() => _inner.Flush();
        public override long Seek(long o, SeekOrigin s) => _inner.Seek(o, s);
        public override void SetLength(long v) => _inner.SetLength(v);
        public override void Write(byte[] b, int o, int c) => _inner.Write(b, o, c);

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _inner.Length;
        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        protected override void Dispose(bool d)
        {
            if (d) _inner.Dispose();
            base.Dispose(d);
        }
    }
}
