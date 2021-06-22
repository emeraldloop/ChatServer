using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


namespace ChatServer
{
    public class ClientObject
    {
        Menu menu = new();
        protected internal string Id { get; private set; }
        public NetworkStream Stream { get; set; }
        string userName;
        string userGroup;
        TcpClient client;
        ServerObject server;
        static private int k=0; //счетчик запросов
        public static int K
        { 
            get { return k; }
            set { k = value; }
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
                Stream = client.GetStream(); //получаем имя и группу пользователя
                string message = GetMessage();
                userName = message;
                message = GetMessage();
                userGroup = message;
                message = userName + " из группы: "+ userGroup + " вошел в чат"; //посылаем сообщение о входе в чат всем пользователям
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message); // в бесконечном цикле получаем сообщения от клиента                
                while (true)
                {
                    //string Id, string userName, string userGroup, ref ServerObject server, ref TcpClient client, ref NetworkStream Stream
                    server.BroadcastBack("\nВведите сообщение/ команду menu", this.Id);
                    try
                    {   
                        message = GetMessage();
                        if (message == "menu") // команды
                        {
                            menu.choosecommand( Id,userName, userGroup, ref server, ref client, Stream);
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
        public string GetMessage()
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
        public byte[] GetHash()
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
            byte[] byteArray = ASCIIEncoding.ASCII.GetBytes(builder.ToString()); // переводим stringbuilder(хеш) в байтовый массив
            return byteArray;
        }
        

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
        
        
    }
}
