#define Debug
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
            PersistenceHelper helper = new PersistenceHelper("table");
            helper.Write(hashtable,"haha");
        }
    }

    class PersistenceHelper
    {        
        protected BinaryReader binaryReader;
        protected BinaryWriter binaryWriter;
        protected bool compressFlag;
        protected string path;

        public void SetPath(string newPath)
        {
            if (newPath[newPath.Length - 1] != '\\')
            {
#if Debug
                throw new Exception("Err \'cause your path is ileagal; there should be a \\ at the end of it");
#else
                Console.WriteLine("Err \'cause your path is ileagal; there should be a \\ at the end of it");
                return;
#endif
            }
            path = newPath;
        }

        public PersistenceHelper(string fileName)
        {     
            this.compressFlag = true;
            path = @".\data\";
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

        public void Write(Hashtable hashtable, string fileName)
        {
            CreateDirectory(path);
            try
            {
                binaryWriter = new BinaryWriter(new FileStream(path + fileName, FileMode.OpenOrCreate));
            }
            catch (IOException e)
            {
#if Debug
                throw e;
#else
                Console.WriteLine("Err:" + e.Message);
                binaryWriter.Close();
                return;
#endif            
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
            binaryWriter = null;
        }        

        public void Read(ref Hashtable hashtable, string fileName)
        {
            if (!File.Exists(path + fileName))
            {
#if Debug               
                throw new Exception("No such File:" + path + fileName);
#else
                Console.WriteLine("No such File:{0}", path + fileName);
                return;
#endif            
            }
            try
            {
                binaryReader = new BinaryReader(new FileStream(path + fileName, FileMode.Open));
            }
            catch (IOException e)
            {
#if Debug
                throw e;
#else
                Console.WriteLine("Err:" + e.Message);
                binaryreader.Close();
                return;
#endif
            }
            string sign = binaryReader.ReadString();
            if (sign != "MEGUMI")
            {
                throw new Exception("not my wife");
            }
            int tableSize = binaryReader.ReadInt32();
            bool compressflg = binaryReader.ReadBoolean();
            string key, value;
            for (int i = 1;i <= tableSize; ++ i)
            {
                key = binaryReader.ReadString();
                value = binaryReader.ReadString();
                if (compressflg)
                {
                    value = Decompress(value);
                }
                hashtable.Add(key, value);
            }
            binaryReader.Close();
            binaryReader = null;
        }
    }
}
