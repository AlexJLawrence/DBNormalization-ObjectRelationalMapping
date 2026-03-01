using System;
using Microsoft.Data.Sqlite;

namespace ObjectRelationalMapping
{
    class Program
    {
        static SqliteConnection connection = new SqliteConnection("Data source=myDb.db");

        public static void Main(string[] args){
            Console.WriteLine("Starting");

            if(args.Length == 0) return;

            connection.Open();

            switch(args[0]){
                case "Create":
                    Console.WriteLine("Creating table");
                    CreateUserTable();
                    break;
                case "Add":
                    Console.WriteLine("Adding user");
                    AddUser(args[1]);
                    break;
                case "Remove":
                    Console.WriteLine("Removing user");
                    RemoveUser(args[1]);
                    break;
                case "Print":
                    Console.WriteLine("Printing users");
                    PrintUsers();
                    break;
                default:
                    break;
            }

            Console.WriteLine("Finished");
        }

        static void CreateUserTable(){
            var createCommand = connection.CreateCommand();
            createCommand.CommandText = @"CREATE TABLE IF NOT EXISTS Users(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL
            );";

            createCommand.ExecuteNonQuery();
        }

        static void AddUser(string username){
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"INSERT INTO Users (Name, Email)
                VALUES(@name, @email)
            ";

            insertCommand.Parameters.AddWithValue("@name", username);
            insertCommand.Parameters.AddWithValue("@email", username + "@gmail.com");

            insertCommand.ExecuteNonQuery();
        }

        static void RemoveUser(string idString){
            int id = int.Parse(idString);

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        static void PrintUsers(){
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Id, Name, Email FROM Users;";

            List<User> users = new List<User>();

            var reader = selectCommand.ExecuteReader();
            while(reader.Read()){
                var user = new User(reader.GetInt16(0), reader.GetString(1), reader.GetString(2));
                users.Add(user);
            }

            foreach(User user in users){
                string toPrint = user.id + " " + user.name + " " + user.email;
                Console.WriteLine(toPrint);
            }
        }
    }

    public class User{
        public int id;
        public string name;
        public string email;

        public User(int id_, string name_, string email_){
            id = id_;
            name = name_;
            email = email_;
        }
    }
}