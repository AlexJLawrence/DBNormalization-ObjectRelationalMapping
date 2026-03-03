using System;
using Microsoft.Data.Sqlite;

namespace ObjectRelationalMapping
{
    class Program
    {
        static SqliteConnection connection = new SqliteConnection("Data source=myDb.db");

        public static void Main(string[] args){
            Console.WriteLine("Starting");


            if(args.Length == 0){
                Console.WriteLine("No arguments given");
                PrintHelp();
                return;
            }

            connection.Open();

            switch(args[0]){
                case "Create":
                    Console.WriteLine("Creating table");
                    CreateUserTable();
                    break;
                case "Add":
                    Console.WriteLine("Adding user");
                    AddUser(args[1]);
                    PrintUsers();
                    break;
                case "Remove":
                    Console.WriteLine("Removing user");
                    RemoveUser(args[1]);
                    PrintUsers();
                    break;
                case "Print":
                    Console.WriteLine("Printing users");
                    PrintUsers();
                    break;
                case "Build":
                    Console.WriteLine("Building user");
                    BuildUser();
                    PrintUsers();
                    break;
                case "Clear":
                    Console.WriteLine("Clearing users table");
                    ClearUsers();
                    PrintUsers();
                    break;
                case "Help":
                    PrintHelp();
                    break;
                default:
                    Console.WriteLine("Unrecognized argument: '" + args[0] + "'");
                    PrintHelp();
                    break;
            }

            Console.WriteLine("Finished");
        }

        static void PrintHelp(){
            Console.WriteLine("Create - Create a a user table if there isn't one");
            Console.WriteLine("Add username - Adds a user with the given username and email of username@gmail.com");
            Console.WriteLine("Remove userId - Removes user with the given userId if there is one");
            Console.WriteLine("Print - Prints the user table");
            Console.WriteLine("Build - Walks through building a user");
            Console.WriteLine("Help - Displays this message");
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
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Users (Name, Email) VALUES(@name, @email)";

            command.Parameters.AddWithValue("@name", username);
            command.Parameters.AddWithValue("@email", username + "@gmail.com");

            command.ExecuteNonQuery();
        }

        static void AddUser(User user){
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Users (Name, Email) VALUES(@name, @email)";

            command.Parameters.AddWithValue("@name", user.name);
            command.Parameters.AddWithValue("@email", user.email);

            command.ExecuteNonQuery();
        }

        static void RemoveUser(string idString){
            int id = int.Parse(idString);

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        static void PrintUsers(){
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Email FROM Users;";

            List<User> users = new List<User>();
            var reader = command.ExecuteReader();
            while(reader.Read()){
                var user = new User(reader.GetInt16(0), reader.GetString(1), reader.GetString(2));
                users.Add(user);
            }

            Console.WriteLine("New Users Table:");
            foreach(User user in users){
                string toPrint = user.id + " " + user.name + " " + user.email;
                Console.WriteLine(toPrint);
            }
        }

        static void BuildUser(){
            User newUser = new User();
            Console.WriteLine("ID is autocremented");
            
            string? name;
            while(true){
                Console.WriteLine("Please give a name:");
                name = Console.ReadLine();
                if(!string.IsNullOrWhiteSpace(name)){
                    break;
                }
            }
            newUser.name = name;

            string? email;
            while(true){
                Console.WriteLine("Please give an email:");
                email = Console.ReadLine();
                if(!string.IsNullOrWhiteSpace(name)){
                    break;
                }
            }
            newUser.email = email;

            AddUser(newUser);
        }

        static void ClearUsers(){
            var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM Users; DELETE FROM sqlite_sequence WHERE name='Users';"; 
            command.ExecuteNonQuery();
        }
    }

    public class User{
        public int? id;
        public string? name;
        public string? email;

        public User(){}

        public User(int id_, string name_, string email_){
            id = id_;
            name = name_;
            email = email_;
        }
    }
}