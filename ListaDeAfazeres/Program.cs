using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Data.SqlClient;

class Program
{
    private static string connectionString;

    static void Main(string[] args)
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        connectionString = configuration.GetConnectionString("dados");

        Console.Write("Digite a tarefa: ");
        var tarefa = Console.ReadLine();
        Console.WriteLine("Digite a data da tarefa (yyyy-mm-dd): ");
        var data = Console.ReadLine();

        // Connect to MySQL database and insert data
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = "INSERT INTO cad_tarefas(tarefa, data_tarefa) VALUES (@tarefa, @data)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tarefa", tarefa);
                command.Parameters.AddWithValue("@data", DateTime.Parse(data));
                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine("Tarefa inserida com sucesso.");
    }
}
