using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HashTablePersistence
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class PersistenceHelper
    {
        private Hashtable hashtable;
        private bool compressFlag;
        public PersistenceHelper(Hashtable table)
        {
            hashtable = table;
            compressFlag = true;
        }
        
        public void EnableCompress()
        {
            compressFlag = true;
        }

        public void DisableCompress()
        {
            compressFlag = false;
        }
        
        protected static byte[] CompressBase(byte[] input)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    gzipStream.Write(input, 0, input.Length);
                    gzipStream.Close();
                    return outStream.ToArray();
                }
            }
        }

        protected static byte[] DecompressBase(byte[] input)
        {

            using (MemoryStream inputStream = new MemoryStream(input))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(outStream);
                        gzipStream.Close();
                        return outStream.ToArray();
                    }
                }

            }
        }

        protected static string Compress(string inputString)
        {
            byte[] inputBytes = Encoding.Default.GetBytes(inputString);
            byte[] outputBytes = CompressBase(inputBytes);
            return Convert.ToBase64String(outputBytes);
        }

        protected static string Decompress(string inputString)
        {
            byte[] inputBytes = Convert.FromBase64String(inputString);
            byte[] outputBytes = DecompressBase(inputBytes);
            return Encoding.Default.GetString(outputBytes);
        }
    }
}
