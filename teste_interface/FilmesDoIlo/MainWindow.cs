using System;
using System.Text;
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

            ApplyCustomStyles();


            VBox mainVBox = new VBox(false, 5);

            // Botões para as opções
            Button addButton = new Button("Adicionar Filme");
            addButton.Clicked += OnAddFilmeMenu;
            mainVBox.PackStart(addButton, false, false, 5);

            Button updateButton = new Button("Atualizar Filme");
            updateButton.Clicked += OnUpdateFilmeMenu;
            mainVBox.PackStart(updateButton, false, false, 5);

            Button listButton = new Button("Listar Filme");
            listButton.Clicked += OnListFilmeMenu;
            mainVBox.PackStart(listButton, false, false, 5);

            Button deleteButton = new Button("Deletar Filme");
            deleteButton.Clicked += OnDeleteFilmeMenu;
            mainVBox.PackStart(deleteButton, false, false, 5);

            // Adiciona uma área de status
            statusLabel = new Label();
            mainVBox.PackEnd(statusLabel, false, false, 10);

            Add(mainVBox);
            ShowAll();
        }

        private void ApplyCustomStyles()
        {
            string css = @"
    /* Estilo para a janela principal */
    window {
        background-color: #f5f5f5;
        color: #4e6bed;
    }

    /* Estilo para os botões */
    button {
        background-color: #4e6bed;
        color: #f5f5f5;
        border-width: 2px;
        border-color: #4e6bed;
        padding: 10px;
        border-radius: 5px;
        min-width: 200px; /* Define a largura mínima do botão */
        margin: 5px 0px; /* Define uma margem em cima e embaixo */
    }

    /* Estilo para os botões ao passar o mouse */
    button:hover {
        background-color: #f5f5f5;
        color: #242426; /* Texto preto ao passar o mouse */
        border-color: #242426;
        box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.3); /* Adiciona a sombra */
    }

    /* Estilo para o texto */
    label {
        color: #242426;
        font-size: 16px;
        font-weight: bold;
    }

    /* Estilo para entradas de texto */
    entry {
        background-color: #f5f5f5;
        color: #4e6bed;
        border-width: 1px;
        border-color: #4e6bed;
        padding: 5px;
    }
";

            // Aplica o CSS
            CssProvider provider = new CssProvider();
            provider.LoadFromData(css);
            StyleContext.AddProviderForScreen(
                Gdk.Screen.Default,
                provider,
                Gtk.StyleProviderPriority.User
            );
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
                }
            }
            ExibirPopUp("Sucesso", "Filme adicionado com sucesso!");
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
                }
            }
            ExibirPopUp("Atualizado", "Filme atualizado com sucesso!");
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
                        if (reader.HasRows)
                        {
                            StringBuilder result = new StringBuilder();
                            result.Append("<b><u>Filmes Encontrados:</u></b>\n\n\n");

                            while (reader.Read())
                            {
                                result.AppendFormat("<b>Título:</b>  {0}\n\n", reader["titulo"]);
                                result.AppendFormat("<b>Diretor:</b> {0}\n\n", reader["diretor"]);
                                result.AppendFormat("<b>Ano:</b>     {0}\n\n", reader["ano"]);
                                result.AppendFormat("<b>Gênero:</b>  {0}\n\n", reader["genero"]);
                            }

                            ExibirModal("Resultado da Busca", result.ToString());
                        }
                        else
                        {
                            ExibirModal("Resultado da Busca", "Nenhum filme encontrado.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Exibe uma janela modal centralizada com as informações fornecidas.
        /// </summary>
        /// <param name="titulo">Título da janela.</param>
        /// <param name="conteudo">Conteúdo a ser exibido na janela.</param>
        private void ExibirModal(string titulo, string conteudo)
        {
            Dialog dialog = new Dialog(titulo, this, DialogFlags.Modal);
            dialog.SetDefaultSize(400, 400);

            // Use Gtk.Box com orientação vertical
            Box contentBox = (Box)dialog.ContentArea;

            Label contentLabel = new Label
            {
                Markup = conteudo,
                Xalign = 0.5f,
                Yalign = 0.5f,
                LineWrap = true,
                LineWrapMode = Pango.WrapMode.WordChar
            };

            // Adiciona o conteúdo ao box
            contentBox.PackStart(contentLabel, true, true, 10);

            // Botão "Fechar"
            Button closeButton = new Button("Fechar");
            closeButton.Clicked += (s, e) => dialog.Destroy();
            contentBox.PackEnd(closeButton, false, false, 10);

            dialog.ShowAll();
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
            ExibirPopUp("Deletado", "Filme deletado com sucesso!");
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
            VBox inputVBox = new VBox(false, 5);


            // Adiciona os campos de entrada, se existentes
            if (idEntry != null) inputVBox.PackStart(idEntry, false, false, 0);
            if (tituloEntry != null) inputVBox.PackStart(tituloEntry, false, false, 0);
            if (diretorEntry != null) inputVBox.PackStart(diretorEntry, false, false, 0);
            if (anoEntry != null) inputVBox.PackStart(anoEntry, false, false, 0);
            if (generoEntry != null) inputVBox.PackStart(generoEntry, false, false, 0);
            if (searchEntry != null) inputVBox.PackStart(searchEntry, false, false, 0);

            // Adiciona o botão de ação
            inputVBox.PackStart(actionButton, false, false, 0);

            // Adiciona o botão de voltar
            Button backButton = new Button("Voltar");
            backButton.Clicked += (sender, e) => VoltarParaMenuInicial();
            inputVBox.PackStart(backButton, false, false, 0);

            mainVBox.PackStart(inputVBox, true, true, 0);
            mainVBox.PackStart(statusLabel, false, false, 0);

            Add(mainVBox);
            ShowAll();
        }

        private void VoltarParaMenuInicial()
        {
            // Remove todos os widgets para voltar ao menu inicial
            foreach (Widget child in Children)
            {
                Remove(child);
                child.Destroy();
            }

            VBox mainVBox = new VBox(false, 5);

            // Botões para as opções
            Button addButton = new Button("Adicionar Filme");
            addButton.Clicked += OnAddFilmeMenu;
            mainVBox.PackStart(addButton, false, false, 5);

            Button updateButton = new Button("Atualizar Filme");
            updateButton.Clicked += OnUpdateFilmeMenu;
            mainVBox.PackStart(updateButton, false, false, 5);

            Button listButton = new Button("Listar Filme");
            listButton.Clicked += OnListFilmeMenu;
            mainVBox.PackStart(listButton, false, false, 5);

            Button deleteButton = new Button("Deletar Filme");
            deleteButton.Clicked += OnDeleteFilmeMenu;
            mainVBox.PackStart(deleteButton, false, false, 5);

            // Adiciona uma área de status
            mainVBox.PackEnd(statusLabel, false, false, 10);

            Add(mainVBox);
            ShowAll();
        }

        private void ExibirPopUp(string titulo, string mensagem)
        {
            // Cria um diálogo personalizado
            Dialog dialog = new Dialog(titulo, this, DialogFlags.Modal);
            dialog.SetDefaultSize(400, 200);

            // Configura o layout e estilo
            VBox contentBox = (VBox)dialog.ContentArea;

            // Configura o fundo claro e o texto
            contentBox.ModifyBg(StateType.Normal, new Gdk.Color(245, 245, 245)); // Fundo claro
            Label messageLabel = new Label(mensagem)
            {
                Xalign = 0.5f,
                Yalign = 0.5f
            };
            messageLabel.ModifyFg(StateType.Normal, new Gdk.Color(36, 36, 36)); // Texto escuro

            // Adiciona a mensagem ao diálogo
            contentBox.PackStart(messageLabel, true, true, 10);

            // Adiciona um botão para fechar o diálogo
            Button closeButton = new Button("OK");
            closeButton.Clicked += (s, e) => dialog.Destroy();
            contentBox.PackEnd(closeButton, false, false, 10);

            dialog.ShowAll();
        }




        public static void Main()
        {
            Application.Init();
            new MainWindow();
            Application.Run();
        }
    }
}
