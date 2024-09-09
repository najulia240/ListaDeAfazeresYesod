using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;

class Program
{
    private static string? connectionString;

    static void Main(string[] args)
    {
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        connectionString = configuration.GetConnectionString("dados");

        ShowMenu();
        var option = Console.ReadLine();

        while (true)
        {
            switch (option)
            {
                case "1":
                    InserirTarefa();
                    break;
                case "2":
                    AtualizarTarefa();
                    break;
                case "3":
                    RemoverTarefa();
                    break;
                case "4":
                    ListarTarefas();
                    break;
                case "5":
                    Console.WriteLine("Saindo...");
                    return;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    WaitForUser();
                    break;
            }

            ShowMenu();
            option = Console.ReadLine();
        }
    }

    private static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("Menu:");
        Console.WriteLine("1. Inserir tarefa");
        Console.WriteLine("2. Atualizar tarefa");
        Console.WriteLine("3. Remover tarefa");
        Console.WriteLine("4. Listar tarefas");
        Console.WriteLine("5. Sair");
        Console.Write("Escolha uma opção: ");
    }

    private static void InserirTarefa()
    {
        Console.Clear();
        var tarefa = Prompt("Digite a tarefa: ");
        var data = Prompt("Digite a data da tarefa (yyyy-mm-dd): ");
        var dificuldade = Prompt("Digite a dificuldade da tarefa (Fácil, Médio, Difícil): ");

        if (!ValidateDate(data))
        {
            Console.WriteLine("Formato de data inválido. Por favor, use o formato yyyy-mm-dd.");
            WaitForUser();
            return;
        }

        try
        {
            ExecuteNonQuery("INSERT INTO cad_tarefas (tarefa, data_tarefas, dificuldade) VALUES (@tarefa, @data, @dificuldade)",
                new MySqlParameter("@tarefa", tarefa),
                new MySqlParameter("@data", data),
                new MySqlParameter("@dificuldade", dificuldade));

            Console.WriteLine("Tarefa inserida com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }

        WaitForUser();
    }

    private static void AtualizarTarefa()
    {
        Console.Clear();
        var id = Prompt("Digite o ID da tarefa a ser atualizada: ");
        var tarefa = Prompt("Digite a nova descrição da tarefa: ");
        var data = Prompt("Digite a nova data da tarefa (yyyy-mm-dd): ");
        var dificuldade = Prompt("Digite a nova dificuldade da tarefa (Fácil, Médio, Difícil): ");

        if (!ValidateDate(data))
        {
            Console.WriteLine("Formato de data inválido. Por favor, use o formato yyyy-mm-dd.");
            WaitForUser();
            return;
        }

        try
        {
            ExecuteNonQuery("UPDATE cad_tarefas SET tarefa = @tarefa, data_tarefas = @data, dificuldades = @dificuldade WHERE id = @id",
                new MySqlParameter("@id", id),
                new MySqlParameter("@tarefa", tarefa),
                new MySqlParameter("@data", data),
                new MySqlParameter("@dificuldade", dificuldade));

            Console.WriteLine("Tarefa atualizada com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }

        WaitForUser();
    }

    private static void RemoverTarefa()
    {
        Console.Clear();
        var id = Prompt("Digite o ID da tarefa a ser removida: ");

        try
        {
            ExecuteNonQuery("DELETE FROM cad_tarefas WHERE id = @id", new MySqlParameter("@id", id));
            Console.WriteLine("Tarefa removida com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }

        WaitForUser();
    }

    private static void ListarTarefas()
    {
        Console.Clear();
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT id, tarefa, data_tarefas, dificuldades FROM cad_tarefas";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["id"]}, Tarefa: {reader["tarefa"]}, Data: {reader["data_tarefas"]}, Dificuldade: {reader["dificuldades"]}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }

        WaitForUser();
    }

    private static string Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }

    private static void WaitForUser()
    {
        Console.WriteLine("Pressione qualquer tecla para voltar ao menu...");
        Console.ReadKey();
    }

    private static bool ValidateDate(string date)
    {
        return DateTime.TryParse(date, out _);
    }

    private static void ExecuteNonQuery(string query, params MySqlParameter[] parameters)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }
        }
    }
}

