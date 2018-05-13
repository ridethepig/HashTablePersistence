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
            Hashtable hashtable = new Hashtable();
            hashtable.Add("ha", "ha");
            PersistenceHelper helper = new PersistenceHelper(hashtable, "table");
            helper.Write();
        }
    }

    class PersistenceHelper
    {
        protected Hashtable hashtable;
        protected BinaryReader binaryReader;
        protected BinaryWriter binaryWriter;
        protected bool compressFlag;
        protected string fileName;
        public PersistenceHelper(Hashtable table, string fileName)
        {
            this.hashtable = table;
            this.compressFlag = true;
            this.fileName = fileName;
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

        protected static void CreateDirectory(string directoryPath)
        {            
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public void Write()
        {
            CreateDirectory(".\\data");
            try
            {
                binaryWriter = new BinaryWriter(new FileStream(@".\data\" + fileName, FileMode.OpenOrCreate));
            }
            catch (IOException e)
            {
                Console.WriteLine("Err" + e.Message);
                binaryWriter.Close();
                return;
            }
            binaryWriter.Write("MEGUMI");
            binaryWriter.Write(hashtable.Count);
            binaryWriter.Write(compressFlag);
            string Key, Value;
            foreach (DictionaryEntry itr in hashtable)
            {
                Key = (string)itr.Key;
                Value = (string)itr.Value;
                binaryWriter.Write(Key);
                if (compressFlag)
                {
                    Value = Compress(Value);
                }
                binaryWriter.Write(Value);
            }
            binaryWriter.Close();
        }
    }
}
