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
        List<Person> persons = new List <Person>();// список пользователей
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        Menu menu;
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
       
         async void saveUser()
        {
            // сохранение данных
            try
            {   
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {   
                    Person user = new Person() { id = Id, Name = userName, Group = userGroup };
                    await JsonSerializer.SerializeAsync<Person>(fs, user);
                    server.BroadcastBack("Текущий пользователь был сохранен в файл", this.Id);                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        async void readUsers() 
        {
            // чтение данных
            try
            {
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {
                    
                    Person restoredPerson = await JsonSerializer.DeserializeAsync<Person>(fs);
                    server.BroadcastBack($"ID: {restoredPerson.id}  Name: {restoredPerson.Name}  Group: {restoredPerson.Group}\n", this.Id);
                    server.BroadcastBack("Загрузка окончена", this.Id);
                }
            }
            catch(Exception ex)
            { 
                Console.WriteLine(ex);
            }
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
                message = userGroup + " из группы: "+ userName + " вошел в чат"; //посылаем сообщение о входе в чат всем пользователям
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
                            menu.choosecommand(server,client,this.Id,userName);
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


        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
        /*private void menuuu()
        {   
            server.BroadcastBack("Введите команду", this.Id);

            while (true)
            {
                string command = GetMessage();
                switch (command)
                {
                    case "1":
                        command = String.Format("{0}: - число запросов", k);
                        Console.WriteLine(command);
                        server.BroadcastBack(command, this.Id);
                        break;
                    case "2":
                        k = 0;
                        break;
                    case "3":
                        server.BroadcastBack(userName + " - имя пользователя", this.Id);
                        break;
                    case "4":
                        server.BroadcastBack("1 - сохранение имени пользователя \n2 - чтение имён ", this.Id);
                        command = GetMessage();
                        if (command == "1")
                            saveUser();
                        if (command == "2")
                            readUsers();
                        break;
                    case "5":
                        server.BroadcastBack("Введите строку для изменения", this.Id);
                        char[] array = GetMessage().ToCharArray();
                        Array.Reverse(array);
                        string str = new string(array);
                        server.BroadcastBack(str, this.Id);
                        break;
                    case "6":
                        server.BroadcastBack(ServerObject.ConnectedUsers.ToString()+" - число подключенных пользователей", this.Id);
                        break;
                 

                }
            }
        } */
        
    }
}
