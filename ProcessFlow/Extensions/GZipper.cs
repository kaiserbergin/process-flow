using System.IO;
using System.IO.Compression;

namespace ProcessFlow.Extensions
{
    public static class GZipperExtensions
    {
        public static byte[] Zippify(this object objectToZip)
        {
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(objectToZip);
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            using (var memory = new MemoryStream())
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Compress))
                {
                    gzip.Write(bytes, 0, bytes.Length);
                }

                return memory.ToArray();
            }
        }

        public static T Unzippify<T>(this byte[] bytes)
        {
            var jsonData = string.Empty;

            using (var stream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
            {
                const int size = 4096;
                var buffer = new byte[size];

                using (var memory = new MemoryStream())
                {
                    var count = 0;

                    do
                    {
                        count = stream.Read(buffer, 0, size);

                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);

                    jsonData = System.Text.Encoding.UTF8.GetString(memory.ToArray());
                }
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
