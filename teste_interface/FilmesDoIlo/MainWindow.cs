using System;
using Gtk;
using MySql.Data.MySqlClient;

namespace FilmesDoIlo
{
    public class MainWindow : Window
    {
        private string connectionString = "Server=localhost;Database=DB_Filmes;User ID=felipe;Password=Empirico28;";
        private Entry tituloEntry, diretorEntry, anoEntry, generoEntry, idEntry, searchEntry;
        private Label statusLabel;

        public MainWindow() : base("FILMES DO ILO")
        {
            SetDefaultSize(600, 400);
            SetPosition(WindowPosition.Center);
            DeleteEvent += delegate { Application.Quit(); };

            VBox mainVBox = new VBox(false, 5);

            // Criação dos menus
            MenuBar menuBar = new MenuBar();
            Menu menu = new Menu();

            MenuItem addMenuItem = new MenuItem("Adicionar Filme");
            addMenuItem.Activated += OnAddFilmeMenu;
            menu.Append(addMenuItem);

            MenuItem updateMenuItem = new MenuItem("Atualizar Filme");
            updateMenuItem.Activated += OnUpdateFilmeMenu;
            menu.Append(updateMenuItem);

            MenuItem listMenuItem = new MenuItem("Listar Filme");
            listMenuItem.Activated += OnListFilmeMenu;
            menu.Append(listMenuItem);

            MenuItem deleteMenuItem = new MenuItem("Deletar Filme");
            deleteMenuItem.Activated += OnDeleteFilmeMenu;
            menu.Append(deleteMenuItem);

            MenuItem menuMain = new MenuItem("Opções");
            menuMain.Submenu = menu;
            menuBar.Append(menuMain);

            mainVBox.PackStart(menuBar, false, false, 0);

            // Adiciona uma área de status
            statusLabel = new Label();
            mainVBox.PackStart(statusLabel, false, false, 0);

            Add(mainVBox);
            ShowAll();
        }

        private void OnAddFilmeMenu(object sender, EventArgs e)
        {
            LimparEntrada();

            // Adiciona campos de entrada
            tituloEntry = new Entry() { PlaceholderText = "Título" };
            diretorEntry = new Entry() { PlaceholderText = "Diretor" };
            anoEntry = new Entry() { PlaceholderText = "Ano" };
            generoEntry = new Entry() { PlaceholderText = "Gênero" };
            Button addButton = new Button("Adicionar Filme");
            addButton.Clicked += OnAddFilme;

            ExibirEntrada("Adicionar Filme", addButton);
        }

        private void OnUpdateFilmeMenu(object sender, EventArgs e)
        {
            LimparEntrada();

            // Adiciona campos de entrada
            idEntry = new Entry() { PlaceholderText = "ID do Filme" };
            tituloEntry = new Entry() { PlaceholderText = "Novo Título" };
            diretorEntry = new Entry() { PlaceholderText = "Novo Diretor" };
            anoEntry = new Entry() { PlaceholderText = "Novo Ano" };
            generoEntry = new Entry() { PlaceholderText = "Novo Gênero" };
            Button updateButton = new Button("Atualizar Filme");
            updateButton.Clicked += OnUpdateFilme;

            ExibirEntrada("Atualizar Filme", updateButton);
        }

        private void OnListFilmeMenu(object sender, EventArgs e)
        {
            LimparEntrada();

            // Adiciona campos de busca
            searchEntry = new Entry() { PlaceholderText = "Nome ou ID do Filme" };
            Button searchButton = new Button("Buscar Filme");
            searchButton.Clicked += OnListFilme;

            ExibirEntrada("Buscar Filme", searchButton);
        }

        private void OnDeleteFilmeMenu(object sender, EventArgs e)
        {
            LimparEntrada();

            // Adiciona campo para ID
            idEntry = new Entry() { PlaceholderText = "ID do Filme" };
            Button deleteButton = new Button("Deletar Filme");
            deleteButton.Clicked += OnDeleteFilme;

            ExibirEntrada("Deletar Filme", deleteButton);
        }

        private void OnAddFilme(object sender, EventArgs e)
        {
            string titulo = tituloEntry.Text;
            string diretor = diretorEntry.Text;
            int ano = int.TryParse(anoEntry.Text, out int parsedAno) ? parsedAno : 0;
            string genero = generoEntry.Text;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string insertQuery = "INSERT INTO filmes (titulo, diretor, ano, genero) VALUES (@titulo, @diretor, @ano, @genero)";
                using (var cmd = new MySqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@titulo", titulo);
                    cmd.Parameters.AddWithValue("@diretor", diretor);
                    cmd.Parameters.AddWithValue("@ano", ano);
                    cmd.Parameters.AddWithValue("@genero", genero);
                    cmd.ExecuteNonQuery();
                    statusLabel.Text = "Filme adicionado com sucesso!";
                }
            }
        }

        private void OnUpdateFilme(object sender, EventArgs e)
        {
            int id = int.TryParse(idEntry.Text, out int parsedId) ? parsedId : 0;
            string titulo = tituloEntry.Text;
            string diretor = diretorEntry.Text;
            int ano = int.TryParse(anoEntry.Text, out int parsedAno) ? parsedAno : 0;
            string genero = generoEntry.Text;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE filmes SET titulo = @titulo, diretor = @diretor, ano = @ano, genero = @genero WHERE id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@titulo", titulo);
                    cmd.Parameters.AddWithValue("@diretor", diretor);
                    cmd.Parameters.AddWithValue("@ano", ano);
                    cmd.Parameters.AddWithValue("@genero", genero);
                    cmd.ExecuteNonQuery();
                    statusLabel.Text = "Filme atualizado com sucesso!";
                }
            }
        }

        private void OnListFilme(object sender, EventArgs e)
        {
            string searchTerm = searchEntry.Text;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = int.TryParse(searchTerm, out int id) ?
                               "SELECT * FROM filmes WHERE id = @searchTerm" :
                               "SELECT * FROM filmes WHERE titulo LIKE @searchTerm";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@searchTerm", id > 0 ? id : $"%{searchTerm}%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            statusLabel.Text = $"Filme encontrado: {reader["titulo"]} - {reader["diretor"]} - {reader["ano"]} - {reader["genero"]}";
                        }
                        else
                        {
                            statusLabel.Text = "Filme não encontrado.";
                        }
                    }
                }
            }
        }

        private void OnDeleteFilme(object sender, EventArgs e)
        {
            int id = int.TryParse(idEntry.Text, out int parsedId) ? parsedId : 0;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM filmes WHERE id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    statusLabel.Text = "Filme deletado com sucesso!";
                }
            }
        }

        private void LimparEntrada()
        {
            tituloEntry = diretorEntry = anoEntry = generoEntry = idEntry = searchEntry = null;
        }

        private void ExibirEntrada(string titulo, Button actionButton)
        {
            // Remove todos os widgets para evitar conflito de widgets já associados a contêineres
            foreach (Widget child in Children)
            {
                Remove(child);
                child.Destroy();  // Destroi widgets antigos para evitar reutilização
            }

            // Cria uma nova VBox para organizar os widgets da interface
            VBox mainVBox = new VBox(false, 5);
            mainVBox.PackStart(new Label("FILMES DO ILO"), false, false, 0);

            VBox inputVBox = new VBox(false, 5);
            inputVBox.PackStart(new Label(titulo), false, false, 0);

            if (idEntry != null) idEntry = new Entry() { PlaceholderText = "ID do Filme" };
            if (tituloEntry != null) tituloEntry = new Entry() { PlaceholderText = "Título" };
            if (diretorEntry != null) diretorEntry = new Entry() { PlaceholderText = "Diretor" };
            if (anoEntry != null) anoEntry = new Entry() { PlaceholderText = "Ano" };
            if (generoEntry != null) generoEntry = new Entry() { PlaceholderText = "Gênero" };
            if (searchEntry != null) searchEntry = new Entry() { PlaceholderText = "Nome ou ID do Filme" };

            if (idEntry != null) inputVBox.PackStart(idEntry, false, false, 0);
            if (tituloEntry != null) inputVBox.PackStart(tituloEntry, false, false, 0);
            if (diretorEntry != null) inputVBox.PackStart(diretorEntry, false, false, 0);
            if (anoEntry != null) inputVBox.PackStart(anoEntry, false, false, 0);
            if (generoEntry != null) inputVBox.PackStart(generoEntry, false, false, 0);
            if (searchEntry != null) inputVBox.PackStart(searchEntry, false, false, 0);

            inputVBox.PackStart(actionButton, false, false, 0);
            mainVBox.PackStart(inputVBox, true, true, 0);
            mainVBox.PackStart(statusLabel, false, false, 0);

            Add(mainVBox);
            ShowAll();
        }



        public static void Main()
        {
            Application.Init();
            new MainWindow();
            Application.Run();
        }
    }
}
