using Npgsql;
using System;
using System.Configuration;
using System.Windows;
using BCrypt.Net;

namespace monolith
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CarregarEmpresas();
        }

        private void CarregarEmpresas()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM sp_carregar_empresas()", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbCodigoEmpresa.Items.Add(new { Codigo = reader.GetInt64(0), Nome = reader.GetString(1) });
                        }
                        cmbCodigoEmpresa.DisplayMemberPath = "Nome";
                        cmbCodigoEmpresa.SelectedValuePath = "Codigo";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar empresas: {ex.Message}");
                }
            }
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string senha = txtSenha.Password.Trim();
            var empresaSelecionada = cmbCodigoEmpresa.SelectedValue;

            if (empresaSelecionada == null)
            {
                MessageBox.Show("Por favor, selecione uma empresa.");
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT senha FROM public.tb_cad_usuario WHERE usuario = @usuario AND codigo_empresa = @codigo_empresa", conn))
                    {
                        cmd.Parameters.AddWithValue("usuario", usuario);
                        cmd.Parameters.AddWithValue("codigo_empresa", (long)empresaSelecionada);

                        var resultado = cmd.ExecuteScalar();

                        if (resultado != null)
                        {
                            string hashArmazenada = resultado.ToString();

                            if (BCrypt.Net.BCrypt.Verify(senha, hashArmazenada))
                            {
                                // Salva as informações do usuário e da empresa em variáveis globais
                                Globals.GlobalNomeUsuario = usuario;
                                Globals.GlobalCodigoEmpresa = (int)(long)empresaSelecionada;

                                MessageBox.Show($"Bem-vindo, {Globals.GlobalNomeUsuario}!");

                                HomeWindow homeWindow = new HomeWindow();
                                homeWindow.Show();
                                this.Close(); // Fecha a janela de login
                            }
                            else
                            {
                                MessageBox.Show("Senha incorreta.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Usuário ou empresa não encontrados.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao tentar login: {ex.Message}");
                }
            }
        }
    }
}
