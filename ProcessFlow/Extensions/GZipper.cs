using Polenter.Serialization;
using System.IO;
using System.IO.Compression;

namespace ProcessFlow.Extensions
{
    public static class GZipperExtensions
    {
        private static readonly SharpSerializer _serializer;
        static GZipperExtensions()
        {
            _serializer = new SharpSerializer(true);
        }

        public static byte[] Zippify(this object obj)
        {
            using var memoryStream = new MemoryStream();
            using var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);

            _serializer.Serialize(obj, gZipStream);
            return memoryStream.ToArray();
        }

        public static T Unzippify<T>(this byte[] byteArray)
        {
            using var memoryStream = new MemoryStream(byteArray);
            using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress, true);

            return (T)_serializer.Deserialize(gZipStream);
        }
    }
}
