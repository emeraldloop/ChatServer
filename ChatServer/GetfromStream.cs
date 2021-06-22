using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace ChatServer
{
    class GetfromStream
    {
        public string GetMessage(NetworkStream Stream)
        {

            ClientObject.K++;
            byte[] data = new byte[64]; //буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        public string GetHashfromClient(NetworkStream Stream)
        {
            ClientObject.K++;
            byte[] data = new byte[64]; //буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(bytes.ToString());
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        public string HashMessage(string message)
        {
            
            byte[] hash2sent;
            UnicodeEncoding ue = new();
            byte[] messageBytes = ue.GetBytes(message); // переводим строку в байты

            SHA256 shHash = SHA256.Create();
            hash2sent = shHash.ComputeHash(messageBytes); // переводим байты в хеш

        }



    }
}
