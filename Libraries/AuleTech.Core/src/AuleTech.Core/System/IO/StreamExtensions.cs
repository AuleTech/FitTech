using System.Runtime.CompilerServices;
using System.Text;
using AuleTech.Core.System.IO.FileSystem.Files;

namespace AuleTech.Core.System.IO;

public static class StreamExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream CopyToMemory(this Stream src)
    {
        if (src.CanSeek)
        {
            src.Seek(0, SeekOrigin.Begin);
        }

        var memoryStream = new MemoryStream();
        src.CopyTo(memoryStream);
        src.Flush();
        memoryStream.Position = 0;
        return memoryStream;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream ToStream(this byte[] byteArray)
    {
        if (byteArray == null)
        {
            throw new ArgumentNullException(nameof(byteArray));
        }

        var stream = new MemoryStream();
        stream.Write(byteArray, 0, byteArray.Length);

        stream.Position = 0;

        return stream;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadAllBytes(this Stream src)
    {
        byte[] result;
        if (src is MemoryStream stream)
        {
            result = stream.ToArray();
        }
        else
        {
            using var memoryStream = new MemoryStream();
            src.CopyTo(memoryStream);
            result = memoryStream.ToArray();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<byte[]> ReadAllBytesAsync(this Stream src, CancellationToken cancellationToken = default)
    {
        if (src.CanSeek)
        {
            src.Seek(0, SeekOrigin.Begin);
        }

        using (var memoryStream = new MemoryStream())
        {
            await src.CopyToAsync(memoryStream, 1024, cancellationToken);
            return memoryStream.ToArray();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<string> ReadAllTextAsync(this Stream stream)
    {
        return stream.ReadAllTextAsync(SystemIoFileProxy.DefaultUtf8Encoding);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<string> ReadAllTextAsync(this Stream stream
        , Encoding encoding)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        using var reader = new StreamReader(stream, encoding, true, 4096, true);
        return await reader.ReadToEndAsync();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> ReadLines(this Stream stream
        , bool closeStream = false)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using (var reader = new StreamReader(stream))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        if (closeStream)
        {
            stream.Close();
            stream.Dispose();
        }
    }
}
