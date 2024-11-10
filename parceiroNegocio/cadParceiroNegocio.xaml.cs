using monolith.ativos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace monolith.parceiroNegocio
{
    public partial class cadParceiroNegocio : UserControl
    {
        public cadParceiroNegocio()
        {
            InitializeComponent();
            MainTabControl.SelectedItem = TabListagem;
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            int iCodigoEmpresa = Globals.GlobalCodigoEmpresa;
            string? sDocumento = Utils.ParseNullableString(txtDocumento.Text.Trim());
            int? iCodigoPais = Utils.ParseNullableInt(cboPais.SelectedValue);
            int? iCodigoCidade = Utils.ParseNullableInt(cboCidade.SelectedValue);
            int? iCodigoEstado = Utils.ParseNullableInt(cboEstado.SelectedValue);
            string? sTelefone = Utils.ParseNullableString(txtTelefone.Text.Trim());
            string? sEmail = Utils.ParseNullableString(txtEmail.Text.Trim());
            string? sTipo = Utils.ParseNullableString(cboTipo.SelectedValue);
            string? sNomeFantasia = Utils.ParseNullableString(txtNomeFantasia.Text.Trim());
            string? sRazaoSocial = Utils.ParseNullableString(txtRazaoSocial.Text.Trim());

            CarregarParceiros(iCodigoEmpresa,
                              sDocumento,
                              iCodigoPais,
                              iCodigoCidade,
                              iCodigoEstado,
                              sTelefone,
                              sEmail,
                              sTipo,
                              sNomeFantasia,
                              sRazaoSocial);
        }

        private void CarregarParceiros(int codigoEmpresa,
                                       string? sDocumento,
                                       int? iCodigoPais,
                                       int? iCodigoCidade,
                                       int? iCodigoEstado,
                                       string? sTelefone,
                                       string? sEmail,
                                       string? sTipo,
                                       string? sNomeFantasia,
                                       string? sRazaoSocial)
        {
            var parceiros = new List<clsParceiro>();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM public.fn_gen_filtro_parceiro_negocio(" +
                                                        "@p_codigo_empresa, " +
                                                        "@p_documento, " +
                                                        "@p_codigo_pais, " +
                                                        "@p_codigo_cidade, " +
                                                        "@p_codigo_estado, " +
                                                        "@p_telefone, " +
                                                        "@p_email, " +
                                                        "@p_tipo, " +
                                                        "@p_nome_fantasia, " +
                                                        "@p_razao_social)", conn))
                    {
                        cmd.Parameters.AddWithValue("p_codigo_empresa", codigoEmpresa);
                        cmd.Parameters.AddWithValue("p_documento", (object)sDocumento ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_codigo_pais", (object)iCodigoPais ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_codigo_cidade", (object)iCodigoCidade ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_codigo_estado", (object)iCodigoEstado ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_telefone", (object)sTelefone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_email", (object)sEmail ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_tipo", (object)sTipo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_nome_fantasia", (object)sNomeFantasia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_razao_social", (object)sRazaoSocial ?? DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                parceiros.Add(new clsParceiro
                                {
                                    Codigo_uf_parceiro = reader["codigo_uf_parceiro"]?.ToString(),
                                    Documento = reader["documento_parceiro"]?.ToString(),
                                    Telefone_parceiro = reader["telefone_parceiro"]?.ToString(),
                                    Email_parceiro = reader["email_parceiro"]?.ToString(),
                                    Codigo_empresa = Convert.ToInt32(reader["codigo_empresa"]),
                                    Nome_fantasia_parceiro = reader["nome_fantasia_parceiro"]?.ToString(),
                                    Razao_social_parceiro = reader["razao_social_parceiro"]?.ToString(),
                                    Municipio_parceiro = reader["municipio_parceiro"]?.ToString(),
                                    Pais_parceiro = reader["pais_parceiro"]?.ToString(),
                                    Estado_parceiro = reader["estado_parceiro"]?.ToString(),
                                    Codigo_municipio_parceiro = Convert.ToInt32(reader["codigo_municipio_parceiro"]),
                                    Codigo_estado_parceiro = reader["codigo_estado_parceiro"]?.ToString(),
                                    Endereco = reader["endereco"]?.ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar parceiros: {ex.Message}");
                }
            }

            dgParceirosListagem.ItemsSource = parceiros;
        }

        public class clsParceiro
        {
            public string Codigo_uf_parceiro { get; set; }
            public string Documento { get; set; }
            public string Telefone_parceiro { get; set; }
            public string Email_parceiro { get; set; }
            public int Codigo_empresa { get; set; }
            public string Nome_fantasia_parceiro { get; set; }
            public string Razao_social_parceiro { get; set; }
            public string Municipio_parceiro { get; set; }
            public string Pais_parceiro { get; set; }
            public string Estado_parceiro { get; set; }
            public int Codigo_municipio_parceiro { get; set; }
            public string Codigo_estado_parceiro { get; set; }
            public string Endereco { get; set; }
        }
    }
}
