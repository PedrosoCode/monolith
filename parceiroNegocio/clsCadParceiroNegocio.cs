using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace monolith.parceiroNegocio
{
    class clsCadParceiroNegocio
    {


        public void updateParceiroNegocio(int?  codigoParceiroAtual ,
                                          string sDocumento         ,
                                          string sNomeFantasia      ,
                                          string sRazaoSocial       ,
                                          string sEmail             ,
                                          string sContato           ,
                                          string sTelefone          ,
                                          string sTipo              ,
                                          int? lCodigoPais          ,
                                          int? iCodigoEstado        ,
                                          int? lCodigocidade        ,
                                          string sCep               ,
                                          string sLogradouro        ,
                                          string sNumero            ,
                                          string sComplemento       ,
                                          string sBairro
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
                                                                                              "@p_codigo_cidade,"   +
                                                                                              "@p_tipo_documento,"  +
                                                                                              "@p_cep,"             +
                                                                                              "@p_logradouro,"      +
                                                                                              "@p_numero,"          +
                                                                                              "@p_complemento,"     +
                                                                                              "@p_bairro"           +
                                                                                              ")"
                                                                                              , conn))
                    {

                        bool is_cnpj = false;
                        if (sDocumento.Length == 14)
                        {
                            is_cnpj = true;
                        }
                        else if (sDocumento.Length == 11)
                        {
                            is_cnpj = false;
                        }
                        else
                        {
                            throw new ArgumentException("O número fornecido não é um CPF ou CNPJ válido.");
                        }


                        if (sCep.Length != 8)
                        {
                            throw new ArgumentException("CEP inválido.");
                        }

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
                        command.Parameters.AddWithValue("@p_codigo_pais", lCodigoPais);
                        command.Parameters.AddWithValue("@p_codigo_estado", iCodigoEstado);
                        command.Parameters.AddWithValue("@p_codigo_cidade", lCodigocidade);
                        command.Parameters.AddWithValue("@p_tipo_documento", is_cnpj);
                        command.Parameters.AddWithValue("@p_cep", sCep);
                        command.Parameters.AddWithValue("@p_logradouro", sLogradouro);
                        command.Parameters.AddWithValue("@p_numero", sNumero);
                        command.Parameters.AddWithValue("@p_complemento", sComplemento);
                        command.Parameters.AddWithValue("@p_bairro", sBairro);

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

        

        public void excluirParceiroNegocio(int? codigoParceiroAtual)


        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using (var command = new NpgsqlCommand("CALL sp_delete_cadastro_parceiro_negocio(@p_codigo_empresa,"    +
                                                                                                     "@p_codigo_parceiro" +
                                                                                                     ")"
                                                                                                     , conn))
                    {

                        command.Parameters.AddWithValue("@p_codigo_empresa", Globals.GlobalCodigoEmpresa);
                        command.Parameters.AddWithValue("@p_codigo_parceiro", codigoParceiroAtual);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Parceiro deletado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar os dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void insertParceiroNegocio(string sDocumento,
                                          string sCep,
                                          string sTelefone,
                                          string sEmail,
                                          string sTipo,
                                          string sNomeFantasia,
                                          string sRazaoSocial,
                                          string sLogradouro,
                                          string sNumero,
                                          string sComplemento,
                                          string sBairro,
                                          string sContato,
                                          int? lCodigoPais,
                                          int? lCodigoCidade,
                                          int? iCodigoEstado
                                          )


        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using (var command = new NpgsqlCommand("CALL sp_insert_cadastro_basico_parceiro_negocio(@p_codigo_empresa,"    +
                                                                                                            "@p_documento,"        +
                                                                                                            "@p_cep,"              +
                                                                                                            "@p_telefone,"         +
                                                                                                            "@p_email,"            +
                                                                                                            "@p_data_cadastro,"    +
                                                                                                            "@p_tipo,"             +
                                                                                                            "@p_nome_fantasia,"    +
                                                                                                            "@p_razao_social,"     +
                                                                                                            "@p_logradouro,"       +
                                                                                                            "@p_numero,"           +
                                                                                                            "@p_complemento,"      +
                                                                                                            "@p_bairro,"           +
                                                                                                            "@p_contato,"          +
                                                                                                            "@p_codigo_pais,"      +
                                                                                                            "@p_codigo_cidade,"    +
                                                                                                            "@p_codigo_estado,"    +
                                                                                                            "@p_tipo_documento" +
                                                                                                            ")"
                                                                                                            , conn))
                    {

                        bool is_cnpj = false;
                        if (sDocumento.Length == 14)
                        {
                            is_cnpj = true;
                        }
                        else if (sDocumento.Length == 11)
                        {
                            is_cnpj = false;
                        }
                        else
                        {
                            throw new ArgumentException("O número fornecido não é um CPF ou CNPJ válido.");
                        }


                        if (sCep.Length != 8)
                        {
                            throw new ArgumentException("CEP inválido.");
                        }

                        command.Parameters.AddWithValue("@p_codigo_empresa", Globals.GlobalCodigoEmpresa);
                        command.Parameters.AddWithValue("@p_documento", sDocumento);
                        command.Parameters.AddWithValue("@p_cep", sCep);
                        command.Parameters.AddWithValue("@p_telefone", sTelefone);
                        command.Parameters.AddWithValue("@p_email", sEmail);
                        command.Parameters.AddWithValue("@p_data_cadastro", DateTime.Now);
                        command.Parameters.AddWithValue("@p_tipo", sTipo);
                        command.Parameters.AddWithValue("@p_nome_fantasia", sNomeFantasia);
                        command.Parameters.AddWithValue("@p_razao_social", sRazaoSocial);
                        command.Parameters.AddWithValue("@p_logradouro", sLogradouro);
                        command.Parameters.AddWithValue("@p_numero", sNumero);
                        command.Parameters.AddWithValue("@p_complemento", sComplemento);
                        command.Parameters.AddWithValue("@p_bairro", sBairro);
                        command.Parameters.AddWithValue("@p_contato", sContato);
                        command.Parameters.AddWithValue("@p_codigo_pais", lCodigoPais ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_codigo_cidade", lCodigoCidade ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_codigo_estado", iCodigoEstado ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_tipo_documento", is_cnpj);



                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dados inseridos com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inserir os dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
