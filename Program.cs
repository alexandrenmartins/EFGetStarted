using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace HelloWorldSample;

class Program
{
    static void Main()
    {
        if( File.Exists("hello.db"))
        {
            //File.GetAttributes("Path");
            //File.Delete("hello.db");
        }
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        Console.WriteLine($"Path is {path}.");

        using (var connection = new SqliteConnection("Data Source=hello.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                    CREATE TABLE user (
                        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL
                    );

                    INSERT INTO user
                    VALUES (1, 'Brice'),
                           (2, 'Alexander'),
                           (3, 'Nate');
                ";
            command.ExecuteNonQuery();

            Console.Write("Name: ");
            var name = Console.ReadLine();

            #region snippet_Parameter
            command.CommandText =
            @"
                    INSERT INTO user (name)
                    VALUES ($name)
                ";
            command.Parameters.AddWithValue("$name", name);
            #endregion
            command.ExecuteNonQuery();

            command.CommandText =
            @"
                    SELECT last_insert_rowid()
                ";
            var newId = (long)command.ExecuteScalar();

            Console.WriteLine($"Your new user ID is {newId}.");
            //connection.Close();
        }

        Console.Write("User ID: ");
        var id = int.Parse(Console.ReadLine());

        #region snippet_HelloWorld
        using (var connection = new SqliteConnection("Data Source=hello.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                    SELECT name
                    FROM user
                    WHERE id = $id
                ";
            command.Parameters.AddWithValue("$id", id);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(0);

                    Console.WriteLine($"Hello, {name}!");
                }
            }
            //connection.Close();
            command = connection.CreateCommand();
            command.CommandText =
            @"
                    SELECT *
                    FROM user
                ";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id2 = reader.GetString(0);
                    var name = reader.GetString(1);

                    Console.WriteLine(id2 + "-" + name);
                }
            }
        }
        #endregion

        // Clean up
        //File.Delete("hello.db");
    }
}