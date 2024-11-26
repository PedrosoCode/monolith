using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace monolith.parceiroNegocio
{
    class clsCadParceiroNegocio
    {


        public void updateParceiroNegocio(int? codigoParceiroAtual,
                                          String sDocumento,
                                          String sNomeFantasia,
                                          String sRazaoSocial,
                                          String sEmail,
                                          String sContato,
                                          String sTelefone,
                                          String sTipo,
                                          int? iCodigoPais,
                                          int? iCodigoEstado,
                                          int? iCodigocidade
                                           )
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using (var command = new NpgsqlCommand("CALL sp_atualizar_parceiro_negocio(@p_codigo, "         +
                                                                                              "@p_codigo_empresa, " +
                                                                                              "@p_documento,"       +
                                                                                              "@p_nome_fantasia,"   +
                                                                                              "@p_razao_social,"    +
                                                                                              "@p_email,"           +
                                                                                              "@p_contato,"         +
                                                                                              "@p_telefone,"        +
                                                                                              "@p_tipo,"            +
                                                                                              "@p_codigo_pais,"     +
                                                                                              "@p_codigo_estado,"   +
                                                                                              "@p_codigo_cidade"    +
                                                                                              ")"
                                                                                              , conn))
                    {
                        // Define os parâmetros da stored procedure
                        command.Parameters.AddWithValue("@p_codigo", codigoParceiroAtual);
                        command.Parameters.AddWithValue("@p_codigo_empresa", Globals.GlobalCodigoEmpresa);
                        command.Parameters.AddWithValue("@p_documento", sDocumento);
                        command.Parameters.AddWithValue("@p_nome_fantasia", sNomeFantasia);
                        command.Parameters.AddWithValue("@p_razao_social", sRazaoSocial);
                        command.Parameters.AddWithValue("@p_email", sEmail);
                        command.Parameters.AddWithValue("@p_contato", sContato);
                        command.Parameters.AddWithValue("@p_telefone", sTelefone);
                        command.Parameters.AddWithValue("@p_tipo", sTipo);
                        command.Parameters.AddWithValue("@p_codigo_pais", iCodigoPais);
                        command.Parameters.AddWithValue("@p_codigo_estado", iCodigoEstado);
                        command.Parameters.AddWithValue("@p_codigo_cidade", iCodigocidade);
                        


                        // Executa a stored procedure
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dados atualizados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar os dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
