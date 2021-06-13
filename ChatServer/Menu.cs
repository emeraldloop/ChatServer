using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Menu
    {   
        public void choosecommand(ServerObject server, TcpClient client, string Id, string userName)
        {
            ClientObject Client = new ClientObject(client, server);
            
            server.BroadcastBack("Введите команду", Id);

            while (true)
            {
                string command = Client.GetMessage();
                switch (command)
                {
                    case "1":
                        command = String.Format("{0}: - число запросов", ClientObject.K);
                        Console.WriteLine(command);
                        server.BroadcastBack(command, Id);
                        break;
                    case "2":
                        ClientObject.K = 0;
                        break;
                    case "3":
                        server.BroadcastBack(userName + " - имя пользователя", Id);
                        break;
                    case "4":
                        server.BroadcastBack("1 - сохранение имени пользователя \n2 - чтение имён ", Id);
                        command = Client.GetMessage();
                        if (command == "1")
                            saveUser();
                        if (command == "2")
                            readUsers();
                        break;
                    case "5":
                        server.BroadcastBack("Введите строку для изменения", Id);
                        char[] array = Client.GetMessage().ToCharArray();
                        Array.Reverse(array);
                        string str = new string(array);
                        server.BroadcastBack(str, Id);
                        break;
                    case "6":
                        server.BroadcastBack(ServerObject.ConnectedUsers.ToString() + " - число подключенных пользователей", Id);
                        break;


                }
            }
        }
    }
}
