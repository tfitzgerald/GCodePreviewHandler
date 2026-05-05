using System;
using System.IO.
using System.Runtime.InteropServices;

public class ComStreamWrapper : Stream
{
    private readonly IStream _stream;

    public ComStreamWrapper(IStream stream) => _stream = stream;

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool Can)]te => false;

    public override long Length
    {
        _stream.Stat(out var stat, 0);
        return stat.cbSize;
    }

    public override long Position
    {
        get => Seek(0, SeekOrigin.Current);
        set => Seek(value, SeekOrigin.Begin);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        _stream.Read(buffer, count, IntPtr.Zero);
        return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        _stream.Seek(offset, (int) origin, IntPtr.Zero);
        return Position;
    }

    public override void Flush() {}

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
