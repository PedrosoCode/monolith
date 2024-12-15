using Monolith.Ativos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using static System.Net.Mime.MediaTypeNames;


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
                                   int? codigoCliente       = null,
                                   string numeroSerie       = null,
                                   int? codigoFabricante    = null,
                                   string modelo            = null,
                                   string observacao        = null
                                   )
        {
            var dataTable = new System.Data.DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM public.fn_select_cad_ativo_dados(@p_codigo_empresa, "      +
                                                                                                       "@p_codigo_cliente, "     +
                                                                                                       "@p_numero_serie, "       +
                                                                                                       "@p_codigo_fabricante, "  +
                                                                                                       "@p_modelo, "             +
                                                                                                       "@p_observacao"           +
                                                                                                       ")", conn))
                    {
                        cmd.Parameters.AddWithValue("p_codigo_empresa"      , Globals.GlobalCodigoEmpresa              );
                        cmd.Parameters.AddWithValue("p_codigo_cliente"      , (object)codigoCliente     ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_numero_serie"        , (object)numeroSerie       ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_codigo_fabricante"   , (object)codigoFabricante  ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_modelo"              , (object)modelo            ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_observacao"          , (object)observacao        ?? DBNull.Value);

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

        public List<FotoAtivo> ObterCaminhosImagens(int? iCodigoAtivo)
        {
            var fotos = new List<FotoAtivo>();

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@p_codigo_empresa"   , Globals.GlobalCodigoEmpresa           },
                    { "@p_codigo"           , iCodigoAtivo ?? (object)DBNull.Value  }
                };

                string commandText = "SELECT caminho_completo, titulo " +
                                     "FROM tb_cad_ativo_foto "          +
                                     "WHERE codigo_empresa = @p_codigo_empresa";

                var dbHelper = new DatabaseHelper();
                using (var reader = dbHelper.ExecuteReader(commandText, parameters))
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            var caminhoCompleto = reader.GetString(0);
                            var nomeImagem = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            fotos.Add(new FotoAtivo(caminhoCompleto, nomeImagem));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao obter caminhos das imagens: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return fotos;
        }

        public string ObterCaminhoUpload()
        {
            string commandText = "SELECT "       +
                                 "imagem_ativo " +
                                 "FROM tb_cfg_caminho_arquivos_gerais";

            var dbHelper = new DatabaseHelper();
            using (var reader = dbHelper.ExecuteReader(commandText))
            {
                if (reader.Read())
                {
                    return reader.GetString(0);
                }
            }

            throw new Exception("Caminho de upload não configurado.");
        }

        public Dictionary<string, object> LoadDadosAtivo(int? iCodigoAtivoAtual
                                                        )
        {
            var dados = new Dictionary<string, object>();

            try
            {

                var parameters = new Dictionary<string, object>
                {
                    { "@p_codigo_empresa"   , Globals.GlobalCodigoEmpresa               },
                    { "@p_codigo_ativo"     , iCodigoAtivoAtual ?? (object)DBNull.Value }
                };

                string commandText = "SELECT * FROM fn_cadastro_ativo_select_dados(@p_codigo_empresa, " +
                                                                                  "@p_codigo_ativo"     +
                                                                                  ")";

                var dbHelper = new DatabaseHelper();
                using (var reader = dbHelper.ExecuteReader(commandText, parameters))
                {
                    if (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dados[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao obter dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return dados;
        }

        public void UploadImagem(int? iCodigoAtivo,
                         string sArquivo,
                         string sCaminhoUpload,
                         string sNomeFoto)
        {
            try
            {
                // Gerar um GUID único para o arquivo
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(sArquivo); // Exemplo: "b3d1d2b0-5358-44e8-ae6f-b2db56b967f3.png"

                string sCaminhoDestino = Path.Combine(sCaminhoUpload, uniqueFileName);

                if (!Directory.Exists(sCaminhoUpload))
                {
                    Directory.CreateDirectory(sCaminhoUpload);
                }

                File.Copy(sArquivo, sCaminhoDestino, overwrite: true);

                var parameters = new Dictionary<string, object>
                {
                    { "@p_codigo_empresa"   , Globals.GlobalCodigoEmpresa           },
                    { "@p_codigo_ativo"     , iCodigoAtivo ?? (object)DBNull.Value  },
                    { "@p_caminho_completo" , sCaminhoDestino                       }, 
                    { "@p_titulo"           , sNomeFoto                             }
                };

                var commandText = "CALL sp_insert_cadastro_basico_ativo_foto(@p_codigo_ativo, " +
                                                                            "@p_codigo_empresa, "     +
                                                                            "@p_titulo, "             +
                                                                            "@p_caminho_completo "    +
                                                                            ")";

                var dbHelper = new DatabaseHelper();
                dbHelper.ExecuteCommand(commandText, parameters);

                MessageBox.Show("Imagem carregada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao fazer upload da imagem: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void inserirAtivo(int? iCodigoAtivoAtual,
                                 string sNumeroSerie,
                                 string sModelo,
                                 string sAlias,
                                 string sObservacao,
                                 int? iCodigoParceiroNegocio,
                                 int? iCodigoFabricante,
                                 string sTexto
                                 )
        {
            try
            {

                var parameters = new Dictionary<string, object>
                {
                    { "@p_codigo_cliente"       , iCodigoParceiroNegocio       },
                    { "@p_numero_serie"         , sNumeroSerie                 },
                    { "@p_codigo_fabricante"    , iCodigoFabricante            },
                    { "@p_modelo"               , sModelo                      },
                    { "@p_observacao"           , sObservacao                  },
                    { "@p_codigo_empresa"       , Globals.GlobalCodigoEmpresa  },
                    { "@p_descricao"            , sTexto                       },
                    { "@p_alias"                , sAlias                       },
                };

                var commandText = "CALL sp_insert_cadastro_basico_ativo(@p_codigo_cliente, "     +
                                                                       "@p_numero_serie, "       +
                                                                       "@p_codigo_fabricante, "  +
                                                                       "@p_modelo, "             +
                                                                       "@p_observacao, "         +
                                                                       "@p_codigo_empresa, "     +
                                                                       "@p_descricao, "          +
                                                                       "@p_alias "               +
                                                                       ")";

                var dbHelper = new DatabaseHelper();
                dbHelper.ExecuteCommand(commandText, parameters);

                MessageBox.Show("Ativo cadastrado!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inserir os dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }














    }
}
