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

                    foreach (KeyValuePair<string, Person> dict in restoredUsers)
                    {
                        string message = "Id: " + dict.Key + " Имя: " + dict.Value.Name + " Группа: " + dict.Value.Group + "\n";
                        Broadcast(message,Stream);                        
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        async public void deleteUser(String Id2del)
        {
            Dictionary<string, Person> restoredUsers = new();
            // чтение данных
            try
            {
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {

                    restoredUsers = await JsonSerializer.DeserializeAsync<Dictionary<string, Person>>(fs);
                    if (restoredUsers.ContainsKey(Id2del))
                        restoredUsers.Remove(Id2del);
                }
                File.Delete("user.json");
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync(fs, restoredUsers);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

         async public void GetTime(NetworkStream Stream)
         {
            while(true)
            {
                Broadcast(DateTime.Now.ToString(),Stream);
                await Task.Delay(5000);
                
            }
         }
        private void Broadcast(string message, NetworkStream Stream)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Stream.Write(data, 0, data.Length); // передача данных
        }







    }
}
