using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustchainCore.Extensions
{
    public static class MemoryStreamExtensions
    {
        public static void WriteBytes(this MemoryStream ms, byte[] data)
        {
            ms.Write(data, 0, data.Length);
        }
        /// <summary>
        /// Write a string in UTF8 format to the memoryStream
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="text"></param>
        public static void WriteString(this MemoryStream ms, string text)
        {
            ms.WriteBytes(Encoding.UTF8.GetBytes(text));
        }

        public static void WriteInteger(this MemoryStream ms, int num)
        {
            var bytes = BitConverter.GetBytes(num);
            ms.WriteBytes(bytes);
        }

        public static void WriteInteger(this MemoryStream ms, uint num)
        {
            var bytes = BitConverter.GetBytes(num);
            ms.WriteBytes(bytes);
        }

        public static void WriteLong(this MemoryStream ms, long num)
        {
            var bytes = BitConverter.GetBytes(num);
            ms.WriteBytes(bytes);
        }

    }
}
