using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

namespace ChatClient
{
    class Program
    {
        static string userName;
        static string group;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;
        

        static void Main(string[] args)
        {
            
            Console.Write("Введите своё имя: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //подключение клиента
                stream = client.GetStream();  // получаем поток

                string message = userName+"_"+ userName.Length.ToString() + "_"+ GetHashMessage(userName);
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length); //отправили имя + _длину имени_ + хеш имени

                Console.Write("Введите свою группу: ");
                group = Console.ReadLine();
                message = group + "_" + group.Length.ToString() + "_" + GetHashMessage(group);

                data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length); // отправили группу + _длину группы_ + хеш группы



                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); // старт потока
                Console.WriteLine("Добро пожаловать, {0} из группы: {1}", userName, group);
                SendMessage();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        //отправка сообщений
        static void SendMessage()
        {
            

            while (true)
            {
                string message = Console.ReadLine();
                message = message + "_" + message.Length.ToString() + "_" + GetHashMessage(message); 
                byte[] data = Encoding.Unicode.GetBytes(message); // отправили сообщение + _длину сообщения_ + хеш сообщения
                stream.Write(data, 0, data.Length); // отправка сообщения
               

            }
        }
        // получение сообщений
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; //буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    string message = builder.ToString();
                    Console.WriteLine(message); //вывод сообщения

                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close(); // отключение потока
            if (client != null)
                client.Close(); // отключение клиента 
            Environment.Exit(0); // завершение процесса
        }

        public static string GetHashMessage(string message)
        {

            byte[] newhash;
            UnicodeEncoding ue = new();
            byte[] messageBytes = ue.GetBytes(message); // переводим строку в байты

            SHA256 shHash = SHA256.Create();
            newhash = shHash.ComputeHash(messageBytes); // переводим байты в хеш

            StringBuilder builder = new StringBuilder();

            foreach (byte b in newhash)
            {
                builder.Append(b + " ");
            }
            string hash = builder.ToString();
            hash.Remove(hash.Length - 1);
            return hash;
        }

    }
}
