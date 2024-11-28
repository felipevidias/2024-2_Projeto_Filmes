using System;
using System.Security.Cryptography;
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
            try
            {
                // Criptografa os campos antes de armazená-los no banco
                string tituloCriptografado = EncryptString(tituloEntry.Text);
                string diretorCriptografado = EncryptString(diretorEntry.Text);
                string generoCriptografado = EncryptString(generoEntry.Text);

                // Criptografando o ano (convertendo para string antes)
                int anoInt = int.Parse(anoEntry.Text);  // Converte o ano para inteiro
                string anoCriptografado = EncryptString(anoInt.ToString());  // Criptografa o ano como string

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO filmes (titulo, diretor, genero, ano) VALUES (@titulo, @diretor, @genero, @ano)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@titulo", tituloCriptografado);
                        cmd.Parameters.AddWithValue("@diretor", diretorCriptografado);
                        cmd.Parameters.AddWithValue("@genero", generoCriptografado);
                        cmd.Parameters.AddWithValue("@ano", anoCriptografado);  // Inserir o ano criptografado como string

                        cmd.ExecuteNonQuery();
                    }
                }

                ExibirPopUp("Sucesso", "Filme adicionado com sucesso!");
            }
            catch (Exception ex)
            {
                ExibirPopUp("Erro", $"Ocorreu um erro ao adicionar o filme: {ex.Message}");
            }
        }


        private void SaveDecryptedDataToFile(string titulo, string diretor, int ano, string genero)
        {
            string filePath = "filmes_decriptografados.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true)) // 'true' para adicionar ao arquivo
            {
                writer.WriteLine($"Título: {titulo}");
                writer.WriteLine($"Diretor: {diretor}");
                writer.WriteLine($"Ano: {ano}");
                writer.WriteLine($"Gênero: {genero}");
                writer.WriteLine(); // Adiciona uma linha em branco entre os filmes
            }
        }


        private string EncryptString(string plainText)
        {
            string key = "1234567890123456"; // Chave de 16 caracteres (128 bits)
            string iv = "6543210987654321"; // Vetor de inicialização de 16 caracteres

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private void SaveEncryptedDataToFile(string encryptedData)
        {
            string filePath = "filmes_criptografados.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true)) // 'true' para adicionar ao arquivo
            {
                writer.WriteLine(encryptedData);
            }
        }

        private void SaveDecryptedDataToFile(string decryptedData)
        {
            string filePath = "filmes_decriptografados.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true)) // 'true' para adicionar ao arquivo
            {
                writer.WriteLine(decryptedData);
            }
        }


        // Nova função para ler os dados criptografados do arquivo
        private string ReadEncryptedDataFromFile()
        {
            string filePath = "filmes_criptografados.txt";
            string encryptedData = string.Empty;
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    encryptedData = reader.ReadToEnd();
                }
            }
            return encryptedData; // Retorna os dados criptografados que foram lidos
        }

        private string DecryptString(string encryptedText)
        {
            string key = "1234567890123456"; // Chave de 16 caracteres (128 bits)
            string iv = "6543210987654321"; // Vetor de inicialização de 16 caracteres

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd(); // Retorna os dados descriptografados
                        }
                    }
                }
            }
        }

        private void UpdateEncryptedFile()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM filmes";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Limpa o arquivo criptografado antes de reescrever
                            File.WriteAllText("filmes_criptografados.txt", string.Empty);

                            while (reader.Read())
                            {
                                string titulo = reader["titulo"].ToString();
                                string diretor = reader["diretor"].ToString();
                                int ano = Convert.ToInt32(reader["ano"]);
                                string genero = reader["genero"].ToString();

                                string filmeData = $"{titulo},{diretor},{ano},{genero}";
                                string encryptedData = EncryptString(filmeData);

                                // Reescreve o arquivo com dados criptografados
                                SaveEncryptedDataToFile(encryptedData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExibirPopUp("Erro", $"Ocorreu um erro ao atualizar o arquivo criptografado: {ex.Message}");
            }
        }


        private void AtualizarArquivoFilmes()
        {
            try
            {
                // Cria uma lista de filmes com dados descriptografados
                List<string> filmesDescriptografados = new List<string>();

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM filmes";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string titulo = DecryptString(reader["titulo"].ToString());
                                    string diretor = DecryptString(reader["diretor"].ToString());
                                    string genero = DecryptString(reader["genero"].ToString());
                                    string ano = DecryptString(reader["ano"].ToString());

                                    // Formata a linha para o .txt
                                    string filme = $"Título: {titulo}\nDiretor: {diretor}\nAno: {ano}\nGênero: {genero}\n";
                                    filmesDescriptografados.Add(filme);
                                }
                            }
                            else
                            {
                                filmesDescriptografados.Add("Nenhum filme encontrado.");
                            }
                        }
                    }
                }

                // Reescreve o arquivo .txt com filmes descriptografados
                File.WriteAllLines("filmes.txt", filmesDescriptografados);
            }
            catch (Exception ex)
            {
                ExibirPopUp("Erro", $"Ocorreu um erro ao atualizar o arquivo de filmes: {ex.Message}");
            }
        }


        private void OnUpdateFilme(object sender, EventArgs e)
        {
            try
            {
                string idFilme = idEntry.Text;
                string novoTitulo = tituloEntry.Text;
                string novoDiretor = diretorEntry.Text;
                string novoGenero = generoEntry.Text;
                string novoAno = anoEntry.Text;

                // Criptografa os dados antes de atualizar no banco
                string tituloCriptografado = EncryptString(novoTitulo);
                string diretorCriptografado = EncryptString(novoDiretor);
                string generoCriptografado = EncryptString(novoGenero);
                string anoCriptografado = EncryptString(novoAno);

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE filmes SET titulo = @titulo, diretor = @diretor, genero = @genero, ano = @ano WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@titulo", tituloCriptografado);
                        cmd.Parameters.AddWithValue("@diretor", diretorCriptografado);
                        cmd.Parameters.AddWithValue("@genero", generoCriptografado);
                        cmd.Parameters.AddWithValue("@ano", anoCriptografado);
                        cmd.Parameters.AddWithValue("@id", idFilme);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Atualiza o arquivo .txt após atualizar o filme
                AtualizarArquivoFilmes();

                ExibirPopUp("Sucesso", "Filme atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                ExibirPopUp("Erro", $"Ocorreu um erro ao atualizar o filme: {ex.Message}");
            }
        }


        private void OnListFilme(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = searchEntry.Text.ToLower(); // Normaliza o termo de busca
                List<string> resultados = new List<string>();

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM filmes"; // Seleciona todos os filmes para processamento manual
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Descriptografa os campos
                                string titulo = DecryptString(reader["titulo"].ToString());
                                string diretor = DecryptString(reader["diretor"].ToString());
                                string genero = DecryptString(reader["genero"].ToString());
                                string ano = DecryptString(reader["ano"].ToString());

                                // Verifica se o termo de busca está presente em algum campo
                                if (titulo.ToLower().Contains(searchTerm) ||
                                    diretor.ToLower().Contains(searchTerm) ||
                                    genero.ToLower().Contains(searchTerm) ||
                                    ano.ToLower().Contains(searchTerm))
                                {
                                    // Formata o resultado para exibição
                                    resultados.Add($"Título: {titulo}\nDiretor: {diretor}\nAno: {ano}\nGênero: {genero}\n");
                                }
                            }
                        }
                    }
                }

                // Exibe os resultados encontrados ou uma mensagem de "não encontrado"
                if (resultados.Count > 0)
                {
                    ExibirPopUp("Resultado da Busca", string.Join("\n\n", resultados));
                }
                else
                {
                    ExibirPopUp("Resultado da Busca", "Nenhum filme encontrado.");
                }
            }
            catch (Exception ex)
            {
                ExibirPopUp("Erro", $"Ocorreu um erro ao buscar o filme: {ex.Message}");
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
            try
            {
                string idFilme = idEntry.Text;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM filmes WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idFilme);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Atualiza o arquivo .txt após deletar o filme
                AtualizarArquivoFilmes();

                ExibirPopUp("Sucesso", "Filme deletado com sucesso!");
            }
            catch (Exception ex)
            {
                ExibirPopUp("Erro", $"Ocorreu um erro ao deletar o filme: {ex.Message}");
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
            Box contentBox = (Box)dialog.ContentArea; // Troquei Gtk.VBox por Gtk.Box

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
