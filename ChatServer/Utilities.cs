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
        public Dictionary<string, Person> Users2read = new();
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
        async public void takeUsers()
        {
            Dictionary<string, Person> restoredUsers = new();
            // чтение данных
            try
            {
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {

                    restoredUsers = await JsonSerializer.DeserializeAsync<Dictionary<string, Person>>(fs);
                    Users2read = restoredUsers;
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        


    }
}
