﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Menu
    {

        public void choosecommand(string Id, string userName, string userGroup, ref ServerObject server, ref TcpClient client, NetworkStream Stream)
        {
            try
            {
                ClientObject Client = new ClientObject(client, server);
                Utilities utility = new();
                server.BroadcastBack("Введите команду", Id);
                bool complete = false;
                while (complete != true)
                {
                    Client.Stream = Stream; 
                    string command = Client.GetMessage();
                    switch (command)
                    {
                        case "1":
                            command = String.Format("{0}: - число запросов", ClientObject.K);
                            Console.WriteLine(command);
                            server.BroadcastBack(command, Id);
                            complete = true;
                            break;
                        case "2":
                            ClientObject.K = 0;
                            server.BroadcastBack("Счётчик сброшен", Id);
                            complete = true;
                            break;
                        case "3":
                            server.BroadcastBack(userName + " - имя пользователя", Id);
                            complete = true;
                            break;
                        case "4":
                            server.BroadcastBack("1 - сохранение имени пользователя \n2 - чтение имён ", Id);
                            command = Client.GetMessage();
                            if (command == "1")
                            {
                                utility.saveUser(Id,userName,userGroup);
                                server.BroadcastBack("Текущий пользователь был сохранен в файл", Id);
                            }
                            if (command == "2")
                            {   
                                
                                utility.takeUsers();
                                foreach (Person p in utility.Users2read.Values)
                                {
                                    server.BroadcastBack(String.Format($"Имя: {1}, Группа: {2}\n", p.Name, p.Group), Id);
                                }
                                server.BroadcastBack("Загрузка окончена \n", Id);
                            }
                            complete = true;
                            break;
                        case "5":
                            server.BroadcastBack("Введите строку для изменения", Id);
                            char[] array = Client.GetMessage().ToCharArray();
                            Array.Reverse(array);
                            string str = new string(array);
                            server.BroadcastBack(str, Id);
                            complete = true;
                            break;
                        case "6":
                            server.BroadcastBack((ServerObject.ConnectedUsers - 1).ToString() + " - число подключенных пользователей", Id);
                            complete = true;
                            break;


                    }
                
                }
                server.RemoveConnection(Client.Id);
            }
            catch (Exception ex)
            { Console.WriteLine(ex); }
        }
    }
}
