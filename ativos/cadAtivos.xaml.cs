using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace monolith.ativos
{
    public partial class cadAtivos : UserControl
    {
        public cadAtivos()
        {
            InitializeComponent();
        }

        private void CarregarAtivos(int codigoEmpresa, 
                                    int? codigo = null, 
                                    int? codigoCliente = null, 
                                    string numeroSerie = null, 
                                    int? codigoFabricante = null, 
                                    string modelo = null, 
                                    string observacao = null)
        {
            var ativos = new List<Ativo>();

            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM public.fn_select_cad_ativo_dados(@p_codigo_empresa, @p_codigo, @p_codigo_cliente, @p_numero_serie, @p_codigo_fabricante, @p_modelo, @p_observacao)", conn))
                {
                    // Adiciona os parâmetros nomeados
                    cmd.Parameters.AddWithValue("p_codigo_empresa", codigoEmpresa);
                    cmd.Parameters.AddWithValue("p_codigo", (object)codigo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_codigo_cliente", (object)codigoCliente ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_numero_serie", (object)numeroSerie ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_codigo_fabricante", (object)codigoFabricante ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_modelo", (object)modelo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_observacao", (object)observacao ?? DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ativos.Add(new Ativo
                            {
                                ID = (int)reader["codigo_ativo"],
                                CodigoCliente = (int)reader["codigo_cliente_ativo"],
                                NumeroSerie = reader["numero_serie_ativo"].ToString(),
                                CodigoFabricante = (int)reader["codigo_fabricante_ativo"],
                                Modelo = reader["modelo_ativo"].ToString(),
                                Observacao = reader["observacao_ativo"].ToString(),
                                DataInput = (DateTime)reader["data_input_ativo"],
                                CodigoEmpresa = (int)reader["codigo_empresa_ativo"]
                            });
                        }
                    }
                }
            }

            dgAtivos.ItemsSource = ativos;
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            int codigoEmpresa = Globals.GlobalCodigoEmpresa;

            // Converte as entradas de texto para os tipos apropriados
            int? codigo = string.IsNullOrWhiteSpace(txtCodigo.Text) ? (int?)null : int.Parse(txtCodigo.Text.Trim());
            int? codigoCliente = string.IsNullOrWhiteSpace(txtCodigoCliente.Text) ? (int?)null : int.Parse(txtCodigoCliente.Text.Trim());
            string numeroSerie = string.IsNullOrWhiteSpace(txtNumeroSerie.Text) ? null : txtNumeroSerie.Text.Trim();
            int? codigoFabricante = string.IsNullOrWhiteSpace(txtCodigoFabricante.Text) ? (int?)null : int.Parse(txtCodigoFabricante.Text.Trim());
            string modelo = string.IsNullOrWhiteSpace(txtModelo.Text) ? null : txtModelo.Text.Trim();
            string observacao = string.IsNullOrWhiteSpace(txtObservacao.Text) ? null : txtObservacao.Text.Trim();

            // Chama o método para carregar os dados
            CarregarAtivos(codigoEmpresa, codigo, codigoCliente, numeroSerie, codigoFabricante, modelo, observacao);
        }
    }

    public class Ativo
    {
        public int ID { get; set; }
        public int CodigoCliente { get; set; }
        public string NumeroSerie { get; set; }
        public int CodigoFabricante { get; set; }
        public string Modelo { get; set; }
        public string Observacao { get; set; }
        public DateTime DataInput { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
