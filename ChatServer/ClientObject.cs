using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

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

                                                       // Ниже идут вспомогательные методы

        
        public string GetMessage()
        {
            try
            {
                //чтение входящего сообщения и преобразование в строку (с проверкой!)
                k++; //счётчик запросов для команды №1 из меню

                byte[] data = new byte[64]; //буфер для получаемых данных
                StringBuilder builder = new StringBuilder();
                int bytes = 0;

                do              // получаем сообщение
                {
                    bytes = Stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (Stream.DataAvailable);
                string fullmessage = builder.ToString(); // полученное сообщение вместе с длинной и хешем


                builder.Clear();         
                for (int i = 0; i <fullmessage.IndexOf("_");i++)
                {
                    builder.Append(fullmessage[i]);
                }
                string message = builder.ToString(); //беру сообщение из fullmessage

                builder.Clear();
                for (int i = fullmessage.IndexOf("_")+1; i < fullmessage.LastIndexOf("_"); i++)
                {
                    builder.Append(fullmessage[i]);
                }
                string receivedLength = builder.ToString(); //беру длину из fullmessage

                builder.Clear();
                for (int i = fullmessage.LastIndexOf("_") + 1; i < fullmessage.Length; i++)
                {
                    builder.Append(fullmessage[i]);
                }
                string receivedHash = builder.ToString(); //беру хеш из fullmessage








                 if (CheckIntegrity(receivedHash,receivedLength, message) == true)
                     return message;
                 else
                     return "Целостность нарушена"; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "123";
            }
            
        }


        //получение хеша из полученного сообщения
        public string GetHashMessage(string message)
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

        private bool CheckIntegrity(string receivedHash, string receivedLength, string message )
        {
            /* если целостность не нарушена - он возвращает true
               если цел-ть нарушена, то он вернёт false */

            string newhash = GetHashMessage(message); //хешируем полученное сообщение
            string newLength = message.Length.ToString(); // получаем длину из полученного сообщения


            bool same = true;

            //Сравнение значений двух длин
            int result = String.Compare(receivedLength, newLength);
            if (result != 0)
                same = false;

            //Сравнение значений двух хешей
            result = String.Compare(receivedHash, newhash);
            if (result != 0)
                same = false;








            return same;
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
