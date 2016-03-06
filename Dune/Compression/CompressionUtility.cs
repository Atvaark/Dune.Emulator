using System.IO;
using Dune.Compression.Zlib;

namespace Dune.Compression
{
    public static class CompressionUtility
    {
        public static byte[] Decompress(byte[] data)
        {
            using (var outputStream = new MemoryStream())
            using (var decompressionStream = new ZlibStream(outputStream, CompressionMode.Decompress))
            using (var inputStream = new MemoryStream(data))
            {
                inputStream.WriteTo(decompressionStream);
                return outputStream.ToArray();
            }
        }

        public static byte[] Compress(byte[] data)
        {
            using (var inputStream = new MemoryStream(data))
            using (var compressionStream = new ZlibStream(inputStream, CompressionMode.Compress))
            using (var outputStream = new MemoryStream())
            {
                compressionStream.WriteTo(outputStream);
                return outputStream.ToArray();
            }
        }

        private static void WriteTo(this Stream inputStream, Stream outputStream)
        {
            byte[] buffer = new byte[2048];
            int bytesRead;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
            }
        }
    }
}