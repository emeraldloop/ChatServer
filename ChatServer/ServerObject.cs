using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;

namespace ChatServer
{
    public class ServerObject
    {
        static TcpListener tcpListener; //сервер для прослушивания
        List<ClientObject> clients = new List<ClientObject>(); // все подключения
        static private int connectedUsers = 0; //счетчик подключенных клиентов (в данный момент)
        static public int ConnectedUsers
        {
            get
            {
                return connectedUsers;
            }
        }
        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
            connectedUsers++;
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение 
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
           
            // и удалеяем его из списка подключений 
            if (client != null)
                clients.Remove(client);
            connectedUsers--;
        }
        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        //трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, string id)
        {
            
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i=0;i<clients.Count;i++)
            {
                if (clients[i].Id != id) // если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); // передача данных
                }    
            }
        }
        protected internal void BroadcastBack(string message, string id)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                byte[] data = Encoding.Unicode.GetBytes(message);

                if (clients[i].Id == id) // если id клиента равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); // передача данных
                }
            }
        }



// отключение всех клиентов-
        protected internal void Disconnect()
        {
            tcpListener.Stop(); // остановка сервера
            for (int i = 0; i<clients.Count;i++)
            {
                clients[i].Close(); // отключение клиента 
            }
            Environment.Exit(0); // завершение процесса
        }
    
    
    
    
    
    }
}
