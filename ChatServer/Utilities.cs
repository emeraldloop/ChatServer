using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatServer
{
    class Utilities
    {

        async public void saveUser(string Id, string userName,string userGroup)
        {
            Dictionary<string, Person> restoredUsers = new();


            // сохранение данных в файл
            try
            {
                if (File.Exists("user.json"))
                {
                    using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                    {
                        restoredUsers = await JsonSerializer.DeserializeAsync<Dictionary<string, Person>>(fs);
                    }
                    File.Delete("user.json");
                    using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                    {
                        Person user = new Person() { Name = userName, Group = userGroup };
                        restoredUsers.Add(Id, user);
                        await JsonSerializer.SerializeAsync(fs, restoredUsers);
                    }
                }
                else
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {                    
                    Person user = new Person() { Name = userName, Group = userGroup };
                    Dictionary<string, Person> users = new();
                    users.Add(Id, user);
                    await JsonSerializer.SerializeAsync(fs, users);                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        async public void takeUsers(NetworkStream Stream)
        {
            Dictionary<string, Person> restoredUsers = new();
            // чтение данных
            try
            {
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {

                    restoredUsers = await JsonSerializer.DeserializeAsync<Dictionary<string, Person>>(fs);

                    foreach (Person p in restoredUsers.Values)
                    {
                        byte[] data = Encoding.Unicode.GetBytes("Имя: "+ p.Name+" Группа: "+ p.Group+"\n");
                        Stream.Write(data, 0, data.Length); // передача данных
                    }
                    byte[] data2 = Encoding.Unicode.GetBytes("Сообщение отправлено");
                    Stream.Write(data2, 0, data2.Length);


                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        


    }
}
