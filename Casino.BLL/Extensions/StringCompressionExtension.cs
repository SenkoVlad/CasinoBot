﻿using System.IO.Compression;
using System.Text;

namespace Casino.BLL.Extensions;

public static class StringCompressionExtension
{
    public static string Compress(this string uncompressedString)
    {
        byte[] compressedBytes;

        using var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString));
        using var compressedStream = new MemoryStream();
        using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.SmallestSize, true))
            uncompressedStream.CopyTo(compressorStream);
        
        compressedBytes = compressedStream.ToArray();

        return Convert.ToBase64String(compressedBytes);
    }


    public static string Decompress(this string compressedString)
    {
        byte[] decompressedBytes;

        var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

        using var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
            decompressorStream.CopyTo(decompressedStream);
            decompressedBytes = decompressedStream.ToArray();

        return Encoding.UTF8.GetString(decompressedBytes);
    }
}