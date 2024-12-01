using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace monolith.ativos
{
    class clsCadAtivo
    {

        public List<KeyValuePair<int, string>> CarregarComboParceiroListagem()
        {
            var parceiros = new List<KeyValuePair<int, string>>();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM public.fn_gen_combo_parceiros_negocio_razao_social(@codigo_empresa)", conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo_empresa", Globals.GlobalCodigoEmpresa);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int codigo = Convert.ToInt32(reader["codigo"]);
                                string razao_social = reader["razao_social"].ToString();
                                parceiros.Add(new KeyValuePair<int, string>(codigo, razao_social));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar parceiro de negócio: {ex.Message}");
                }
            }

            return parceiros;
        }

        public List<KeyValuePair<int, string>> CarregarComboFabricanteListagem()
        {
            var fabricantes = new List<KeyValuePair<int, string>>();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM public.fn_gen_combo_fabricante_nome_fantasia(@codigo_empresa)", conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo_empresa", Globals.GlobalCodigoEmpresa);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int codigo = Convert.ToInt32(reader["codigo"]);
                                string nome_fantasia = reader["nome_fantasia"].ToString();
                                fabricantes.Add(new KeyValuePair<int, string>(codigo, nome_fantasia));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar fabricantes: {ex.Message}");
                }
            }

            return fabricantes;
        }

        public void CarregarAtivos(DataGrid dataGrid,
                                    int? codigo              = null,
                                    int? codigoCliente       = null,
                                    string numeroSerie       = null,
                                    int? codigoFabricante    = null,
                                    string modelo            = null,
                                    string observacao        = null)
        {
            var dataTable = new System.Data.DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM public.fn_select_cad_ativo_dados(@p_codigo_empresa, " +
                                                                                                          "@p_codigo, " +
                                                                                                          "@p_codigo_cliente, " +
                                                                                                          "@p_numero_serie, " +
                                                                                                          "@p_codigo_fabricante, " +
                                                                                                          "@p_modelo, " +
                                                                                                          "@p_observacao" +
                                                                                                          ")", conn))
                    {
                        cmd.Parameters.AddWithValue("p_codigo_empresa", Globals.GlobalCodigoEmpresa);
                        cmd.Parameters.AddWithValue("p_codigo", (object)codigo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_codigo_cliente", (object)codigoCliente ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_numero_serie", (object)numeroSerie ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_codigo_fabricante", (object)codigoFabricante ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_modelo", (object)modelo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_observacao", (object)observacao ?? DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar ativos: {ex.Message}");
                }
            }

            dataGrid.ItemsSource = dataTable.DefaultView;
        }


    }
}
