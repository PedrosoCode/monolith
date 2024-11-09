using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Npgsql;
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
            string usuario = txtUsuario.Text;
            string senha = txtSenha.Password;
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
                    // Chamada para buscar a hash da senha do usuário no banco de dados
                    using (var cmd = new NpgsqlCommand("SELECT senha FROM public.tb_cad_usuario WHERE usuario = @usuario AND codigo_empresa = @codigo_empresa", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("usuario", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuario });
                        cmd.Parameters.Add(new NpgsqlParameter("codigo_empresa", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = (long)empresaSelecionada });

                        var resultado = cmd.ExecuteScalar();

                        if (resultado != null)
                        {
                            string hashArmazenada = resultado.ToString();

                            // Verifique a senha usando BCrypt
                            if (BCrypt.Net.BCrypt.Verify(senha, hashArmazenada))
                            {
                                MessageBox.Show("Login realizado com sucesso!");
                                // Redirecionamento ou outra lógica pode ser inserida aqui
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
