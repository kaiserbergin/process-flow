using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProcessFlow.Extensions
{
    public static class GZipperExtensions
    {
        public static byte[] Zippify(this object obj)
        {
            using var memoryStream = new MemoryStream();
            using var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);

            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(gZipStream, obj);
            return memoryStream.ToArray();
        }

        public static T Unzippify<T>(this byte[] byteArray)
        {
            using var memoryStream = new MemoryStream(byteArray);
            using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress, true);

            var binaryFormatter = new BinaryFormatter();
            return (T)binaryFormatter.Deserialize(gZipStream);
        }
    }
}
