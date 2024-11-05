using System;
using MySql.Data.MySqlClient;

namespace FilmesDoIlo
{
    class Program
    {
        // Dados da conexão
        static string connectionString = "Server=localhost;Database=DB_Filmes;User ID=felipe;Password=Empirico28;";

        static void Main(string[] args)
        {
            // Executa o menu principal
            Menu();
        }

        // Método para criar a tabela de filmes
        static void CreateTable()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS filmes (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        titulo VARCHAR(100) NOT NULL,
                        diretor VARCHAR(100),
                        ano INT,
                        genero VARCHAR(50)
                    );
                ";
                using (MySqlCommand cmd = new MySqlCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Método para adicionar um novo filme
        static void CreateFilme()
        {
            Console.Write("Digite o titulo: ");
            string titulo = Console.ReadLine();
            Console.Write("Digite o diretor: ");
            string diretor = Console.ReadLine();
            Console.Write("Digite o ano: ");
            int ano = int.Parse(Console.ReadLine());
            Console.Write("Digite o genero: ");
            string genero = Console.ReadLine();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string insertQuery = "INSERT INTO filmes (titulo, diretor, ano, genero) VALUES (@titulo, @diretor, @ano, @genero)";
                using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@titulo", titulo);
                    cmd.Parameters.AddWithValue("@diretor", diretor);
                    cmd.Parameters.AddWithValue("@ano", ano);
                    cmd.Parameters.AddWithValue("@genero", genero);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Filme adicionado com sucesso!");
                }
            }
        }

        // Método para listar os filmes
        static void ListFilme()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string selectQuery = "SELECT * FROM filmes";
                using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nLista de Filmes:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["id"]}\nTitulo: {reader["titulo"]}\nDiretor: {reader["diretor"]}\nAno: {reader["ano"]}\nGenero: {reader["genero"]}\n");
                    }
                }
            }
        }

        // Método para atualizar um filme
        static void AtualizarFilme()
        {
            ListFilme();
            Console.Write("Digite o ID que deseja atualizar: ");
            int filmeId = int.Parse(Console.ReadLine());

            Console.Write("Novo Titulo (deixe em branco para manter): ");
            string novoTitulo = Console.ReadLine();
            Console.Write("Novo Diretor (deixe em branco para manter): ");
            string novoDiretor = Console.ReadLine();
            Console.Write("Novo Ano (deixe em branco para manter): ");
            string novoAno = Console.ReadLine();
            Console.Write("Novo Genero (deixe em branco para manter): ");
            string novoGenero = Console.ReadLine();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = "UPDATE filmes SET ";
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                if (!string.IsNullOrEmpty(novoTitulo))
                {
                    updateQuery += "titulo = @titulo, ";
                    cmd.Parameters.AddWithValue("@titulo", novoTitulo);
                }
                if (!string.IsNullOrEmpty(novoDiretor))
                {
                    updateQuery += "diretor = @diretor, ";
                    cmd.Parameters.AddWithValue("@diretor", novoDiretor);
                }
                if (!string.IsNullOrEmpty(novoAno))
                {
                    updateQuery += "ano = @ano, ";
                    cmd.Parameters.AddWithValue("@ano", int.Parse(novoAno));
                }
                if (!string.IsNullOrEmpty(novoGenero))
                {
                    updateQuery += "genero = @genero, ";
                    cmd.Parameters.AddWithValue("@genero", novoGenero);
                }

                updateQuery = updateQuery.TrimEnd(',', ' ') + " WHERE id = @id";
                cmd.CommandText = updateQuery;
                cmd.Parameters.AddWithValue("@id", filmeId);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Filme atualizado com sucesso!");
            }
        }

        // Método para deletar um filme
        static void DeleteFilme()
        {
            ListFilme();
            Console.Write("Digite o ID que deseja deletar: ");
            int filmeId = int.Parse(Console.ReadLine());

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string deleteQuery = "DELETE FROM filmes WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", filmeId);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Filme com ID {filmeId} deletado com sucesso.");
                }
            }
        }

        // Menu principal
        static void Menu()
        {
            CreateTable();
            while (true)
            {
                Console.WriteLine("\n----FILMES-DO-ILO----");
                Console.WriteLine("1. Adicionar Filme");
                Console.WriteLine("2. Listar Filme");
                Console.WriteLine("3. Atualizar Filme");
                Console.WriteLine("4. Deletar Filme");
                Console.WriteLine("5. Sair");

                Console.Write("Digite uma Opção: ");
                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        CreateFilme();
                        break;
                    case "2":
                        ListFilme();
                        break;
                    case "3":
                        AtualizarFilme();
                        break;
                    case "4":
                        DeleteFilme();
                        break;
                    case "5":
                        Console.WriteLine("Saindo...");
                        return;
                    default:
                        Console.WriteLine("Opção Inválida");
                        break;
                }
            }
        }
    }
}
