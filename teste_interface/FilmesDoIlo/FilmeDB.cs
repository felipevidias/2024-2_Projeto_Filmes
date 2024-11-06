using System;
using MySql.Data.MySqlClient;

namespace FilmesDoIlo
{
    public class FilmeDB
    {
        private string connectionString = "Server=localhost;Database=DB_Filmes;User ID=felipe;Password=Empirico28;";

        public void CreateTable()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"CREATE TABLE IF NOT EXISTS filmes (
                                    id INT AUTO_INCREMENT PRIMARY KEY,
                                    titulo VARCHAR(100) NOT NULL,
                                    diretor VARCHAR(100),
                                    ano INT,
                                    genero VARCHAR(50)
                                );";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddFilme(string titulo, string diretor, int ano, string genero)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO filmes (titulo, diretor, ano, genero) VALUES (@titulo, @diretor, @ano, @genero)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@titulo", titulo);
                    cmd.Parameters.AddWithValue("@diretor", diretor);
                    cmd.Parameters.AddWithValue("@ano", ano);
                    cmd.Parameters.AddWithValue("@genero", genero);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public MySqlDataReader ListFilmes()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            string query = "SELECT * FROM filmes";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            return cmd.ExecuteReader();
        }

        public void UpdateFilme(int id, string titulo, string diretor, int ano, string genero)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE filmes SET titulo = @titulo, diretor = @diretor, ano = @ano, genero = @genero WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@titulo", titulo);
                    cmd.Parameters.AddWithValue("@diretor", diretor);
                    cmd.Parameters.AddWithValue("@ano", ano);
                    cmd.Parameters.AddWithValue("@genero", genero);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteFilme(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM filmes WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
