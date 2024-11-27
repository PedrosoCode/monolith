using monolith.ativos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
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

        private int? codigoParceiroAtual;
        clsCadParceiroNegocio FuncsParceiroNegocio = new clsCadParceiroNegocio();



        public cadParceiroNegocio()
        {

            InitializeComponent();
            MainTabControl.SelectedItem = TabListagem;

            // Carregar combos
            CarregarPaisesParaListagem();
            CarregarEstadosParaListagem();
            PopularComboTipo();
        }

        private void NovoParceiro()
        {

            codigoParceiroAtual             = null;
            txtDocumentoDados.Text          = "";
            txtNomeFantasiaDados.Text       = "";
            txtRazaoSocialDados.Text        = "";
            txtEmailDados.Text              = "";
            txtContatoDados.Text            = "";
            txtTelefoneDados.Text           = "";
            cboTipoDados.SelectedValue      = 0;
            cboPaisDados.SelectedValue      = 0;
            cboEstadoDados.SelectedValue    = 0;
            cboCidadeDados.SelectedValue    = 0;
            txtCEPDados.Text                = "";
            txtLogradouroDados.Text         = "";
            txtNumeroDados.Text             = "";
            txtComplementoDados.Text        = "";
            txtBairroDados.Text             = "";

            cboTipoDados.ItemsSource = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("A", "Ambos"),
                new KeyValuePair<string, string>("C", "Cliente"),
                new KeyValuePair<string, string>("F", "Fornecedor")
            };
            cboTipoDados.DisplayMemberPath = "Value";
            cboTipoDados.SelectedValuePath = "Key";

            var paises = CarregarPaises();
            cboPaisDados.ItemsSource = paises;
            cboPaisDados.DisplayMemberPath = "Value";
            cboPaisDados.SelectedValuePath = "Key";

            var estados = CarregarEstados();
            cboEstadoDados.ItemsSource = estados;
            cboEstadoDados.DisplayMemberPath = "Value";
            cboEstadoDados.SelectedValuePath = "Key";

            MainTabControl.SelectedItem = TabDados;
        }

        private void BtnNovo_Click(object sender, RoutedEventArgs e)
        {

            NovoParceiro();

        }
        private void PopularComboTipo()
        {
            var opcoes = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("A", "Ambos"),
                new KeyValuePair<string, string>("C", "Cliente"),
                new KeyValuePair<string, string>("F", "Fornecedor")
            };

            cboTipo.ItemsSource = opcoes;
            cboTipo.DisplayMemberPath = "Value";
            cboTipo.SelectedValuePath = "Key";
        }

        private void CarregarPaisesParaListagem()
        {
            var paises = CarregarPaises();
            cboPais.ItemsSource = paises;
            cboPais.DisplayMemberPath = "Value";
            cboPais.SelectedValuePath = "Key";
        }

        private void CarregarEstadosParaListagem()
        {
            var estados = CarregarEstados();
            cboEstado.ItemsSource = estados;
            cboEstado.DisplayMemberPath = "Value";
            cboEstado.SelectedValuePath = "Key";
        }

        private List<KeyValuePair<int, string>> CarregarPaises()
        {
            var paises = new List<KeyValuePair<int, string>>();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM public.fn_select_paises()", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["codigo_pais"]);
                                string nome = reader["nome_pais"].ToString();
                                paises.Add(new KeyValuePair<int, string>(id, nome));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar países: {ex.Message}");
                }
            }

            return paises;
        }

        private List<KeyValuePair<int, string>> CarregarEstados()
        {
            var estados = new List<KeyValuePair<int, string>>();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT id, nome, uf FROM tb_stc_estados", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["id"]);
                                string nome = reader["nome"].ToString();
                                estados.Add(new KeyValuePair<int, string>(id, nome));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar estados: {ex.Message}");
                }
            }

            return estados;
        }

        private List<KeyValuePair<int, string>> CarregarMunicipios(string ufEstado)
        {
            var municipios = new List<KeyValuePair<int, string>>();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT id, nome FROM municipio WHERE uf = @p_uf", conn))
                    {
                        cmd.Parameters.AddWithValue("@p_uf", (object)ufEstado ?? DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["id"]);
                                string nome = reader["nome"].ToString();
                                municipios.Add(new KeyValuePair<int, string>(id, nome));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar municípios: {ex.Message}");
                }
            }

            return municipios;
        }

        private string? ObterUfPorId(int idEstado)
        {
            string? uf = null;
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT uf FROM tb_stc_estados WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idEstado);
                        uf = cmd.ExecuteScalar()?.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao obter a sigla do estado: {ex.Message}");
                }
            }

            return uf;
        }

        private void cboEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            // Verificar qual ComboBox disparou o evento
            if (comboBox?.SelectedValue is int selectedEstadoId)
            {
                string? ufEstado = ObterUfPorId(selectedEstadoId);

                if (!string.IsNullOrEmpty(ufEstado))
                {
                    var municipios = CarregarMunicipios(ufEstado);

                    if (comboBox.Name == "cboEstado") // Aba de Listagem
                    {
                        cboCidade.ItemsSource = municipios;
                        cboCidade.DisplayMemberPath = "Value";
                        cboCidade.SelectedValuePath = "Key";
                        cboCidade.IsEnabled = municipios.Count > 0;
                    }
                    else if (comboBox.Name == "cboEstadoDados") // Aba de Dados
                    {
                        cboCidadeDados.ItemsSource = municipios;
                        cboCidadeDados.DisplayMemberPath = "Value";
                        cboCidadeDados.SelectedValuePath = "Key";
                        cboCidadeDados.IsEnabled = municipios.Count > 0;
                    }
                }
            }
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
            string? sTipo = Utils.ParseNullableString(cboTipo.SelectedValue?.ToString());
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
                                    Codigo_parceiro = Convert.ToInt32(reader["codigo_parceiro"]),
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

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                codigoParceiroAtual = button.Tag as int?;

                CarregarDadosParceiro(codigoParceiroAtual, Globals.GlobalCodigoEmpresa);
            }
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {

            try
            {

            

            String sDocumento       = txtDocumentoDados.Text.Trim();
            String sNomeFantasia    = txtNomeFantasiaDados.Text.Trim();
            String sRazaoSocial     = txtRazaoSocialDados.Text.Trim();
            String sEmail           = txtEmailDados.Text.Trim();
            String sContato         = txtContatoDados.Text.Trim();
            String sTelefone        = txtTelefoneDados.Text.Trim();
            String? sTipo           = cboTipoDados.SelectedValue as String;
            int? lCodigoPais        = cboPaisDados.SelectedValue as int?;
            int? iCodigoEstado      = cboEstadoDados.SelectedValue as int?;
            int? lCodigoCidade      = cboCidadeDados.SelectedValue as int?;
            String sCep             = txtCEPDados.Text.Trim();
            String sLogradouro      = txtLogradouroDados.Text.Trim();
            String sNumero          = txtNumeroDados.Text.Trim();
            String sComplemento     = txtComplementoDados.Text.Trim();
            String sBairro          = txtBairroDados.Text.Trim();  


            if (codigoParceiroAtual == null)
            {


                FuncsParceiroNegocio.insertParceiroNegocio(sDocumento   ,
                                                           sCep         ,
                                                           sTelefone    ,
                                                           sEmail       ,
                                                           sTipo        ,
                                                           sNomeFantasia,
                                                           sRazaoSocial ,
                                                           sLogradouro  ,
                                                           sNumero      ,
                                                           sComplemento ,
                                                           sBairro      ,
                                                           sContato     ,
                                                           lCodigoPais  ,
                                                           lCodigoCidade,
                                                           iCodigoEstado
                                                           );
            }
            else
            {
                FuncsParceiroNegocio.updateParceiroNegocio( codigoParceiroAtual ,
                                                             sDocumento         ,
                                                             sNomeFantasia      ,
                                                             sRazaoSocial       ,
                                                             sEmail             ,
                                                             sContato           ,
                                                             sTelefone          ,
                                                             sTipo              ,
                                                             lCodigoPais        ,
                                                             iCodigoEstado      ,
                                                             lCodigoCidade      ,
                                                             sCep               ,
                                                             sLogradouro        ,
                                                             sNumero            ,
                                                             sComplemento       ,
                                                             sBairro
                                                            );
            }

                NovoParceiro();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os detalhes do parceiro: {ex.Message}");
            }

        }

        private void CarregarDadosParceiro(int? codigoParceiroAtual, 
                                           int? codigoEmpresa)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM fn_cad_parceiro_negocio_dados(@p_codigo, "        +
                                                                                                    "@p_codigo_empresa" +
                                                                                                    ")", conn))
                    {
                        cmd.Parameters.AddWithValue("@p_codigo", codigoParceiroAtual);
                        cmd.Parameters.AddWithValue("@p_codigo_empresa", codigoEmpresa);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtDocumentoDados.Text = reader["documento"]?.ToString();
                                txtNomeFantasiaDados.Text = reader["nome_fantasia"]?.ToString();
                                txtRazaoSocialDados.Text = reader["razao_social"]?.ToString();
                                txtEmailDados.Text = reader["email"]?.ToString();
                                txtContatoDados.Text = reader["contato"]?.ToString();
                                txtTelefoneDados.Text = reader["telefone"]?.ToString();
                                txtBairroDados.Text = reader["bairro"]?.ToString();
                                txtLogradouroDados.Text = reader["logradouro"]?.ToString();
                                txtNumeroDados.Text = reader["numero"]?.ToString();
                                txtCEPDados.Text = reader["cep"]?.ToString();
                                txtComplementoDados.Text = reader["complemento"]?.ToString();

                                cboTipoDados.ItemsSource = new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("A", "Ambos"),
                                    new KeyValuePair<string, string>("C", "Cliente"),
                                    new KeyValuePair<string, string>("F", "Fornecedor")
                                };
                                cboTipoDados.DisplayMemberPath = "Value";
                                cboTipoDados.SelectedValuePath = "Key";
                                cboTipoDados.SelectedValue = reader["tipo_parceiro"]?.ToString();

                                var paises = CarregarPaises();
                                cboPaisDados.ItemsSource = paises;
                                cboPaisDados.DisplayMemberPath = "Value";
                                cboPaisDados.SelectedValuePath = "Key";
                                cboPaisDados.SelectedValue = Convert.ToInt32(reader["codigo_pais"]);

                                var estados = CarregarEstados();
                                cboEstadoDados.ItemsSource = estados;
                                cboEstadoDados.DisplayMemberPath = "Value";
                                cboEstadoDados.SelectedValuePath = "Key";
                                cboEstadoDados.SelectedValue = Convert.ToInt32(reader["codigo_estado"]);

                                string ufEstado = ObterUfPorId(Convert.ToInt32(reader["codigo_estado"]));
                                var municipios = CarregarMunicipios(ufEstado);
                                cboCidadeDados.ItemsSource = municipios;
                                cboCidadeDados.DisplayMemberPath = "Value";
                                cboCidadeDados.SelectedValuePath = "Key";
                                cboCidadeDados.SelectedValue = Convert.ToInt32(reader["codigo_cidade"]);

                                MainTabControl.SelectedItem = TabDados;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar os detalhes do parceiro: {ex.Message}");
                }
            }
        }

        public class clsParceiro
        {
            public int Codigo_parceiro { get; set; }
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
