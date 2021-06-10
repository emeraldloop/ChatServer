using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatServer
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server;
        static private int k=0;
        public static int K
        { 
            get { return k; }
        }
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {   
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream(); //получаем имя пользователя
                string message = GetMessage();
                userName = message;
                message = userName + " вошел в чат"; //посылаем сообщение о входе в чат всем пользователям
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message); // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    server.BroadcastBack("Введите сообщение/ команду menu", this.Id);
                    try
                    {   
                        message = GetMessage();
                        if (message == "menu") // команды
                        {
                            menu();
                        }
                        else  //чат
                        {
                            message = String.Format("{0}: {1}", userName, message);
                            Console.WriteLine(message);
                            server.BroadcastMessage(message, this.Id);
                        }
                       
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            k++;
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


        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
        private void menu()
        {   
            server.BroadcastBack("Введите команду", this.Id);

            while (true)
            {
                string command = GetMessage();

                if (command == "1")
                {
                    command = String.Format("{0}: - число запросов", k);
                    Console.WriteLine(command);
                    server.BroadcastBack(command, this.Id);
                    break;
                }

                if (command == "2")
                {
                    k = 0;
                    break;
                }
                if (command == "3")
                {
                    server.BroadcastBack(userName + " - имя пользователя", this.Id);
                    break;
                }
               
                if (command == "5")
                {
                    server.BroadcastBack("Введите строку для изменения", this.Id);
                    char[] array = GetMessage().ToCharArray();
                    Array.Reverse(array);
                    string str = new string(array);
                    server.BroadcastBack(str, this.Id);
                    break;
                }
            }
        }

        






    }
}
