using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatServer
{
    class Utilities
    {
        public string Id { get; set; }
        public string userName { get; set; }
        public string userGroup { get; set; }
        public ServerObject server { get; set; }
        //ServerObject server, string Id, string userName, string userGroup
        async public void saveUser()
        {
            // сохранение данных
            try
            {
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {
                    Person user = new Person() { id = Id, Name = userName, Group = userGroup };
                    await JsonSerializer.SerializeAsync<Person>(fs, user);
                    server.BroadcastBack("Текущий пользователь был сохранен в файл", Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        async public void readUsers()
        {
            // чтение данных
            try
            {
                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {

                    Person restoredPerson = await JsonSerializer.DeserializeAsync<Person>(fs);
                    server.BroadcastBack($"ID: {restoredPerson.id}  Name: {restoredPerson.Name}  Group: {restoredPerson.Group}\n", this.Id);
                    server.BroadcastBack("Загрузка окончена", Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }



    }
}
